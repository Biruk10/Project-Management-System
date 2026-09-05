using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Roles.DTOs;
using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Application.Roles.Services;

public class RoleService : IRoleService
{
    private readonly IAppDbContext _context;
    private readonly ITenantService _tenantService;

    public RoleService(IAppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _context.Roles
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        return roles.Select(MapToDto).ToList();
    }

    public async Task<RoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return role == null ? null : MapToDto(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var exists = await _context.Roles.AnyAsync(r => r.Name == dto.Name, cancellationToken);
        if (exists)
            throw new InvalidOperationException($"A role named '{dto.Name}' already exists.");

        var role = new Role
        {
            OrganizationId = orgId,
            Name = dto.Name,
            Description = dto.Description,
            IsSystemRole = false
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);

        await AssignPermissionsAsync(role.Id, dto.PermissionIds, cancellationToken);

        return (await GetByIdAsync(role.Id, cancellationToken))!;
    }

    public async Task<RoleDto> UpdateAsync(int id, UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Role {id} not found.");

        if (role.IsSystemRole)
            throw new InvalidOperationException("System roles cannot be renamed.");

        var nameTaken = await _context.Roles.AnyAsync(r => r.Name == dto.Name && r.Id != id, cancellationToken);
        if (nameTaken)
            throw new InvalidOperationException($"A role named '{dto.Name}' already exists.");

        role.Name = dto.Name;
        role.Description = dto.Description;

        foreach (var rp in role.RolePermissions.ToList())
            _context.RolePermissions.Remove(rp);

        await _context.SaveChangesAsync(cancellationToken);
        await AssignPermissionsAsync(id, dto.PermissionIds, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Role {id} not found.");

        if (role.IsSystemRole)
            throw new InvalidOperationException("System roles cannot be deleted.");

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task AssignPermissionsAsync(int roleId, List<int> permissionIds, CancellationToken cancellationToken)
    {
        foreach (var permId in permissionIds.Distinct())
        {
            var permExists = await _context.Permissions.AnyAsync(p => p.Id == permId, cancellationToken);
            if (permExists)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permId
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            Permissions = role.RolePermissions.Select(rp => rp.Permission.Key).ToList()
        };
    }
}
