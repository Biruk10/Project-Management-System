using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Budgets.DTOs;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Domain.Entities;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Application.Budgets.Services;

public class BudgetRequestService : IBudgetRequestService
{
    private readonly IAppDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ICurrentUserService _currentUserService;

    public BudgetRequestService(
        IAppDbContext context,
        ITenantService tenantService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _tenantService = tenantService;
        _currentUserService = currentUserService;
    }

    public async Task<List<BudgetRequestDto>> GetByProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var list = await _context.BudgetRequests
            .Include(br => br.Project).ThenInclude(p => p.ProjectManager)
            .Include(br => br.ReviewedByUser)
            .Where(br => br.ProjectId == projectId)
            .OrderByDescending(br => br.CreatedAt)
            .ToListAsync(cancellationToken);

        return list.Select(MapToDto).ToList();
    }

    public async Task<List<BudgetRequestDto>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        var list = await _context.BudgetRequests
            .Include(br => br.Project).ThenInclude(p => p.ProjectManager)
            .Include(br => br.ReviewedByUser)
            .Where(br => br.Status == BudgetRequestStatus.Pending)
            .OrderByDescending(br => br.CreatedAt)
            .ToListAsync(cancellationToken);

        return list.Select(MapToDto).ToList();
    }

    public async Task<List<BudgetRequestDto>> GetAllAsync(int? projectId = null, BudgetRequestStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.BudgetRequests
            .Include(br => br.Project).ThenInclude(p => p.ProjectManager)
            .Include(br => br.ReviewedByUser)
            .AsQueryable();

        if (projectId.HasValue)
            query = query.Where(br => br.ProjectId == projectId.Value);

        if (status.HasValue)
            query = query.Where(br => br.Status == status.Value);

        var list = await query.OrderByDescending(br => br.CreatedAt).ToListAsync(cancellationToken);
        return list.Select(MapToDto).ToList();
    }

    public async Task<BudgetRequestDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _context.BudgetRequests
            .Include(br => br.Project).ThenInclude(p => p.ProjectManager)
            .Include(br => br.ReviewedByUser)
            .FirstOrDefaultAsync(br => br.Id == id, cancellationToken);

        return item == null ? null : MapToDto(item);
    }

    public async Task<BudgetRequestDto> CreateAsync(CreateBudgetRequestDto dto, CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var project = await _context.Projects
            .Include(p => p.Budgets).ThenInclude(b => b.BudgetLines)
            .FirstOrDefaultAsync(p => p.Id == dto.ProjectId, cancellationToken)
            ?? throw new KeyNotFoundException($"Project {dto.ProjectId} not found.");

        if (dto.RequestedAmount <= 0)
            throw new ArgumentException("Requested amount must be greater than 0.");

        if (string.IsNullOrWhiteSpace(dto.Reason))
            throw new ArgumentException("Reason is required.");

        // Determine current budget for this category if existing
        decimal currentCategoryBudget = 0;
        int? budgetLineId = dto.BudgetLineId;

        var primaryBudget = project.Budgets.OrderByDescending(b => b.CreatedAt).FirstOrDefault();
        if (primaryBudget != null)
        {
            var line = budgetLineId.HasValue
                ? primaryBudget.BudgetLines.FirstOrDefault(l => l.Id == budgetLineId.Value)
                : primaryBudget.BudgetLines.FirstOrDefault(l => l.Category.Equals(dto.Category, StringComparison.OrdinalIgnoreCase));

            if (line != null)
            {
                currentCategoryBudget = line.AllocatedAmount;
                budgetLineId = line.Id;
            }
        }

        var request = new BudgetRequest
        {
            OrganizationId = orgId,
            ProjectId = dto.ProjectId,
            BudgetLineId = budgetLineId,
            Category = string.IsNullOrWhiteSpace(dto.Category) ? "General" : dto.Category.Trim(),
            CurrentBudget = currentCategoryBudget,
            RequestedAmount = dto.RequestedAmount,
            Reason = dto.Reason.Trim(),
            Status = BudgetRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _currentUserService.UserId
        };

        _context.BudgetRequests.Add(request);
        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(request.Id, cancellationToken))!;
    }

    public async Task<BudgetRequestDto> ReviewAsync(int id, ReviewBudgetRequestDto dto, CancellationToken cancellationToken = default)
    {
        var request = await _context.BudgetRequests
            .Include(br => br.Project).ThenInclude(p => p.Budgets).ThenInclude(b => b.BudgetLines)
            .FirstOrDefaultAsync(br => br.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Budget request {id} not found.");

        if (request.Status != BudgetRequestStatus.Pending)
            throw new InvalidOperationException("This budget request has already been reviewed.");

        request.ReviewedBy = _currentUserService.UserId;
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewComment = dto.ReviewComment?.Trim();

        if (dto.Approve)
        {
            request.Status = BudgetRequestStatus.Approved;

            // Apply requested amount to project budget
            var project = request.Project;
            var primaryBudget = project.Budgets.OrderByDescending(b => b.CreatedAt).FirstOrDefault();

            if (primaryBudget == null)
            {
                // Create budget if none existed
                primaryBudget = new Budget
                {
                    OrganizationId = request.OrganizationId,
                    ProjectId = request.ProjectId,
                    TotalAmount = request.RequestedAmount,
                    Status = BudgetStatus.Approved,
                    ApprovedBy = _currentUserService.UserId,
                    ApprovedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = _currentUserService.UserId
                };
                _context.Budgets.Add(primaryBudget);
                await _context.SaveChangesAsync(cancellationToken);

                var line = new BudgetLine
                {
                    BudgetId = primaryBudget.Id,
                    Category = request.Category,
                    AllocatedAmount = request.RequestedAmount,
                    Description = $"Budget request approved: {request.Reason}"
                };
                _context.BudgetLines.Add(line);
                await _context.SaveChangesAsync(cancellationToken);
                request.BudgetLineId = line.Id;
            }
            else
            {
                // Increase total amount
                primaryBudget.TotalAmount += request.RequestedAmount;
                primaryBudget.UpdatedAt = DateTime.UtcNow;
                primaryBudget.UpdatedByUserId = _currentUserService.UserId;

                // Find matching category line or create a new one
                var line = request.BudgetLineId.HasValue
                    ? primaryBudget.BudgetLines.FirstOrDefault(l => l.Id == request.BudgetLineId.Value)
                    : primaryBudget.BudgetLines.FirstOrDefault(l => l.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase));

                if (line != null)
                {
                    line.AllocatedAmount += request.RequestedAmount;
                }
                else
                {
                    var newLine = new BudgetLine
                    {
                        BudgetId = primaryBudget.Id,
                        Category = request.Category,
                        AllocatedAmount = request.RequestedAmount,
                        Description = $"Budget request approved: {request.Reason}"
                    };
                    _context.BudgetLines.Add(newLine);
                    await _context.SaveChangesAsync(cancellationToken);
                    request.BudgetLineId = newLine.Id;
                }
            }
        }
        else
        {
            request.Status = BudgetRequestStatus.Declined;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return (await GetByIdAsync(id, cancellationToken))!;
    }

    private static BudgetRequestDto MapToDto(BudgetRequest br)
    {
        return new BudgetRequestDto
        {
            Id = br.Id,
            OrganizationId = br.OrganizationId,
            ProjectId = br.ProjectId,
            ProjectName = br.Project?.Name ?? string.Empty,
            ProjectManagerId = br.Project?.ProjectManagerId,
            ProjectManagerName = br.Project?.ProjectManager != null
                ? $"{br.Project.ProjectManager.FirstName} {br.Project.ProjectManager.LastName}"
                : null,
            BudgetLineId = br.BudgetLineId,
            Category = br.Category,
            CurrentBudget = br.CurrentBudget,
            RequestedAmount = br.RequestedAmount,
            Reason = br.Reason,
            Status = br.Status,
            ReviewedBy = br.ReviewedBy,
            ReviewedByName = br.ReviewedByUser != null
                ? $"{br.ReviewedByUser.FirstName} {br.ReviewedByUser.LastName}"
                : null,
            ReviewedAt = br.ReviewedAt,
            ReviewComment = br.ReviewComment,
            CreatedAt = br.CreatedAt
        };
    }
}
