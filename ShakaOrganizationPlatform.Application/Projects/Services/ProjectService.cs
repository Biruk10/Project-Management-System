using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Common.Models;
using ShakaOrganizationPlatform.Application.Projects.DTOs;
using ShakaOrganizationPlatform.Domain.Entities;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Application.Projects.Services;

public class ProjectService : IProjectService
{
    private readonly IAppDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ICurrentUserService _currentUserService;

    public ProjectService(IAppDbContext context, ITenantService tenantService, ICurrentUserService currentUserService)
    {
        _context = context;
        _tenantService = tenantService;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<ProjectDto>> GetAllAsync(ProjectFilterParams filters, CancellationToken cancellationToken = default)
    {
        var query = _context.Projects
            .Include(p => p.ProjectManager)
            .Include(p => p.Members).ThenInclude(m => m.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(search) || (p.Description != null && p.Description.ToLower().Contains(search)));
        }

        if (filters.Status.HasValue)
            query = query.Where(p => p.Status == filters.Status.Value);

        if (filters.ProjectManagerId.HasValue)
            query = query.Where(p => p.ProjectManagerId == filters.ProjectManagerId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var projects = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync(cancellationToken);

        var items = projects.Select(MapToDto).ToList();
        return PagedResult<ProjectDto>.Create(items, totalCount, filters.Page, filters.PageSize);
    }

    public async Task<ProjectDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects
            .Include(p => p.ProjectManager)
            .Include(p => p.Members).ThenInclude(m => m.User)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return project == null ? null : MapToDto(project);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var project = new Project
        {
            OrganizationId = orgId,
            Name = dto.Name,
            Description = dto.Description,
            StartDate = EnsureUtc(dto.StartDate),
            EndDate = EnsureUtc(dto.EndDate),
            Status = ProjectStatus.NotStarted,
            ProgressPercentage = 0,
            ProjectManagerId = dto.ProjectManagerId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _currentUserService.UserId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);

        if (dto.ProjectManagerId.HasValue)
        {
            var memberExists = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == project.Id && pm.UserId == dto.ProjectManagerId.Value, cancellationToken);

            if (!memberExists)
            {
                _context.ProjectMembers.Add(new ProjectMember
                {
                    OrganizationId = orgId,
                    ProjectId = project.Id,
                    UserId = dto.ProjectManagerId.Value,
                    ProjectRole = "ProjectManager",
                    JoinedAt = DateTime.UtcNow
                });

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        if (dto.InitialBudget.HasValue && dto.InitialBudget.Value > 0)
        {
            var budget = new Budget
            {
                OrganizationId = orgId,
                ProjectId = project.Id,
                TotalAmount = dto.InitialBudget.Value,
                Status = BudgetStatus.Approved,
                ApprovedBy = _currentUserService.UserId,
                ApprovedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = _currentUserService.UserId
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync(cancellationToken);

            var category = string.IsNullOrWhiteSpace(dto.BudgetCategory) ? "General" : dto.BudgetCategory.Trim();
            _context.BudgetLines.Add(new BudgetLine
            {
                BudgetId = budget.Id,
                Category = category,
                Description = "Initial project budget allocation",
                AllocatedAmount = dto.InitialBudget.Value
            });

            await _context.SaveChangesAsync(cancellationToken);
        }

        return (await GetByIdAsync(project.Id, cancellationToken))!;
    }

    public async Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Project {id} not found.");

        project.Name = dto.Name;
        project.Description = dto.Description;
        project.StartDate = EnsureUtc(dto.StartDate);
        project.EndDate = EnsureUtc(dto.EndDate);
        project.Status = dto.Status;
        project.ProgressPercentage = dto.ProgressPercentage;
        project.ProjectManagerId = dto.ProjectManagerId;
        project.UpdatedAt = DateTime.UtcNow;
        project.UpdatedByUserId = _currentUserService.UserId;

        if (dto.ProjectManagerId.HasValue)
        {
            var memberExists = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == project.Id && pm.UserId == dto.ProjectManagerId.Value, cancellationToken);

            if (!memberExists)
            {
                var orgId = _tenantService.OrganizationId ?? project.OrganizationId;
                _context.ProjectMembers.Add(new ProjectMember
                {
                    OrganizationId = orgId,
                    ProjectId = project.Id,
                    UserId = dto.ProjectManagerId.Value,
                    ProjectRole = "ProjectManager",
                    JoinedAt = DateTime.UtcNow
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Project {id} not found.");

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProjectDto> AddMemberAsync(int projectId, AddProjectMemberDto dto, CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken)
            ?? throw new KeyNotFoundException($"Project {projectId} not found.");

        int? matchedUserId = dto.UserId;
        string firstName = dto.FirstName?.Trim() ?? string.Empty;
        string lastName = dto.LastName?.Trim() ?? string.Empty;
        string email = dto.Email?.Trim() ?? string.Empty;
        string? phone = dto.Phone?.Trim();

        if (dto.UserId.HasValue)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId.Value, cancellationToken);
            if (user != null)
            {
                if (string.IsNullOrEmpty(firstName)) firstName = user.FirstName;
                if (string.IsNullOrEmpty(lastName)) lastName = user.LastName;
                if (string.IsNullOrEmpty(email)) email = user.Email;
            }
        }
        else if (!string.IsNullOrEmpty(email))
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
            if (user != null)
            {
                matchedUserId = user.Id;
                if (string.IsNullOrEmpty(firstName)) firstName = user.FirstName;
                if (string.IsNullOrEmpty(lastName)) lastName = user.LastName;
            }
        }

        var alreadyMember = await _context.ProjectMembers.AnyAsync(pm =>
            pm.ProjectId == projectId && (
                (matchedUserId.HasValue && pm.UserId == matchedUserId) ||
                (!string.IsNullOrEmpty(email) && pm.Email.ToLower() == email.ToLower())
            ), cancellationToken);

        if (alreadyMember)
            throw new InvalidOperationException("Person is already a member of this project.");

        _context.ProjectMembers.Add(new ProjectMember
        {
            OrganizationId = orgId,
            ProjectId = projectId,
            UserId = matchedUserId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            ProjectRole = string.IsNullOrWhiteSpace(dto.ProjectRole) ? "Member" : dto.ProjectRole.Trim(),
            JoinedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(projectId, cancellationToken))!;
    }

    public async Task RemoveMemberAsync(int projectId, int memberId, CancellationToken cancellationToken = default)
    {
        var member = await _context.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && (pm.Id == memberId || pm.UserId == memberId), cancellationToken)
            ?? throw new KeyNotFoundException("Project member not found.");

        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static ProjectDto MapToDto(Project project)
    {
        return new ProjectDto
        {
            Id = project.Id,
            OrganizationId = project.OrganizationId,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Status = project.Status,
            ProgressPercentage = project.ProgressPercentage,
            ProjectManagerId = project.ProjectManagerId,
            ProjectManagerName = project.ProjectManager != null
                ? $"{project.ProjectManager.FirstName} {project.ProjectManager.LastName}"
                : null,
            CreatedAt = project.CreatedAt,
            Members = project.Members.Select(m => new ProjectMemberDto
            {
                Id = m.Id,
                UserId = m.UserId,
                FirstName = !string.IsNullOrEmpty(m.FirstName) ? m.FirstName : (m.User != null ? m.User.FirstName : string.Empty),
                LastName = !string.IsNullOrEmpty(m.LastName) ? m.LastName : (m.User != null ? m.User.LastName : string.Empty),
                FullName = (!string.IsNullOrEmpty(m.FirstName) || !string.IsNullOrEmpty(m.LastName))
                    ? $"{m.FirstName} {m.LastName}".Trim()
                    : (m.User != null ? $"{m.User.FirstName} {m.User.LastName}" : string.Empty),
                Email = !string.IsNullOrEmpty(m.Email) ? m.Email : (m.User != null ? m.User.Email : string.Empty),
                Phone = m.Phone,
                ProjectRole = m.ProjectRole,
                JoinedAt = m.JoinedAt
            }).ToList()
        };
    }

    private static DateTime EnsureUtc(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Utc) return dt;
        return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }

    private static DateTime? EnsureUtc(DateTime? dt)
    {
        if (!dt.HasValue) return null;
        if (dt.Value.Kind == DateTimeKind.Utc) return dt.Value;
        return DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
    }
}
