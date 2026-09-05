using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Auth.Services;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Common.Models;
using ShakaOrganizationPlatform.Application.Users.DTOs;
using ShakaOrganizationPlatform.Domain.Entities;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Application.Users.Services;

public class UserService : IUserService
{
    private readonly IAppDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasherService _passwordHasher;

    public UserService(
        IAppDbContext context,
        ITenantService tenantService,
        ICurrentUserService currentUserService,
        IPasswordHasherService passwordHasher)
    {
        _context = context;
        _tenantService = tenantService;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<PagedResult<UserDto>> GetAllAsync(UserFilterParams filters, CancellationToken cancellationToken = default)
    {
        var query = _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var search = filters.Search.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Email.ToLower().Contains(search));
        }

        if (filters.IsActive.HasValue)
            query = query.Where(u => u.IsActive == filters.IsActive.Value);

        if (filters.Status.HasValue)
            query = query.Where(u => u.Status == filters.Status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(u => u.FirstName)
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync(cancellationToken);

        var items = users.Select(MapToDto).ToList();
        return PagedResult<UserDto>.Create(items, totalCount, filters.Page, filters.PageSize);
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var exists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower(), cancellationToken);

        if (exists)
            throw new InvalidOperationException($"A user with email '{dto.Email}' already exists in this organization.");

        var user = new User
        {
            OrganizationId = orgId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email.ToLower(),
            IsActive = true,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _currentUserService.UserId
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var roleId in dto.RoleIds.Distinct())
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId, cancellationToken);
            if (roleExists)
            {
                _context.UserRoles.Add(new UserRole
                {
                    OrganizationId = orgId,
                    UserId = user.Id,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedByUserId = _currentUserService.UserId
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(user.Id, cancellationToken))!;
    }

    public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"User {id} not found.");

        var emailTaken = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.Id != id, cancellationToken);

        if (emailTaken)
            throw new InvalidOperationException($"Email '{dto.Email}' is already in use.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email.ToLower();
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedByUserId = _currentUserService.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task ActivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"User {id} not found.");

        user.IsActive = true;
        user.Status = UserStatus.Active;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"User {id} not found.");

        user.IsActive = false;
        user.Status = UserStatus.Inactive;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"User {id} not found.");

        var currentUserId = _currentUserService.UserId;
        if (currentUserId.HasValue && currentUserId.Value == id)
            throw new InvalidOperationException("You cannot delete your own account.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignRolesAsync(int id, AssignRolesDto dto, CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"User {id} not found.");

        var existingRoles = await _context.UserRoles
            .Where(ur => ur.UserId == id)
            .ToListAsync(cancellationToken);

        foreach (var existing in existingRoles)
            _context.UserRoles.Remove(existing);

        foreach (var roleId in dto.RoleIds.Distinct())
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId, cancellationToken);
            if (roleExists)
            {
                _context.UserRoles.Add(new UserRole
                {
                    OrganizationId = orgId,
                    UserId = id,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedByUserId = _currentUserService.UserId
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            OrganizationId = user.OrganizationId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            IsActive = user.IsActive,
            Status = user.Status,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}
