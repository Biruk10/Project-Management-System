using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.AuditLogs.DTOs;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Common.Models;

namespace ShakaOrganizationPlatform.Application.AuditLogs.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAppDbContext _context;

    public AuditLogService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AuditLogDto>> GetAllAsync(AuditLogFilterParams filters, CancellationToken cancellationToken = default)
    {
        var query = _context.AuditLogs
            .Include(a => a.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.EntityName))
            query = query.Where(a => a.EntityName == filters.EntityName);

        if (!string.IsNullOrWhiteSpace(filters.Action))
            query = query.Where(a => a.Action.Contains(filters.Action));

        if (filters.UserId.HasValue)
            query = query.Where(a => a.UserId == filters.UserId.Value);

        if (filters.From.HasValue)
            query = query.Where(a => a.OccurredAt >= filters.From.Value);

        if (filters.To.HasValue)
            query = query.Where(a => a.OccurredAt <= filters.To.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var logs = await query
            .OrderByDescending(a => a.OccurredAt)
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync(cancellationToken);

        var items = logs.Select(a => new AuditLogDto
        {
            Id = a.Id,
            OrganizationId = a.OrganizationId,
            UserId = a.UserId,
            UserName = a.User != null ? $"{a.User.FirstName} {a.User.LastName}" : null,
            Action = a.Action,
            EntityName = a.EntityName,
            EntityId = a.EntityId,
            OldValues = a.OldValues,
            NewValues = a.NewValues,
            IpAddress = a.IpAddress,
            OccurredAt = a.OccurredAt
        }).ToList();

        return PagedResult<AuditLogDto>.Create(items, totalCount, filters.Page, filters.PageSize);
    }
}
