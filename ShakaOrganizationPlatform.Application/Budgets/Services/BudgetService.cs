using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Budgets.DTOs;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Domain.Entities;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Application.Budgets.Services;

public class BudgetService : IBudgetService
{
    private readonly IAppDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ICurrentUserService _currentUserService;

    public BudgetService(IAppDbContext context, ITenantService tenantService, ICurrentUserService currentUserService)
    {
        _context = context;
        _tenantService = tenantService;
        _currentUserService = currentUserService;
    }

    public async Task<List<BudgetDto>> GetByProjectAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var budgets = await _context.Budgets
            .Include(b => b.Project)
            .Include(b => b.ApprovedByUser)
            .Include(b => b.BudgetLines).ThenInclude(bl => bl.Expenses)
            .Where(b => b.ProjectId == projectId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken);

        return budgets.Select(MapToDto).ToList();
    }

    public async Task<BudgetDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var budget = await _context.Budgets
            .Include(b => b.Project)
            .Include(b => b.ApprovedByUser)
            .Include(b => b.BudgetLines).ThenInclude(bl => bl.Expenses)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        return budget == null ? null : MapToDto(budget);
    }

    public async Task<BudgetDto> CreateAsync(CreateBudgetDto dto, CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var projectExists = await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId, cancellationToken);
        if (!projectExists)
            throw new KeyNotFoundException($"Project {dto.ProjectId} not found.");

        var budget = new Budget
        {
            OrganizationId = orgId,
            ProjectId = dto.ProjectId,
            TotalAmount = dto.TotalAmount,
            Status = BudgetStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _currentUserService.UserId
        };

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var lineDto in dto.BudgetLines)
        {
            _context.BudgetLines.Add(new BudgetLine
            {
                BudgetId = budget.Id,
                Category = lineDto.Category,
                Description = lineDto.Description,
                AllocatedAmount = lineDto.AllocatedAmount
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(budget.Id, cancellationToken))!;
    }

    public async Task<BudgetDto> UpdateAsync(int id, UpdateBudgetDto dto, CancellationToken cancellationToken = default)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Budget {id} not found.");

        if (budget.Status != BudgetStatus.Draft)
            throw new InvalidOperationException("Only draft budgets can be updated.");

        budget.TotalAmount = dto.TotalAmount;
        budget.UpdatedAt = DateTime.UtcNow;
        budget.UpdatedByUserId = _currentUserService.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<BudgetDto> ApproveAsync(int id, CancellationToken cancellationToken = default)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Budget {id} not found.");

        budget.Status = BudgetStatus.Approved;
        budget.ApprovedBy = _currentUserService.UserId;
        budget.ApprovedAt = DateTime.UtcNow;
        budget.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<BudgetDto> RejectAsync(int id, CancellationToken cancellationToken = default)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Budget {id} not found.");

        budget.Status = BudgetStatus.Rejected;
        budget.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Budget {id} not found.");

        if (budget.Status == BudgetStatus.Approved)
            throw new InvalidOperationException("Approved budgets cannot be deleted.");

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<BudgetDto> AddBudgetLineAsync(int budgetId, AddBudgetLineDto dto, CancellationToken cancellationToken = default)
    {
        var budget = await _context.Budgets
            .FirstOrDefaultAsync(b => b.Id == budgetId, cancellationToken)
            ?? throw new KeyNotFoundException($"Budget {budgetId} not found.");

        if (budget.Status == BudgetStatus.Approved)
            throw new InvalidOperationException("Cannot add lines to an approved budget.");

        _context.BudgetLines.Add(new BudgetLine
        {
            BudgetId = budgetId,
            Category = dto.Category,
            Description = dto.Description,
            AllocatedAmount = dto.AllocatedAmount
        });

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(budgetId, cancellationToken))!;
    }

    public async Task RemoveBudgetLineAsync(int budgetId, int lineId, CancellationToken cancellationToken = default)
    {
        var line = await _context.BudgetLines
            .FirstOrDefaultAsync(bl => bl.Id == lineId && bl.BudgetId == budgetId, cancellationToken)
            ?? throw new KeyNotFoundException($"Budget line {lineId} not found.");

        _context.BudgetLines.Remove(line);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static BudgetDto MapToDto(Budget budget)
    {
        var totalExpenses = budget.BudgetLines.Sum(bl => bl.Expenses.Sum(e => e.Amount));
        var remaining = budget.TotalAmount - totalExpenses;
        var utilization = budget.TotalAmount > 0
            ? Math.Round((totalExpenses / budget.TotalAmount) * 100, 2)
            : 0;

        return new BudgetDto
        {
            Id = budget.Id,
            OrganizationId = budget.OrganizationId,
            ProjectId = budget.ProjectId,
            ProjectName = budget.Project?.Name ?? string.Empty,
            TotalAmount = budget.TotalAmount,
            Status = budget.Status,
            ApprovedBy = budget.ApprovedBy,
            ApprovedByName = budget.ApprovedByUser != null
                ? $"{budget.ApprovedByUser.FirstName} {budget.ApprovedByUser.LastName}"
                : null,
            ApprovedAt = budget.ApprovedAt,
            TotalExpenses = totalExpenses,
            RemainingBudget = remaining,
            UtilizationPercentage = utilization,
            CreatedAt = budget.CreatedAt,
            BudgetLines = budget.BudgetLines.Select(bl => new BudgetLineDto
            {
                Id = bl.Id,
                BudgetId = bl.BudgetId,
                Category = bl.Category,
                Description = bl.Description,
                AllocatedAmount = bl.AllocatedAmount,
                SpentAmount = bl.Expenses.Sum(e => e.Amount)
            }).ToList()
        };
    }
}
