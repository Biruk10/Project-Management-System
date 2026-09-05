using ShakaOrganizationPlatform.Application.Dashboard.DTOs;

namespace ShakaOrganizationPlatform.Application.Dashboard.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
