using ShakaOrganizationPlatform.Application.AuditLogs.DTOs;
using ShakaOrganizationPlatform.Application.Common.Models;

namespace ShakaOrganizationPlatform.Application.AuditLogs.Services;

public interface IAuditLogService
{
    Task<PagedResult<AuditLogDto>> GetAllAsync(AuditLogFilterParams filters, CancellationToken cancellationToken = default);
}
