using ShakaOrganizationPlatform.Application.Permissions.DTOs;

namespace ShakaOrganizationPlatform.Application.Permissions.Services;

public interface IPermissionService
{
    Task<List<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> UserHasPermissionAsync(int userId, string permissionKey, CancellationToken cancellationToken = default);
}
