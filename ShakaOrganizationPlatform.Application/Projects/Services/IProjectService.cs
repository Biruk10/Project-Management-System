using ShakaOrganizationPlatform.Application.Common.Models;
using ShakaOrganizationPlatform.Application.Projects.DTOs;

namespace ShakaOrganizationPlatform.Application.Projects.Services;

public interface IProjectService
{
    Task<PagedResult<ProjectDto>> GetAllAsync(ProjectFilterParams filters, CancellationToken cancellationToken = default);
    Task<ProjectDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken cancellationToken = default);
    Task<ProjectDto> UpdateAsync(int id, UpdateProjectDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<ProjectDto> AddMemberAsync(int projectId, AddProjectMemberDto dto, CancellationToken cancellationToken = default);
    Task RemoveMemberAsync(int projectId, int memberId, CancellationToken cancellationToken = default);
}
