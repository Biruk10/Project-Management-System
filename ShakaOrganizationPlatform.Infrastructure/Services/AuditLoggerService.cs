using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Domain.Entities;
using ShakaOrganizationPlatform.Infrastructure.Persistence;

namespace ShakaOrganizationPlatform.Infrastructure.Services;

public class AuditLoggerService : IAuditLogger
{
    private readonly AppDbContext _context;

    public AuditLoggerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(int organizationId, int? userId, string action, string entityName, string entityId, string? oldValues = null, string? newValues = null, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        var log = new AuditLog
        {
            OrganizationId = organizationId,
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            OccurredAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
