using ShakaOrganizationPlatform.Application.Common.Models;
using ShakaOrganizationPlatform.Application.Users.DTOs;

namespace ShakaOrganizationPlatform.Application.Users.Services;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllAsync(UserFilterParams filters, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<UserDto> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default);
    Task ActivateAsync(int id, CancellationToken cancellationToken = default);
    Task DeactivateAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task AssignRolesAsync(int id, AssignRolesDto dto, CancellationToken cancellationToken = default);
}
