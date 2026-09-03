using ShakaOrganizationPlatform.Application.Organizations.DTOs;

namespace ShakaOrganizationPlatform.Application.Organizations.Services;

public interface IOrganizationService
{
    Task<OrganizationDto> CreateAsync(CreateOrganizationDto dto, CancellationToken cancellationToken = default);
    Task<OrganizationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<OrganizationDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrganizationDto?> UpdateAsync(int id, UpdateOrganizationDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

