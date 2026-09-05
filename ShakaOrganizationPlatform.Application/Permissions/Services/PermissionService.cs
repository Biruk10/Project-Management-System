using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Permissions.DTOs;

namespace ShakaOrganizationPlatform.Application.Permissions.Services;

public class PermissionService : IPermissionService
{
    private readonly IAppDbContext _context;

    public PermissionService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _context.Permissions
            .OrderBy(p => p.Key)
            .ToListAsync(cancellationToken);

        return permissions.Select(p => new PermissionDto
        {
            Id = p.Id,
            Key = p.Key,
            Description = p.Description
        }).ToList();
    }

    public async Task<List<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var permissions = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Key)
            .Distinct()
            .ToListAsync(cancellationToken);

        return permissions;
    }

    public async Task<bool> UserHasPermissionAsync(int userId, string permissionKey, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp => rp.Permission.Key == permissionKey, cancellationToken);
    }
}
