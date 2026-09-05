using ShakaOrganizationPlatform.Application.Organizations.DTOs;

namespace ShakaOrganizationPlatform.Application.Organizations.Services;

public interface IOrganizationService
{
    Task<OrganizationDto?> GetCurrentAsync(CancellationToken cancellationToken = default);
    Task<OrganizationDto> UpdateAsync(UpdateOrganizationDto dto, CancellationToken cancellationToken = default);
    Task DeactivateAsync(CancellationToken cancellationToken = default);
}
