namespace ShakaOrganizationPlatform.Application.Common.Interfaces;

public interface IAuditLogger
{
    Task LogAsync(int organizationId, int? userId, string action, string entityName, string entityId, string? oldValues = null, string? newValues = null, string? ipAddress = null, CancellationToken cancellationToken = default);
}
