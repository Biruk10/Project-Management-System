using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Common.Models;
using ShakaOrganizationPlatform.Application.Tasks.DTOs;
using ShakaOrganizationPlatform.Domain.Entities;
using ShakaOrganizationPlatform.Domain.Enums;
using TaskStatus = ShakaOrganizationPlatform.Domain.Enums.TaskStatus;

namespace ShakaOrganizationPlatform.Application.Tasks.Services;

public class TaskService : ITaskService
{
    private readonly IAppDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ICurrentUserService _currentUserService;

    public TaskService(IAppDbContext context, ITenantService tenantService, ICurrentUserService currentUserService)
    {
        _context = context;
        _tenantService = tenantService;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<TaskDto>> GetAllAsync(TaskFilterParams filters, CancellationToken cancellationToken = default)
    {
        var query = _context.TaskItems
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(search) || (t.Description != null && t.Description.ToLower().Contains(search)));
        }

        if (filters.ProjectId.HasValue)
            query = query.Where(t => t.ProjectId == filters.ProjectId.Value);

        if (filters.AssignedToUserId.HasValue)
            query = query.Where(t => t.AssignedToUserId == filters.AssignedToUserId.Value);

        if (filters.Status.HasValue)
            query = query.Where(t => t.Status == filters.Status.Value);

        if (filters.Priority.HasValue)
            query = query.Where(t => t.Priority == filters.Priority.Value);

        if (filters.Overdue == true)
            query = query.Where(t => t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed);

        var totalCount = await query.CountAsync(cancellationToken);

        var tasks = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync(cancellationToken);

        var items = tasks.Select(MapToDto).ToList();
        return PagedResult<TaskDto>.Create(items, totalCount, filters.Page, filters.PageSize);
    }

    public async Task<TaskDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _context.TaskItems
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return task == null ? null : MapToDto(task);
    }

    public async Task<TaskDto> CreateAsync(CreateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var projectExists = await _context.Projects
            .AnyAsync(p => p.Id == dto.ProjectId, cancellationToken);

        if (!projectExists)
            throw new KeyNotFoundException($"Project {dto.ProjectId} not found.");

        var task = new TaskItem
        {
            OrganizationId = orgId,
            ProjectId = dto.ProjectId,
            Title = dto.Title,
            Description = dto.Description,
            AssignedToUserId = dto.AssignedToUserId,
            Priority = dto.Priority,
            Status = TaskStatus.Todo,
            DueDate = EnsureUtc(dto.DueDate),
            CompletionPercentage = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _currentUserService.UserId
        };

        _context.TaskItems.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(task.Id, cancellationToken))!;
    }

    public async Task<TaskDto> UpdateAsync(int id, UpdateTaskDto dto, CancellationToken cancellationToken = default)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Task {id} not found.");

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.AssignedToUserId = dto.AssignedToUserId;
        task.Priority = dto.Priority;
        task.Status = dto.Status;
        task.DueDate = EnsureUtc(dto.DueDate);
        task.CompletionPercentage = dto.CompletionPercentage;
        task.UpdatedAt = DateTime.UtcNow;
        task.UpdatedByUserId = _currentUserService.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var task = await _context.TaskItems
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Task {id} not found.");

        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static TaskDto MapToDto(TaskItem task)
    {
        return new TaskDto
        {
            Id = task.Id,
            OrganizationId = task.OrganizationId,
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.Name ?? string.Empty,
            Title = task.Title,
            Description = task.Description,
            AssignedToUserId = task.AssignedToUserId,
            AssignedToUserName = task.AssignedToUser != null
                ? $"{task.AssignedToUser.FirstName} {task.AssignedToUser.LastName}"
                : null,
            Priority = task.Priority,
            Status = task.Status,
            DueDate = task.DueDate,
            CompletionPercentage = task.CompletionPercentage,
            CreatedAt = task.CreatedAt
        };
    }

    private static DateTime? EnsureUtc(DateTime? dt)
    {
        if (!dt.HasValue) return null;
        if (dt.Value.Kind == DateTimeKind.Utc) return dt.Value;
        return DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
    }
}
