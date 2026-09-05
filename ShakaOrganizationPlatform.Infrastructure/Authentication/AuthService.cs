using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Auth.DTOs;
using ShakaOrganizationPlatform.Application.Auth.Services;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Domain.Entities;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Infrastructure.Authentication;

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IAppDbContext context,
        IPasswordHasherService passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterOrganizationAsync(RegisterOrganizationDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.AdminEmail.ToLower(), cancellationToken);

        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email '{dto.AdminEmail}' already exists.");
        }

        var organization = new Organization
        {
            Name = dto.Name,
            LogoUrl = dto.LogoUrl,
            Address = dto.Address,
            Phone = dto.Phone,
            Email = dto.Email,
            TimeZone = string.IsNullOrWhiteSpace(dto.TimeZone) ? "UTC" : dto.TimeZone,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);

        var adminRole = new Role { OrganizationId = organization.Id, Name = "OrganizationAdmin", Description = "Organization Administrator with full tenant access", IsSystemRole = true };
        var pmRole = new Role { OrganizationId = organization.Id, Name = "ProjectManager", Description = "Manages assigned projects, members, and tasks", IsSystemRole = true };
        var financeRole = new Role { OrganizationId = organization.Id, Name = "FinanceOfficer", Description = "Manages budgets, approvals, and expenses", IsSystemRole = true };
        var employeeRole = new Role { OrganizationId = organization.Id, Name = "Employee", Description = "Standard employee operating assigned tasks", IsSystemRole = true };

        _context.Roles.AddRange(adminRole, pmRole, financeRole, employeeRole);
        await _context.SaveChangesAsync(cancellationToken);

        await AssignOrgRolePermissionsAsync(adminRole.Id, pmRole.Id, financeRole.Id, employeeRole.Id, cancellationToken);

        var adminUser = new User
        {
            OrganizationId = organization.Id,
            FirstName = dto.AdminFirstName,
            LastName = dto.AdminLastName,
            Email = dto.AdminEmail.ToLower(),
            Status = UserStatus.Active,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        adminUser.PasswordHash = _passwordHasher.HashPassword(adminUser, dto.AdminPassword);
        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync(cancellationToken);

        _context.UserRoles.Add(new UserRole
        {
            OrganizationId = organization.Id,
            UserId = adminUser.Id,
            RoleId = adminRole.Id,
            AssignedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        var roles = new List<string> { adminRole.Name };
        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(adminUser, organization, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        return BuildAuthResponse(adminUser, organization, roles, accessToken, expiresAt, refreshToken);
    }

    public async Task<AuthResponseDto> RegisterSystemAdminAsync(RegisterSystemAdminDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower(), cancellationToken);

        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email '{dto.Email}' already exists.");
        }

        var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == dto.OrganizationId, cancellationToken);
        if (organization == null)
        {
            organization = await _context.Organizations.FirstOrDefaultAsync(cancellationToken);
            if (organization == null)
            {
                organization = new Organization
                {
                    Name = "System Platform Organization",
                    Email = "admin@system.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Organizations.Add(organization);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        var systemAdminRole = await _context.Roles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.OrganizationId == organization.Id && r.Name == "SystemAdmin", cancellationToken);

        if (systemAdminRole == null)
        {
            systemAdminRole = new Role
            {
                OrganizationId = organization.Id,
                Name = "SystemAdmin",
                Description = "System-wide Platform Administrator with global access",
                IsSystemRole = true
            };
            _context.Roles.Add(systemAdminRole);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var user = new User
        {
            OrganizationId = organization.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email.ToLower(),
            Status = UserStatus.Active,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _context.UserRoles.Add(new UserRole
        {
            OrganizationId = organization.Id,
            UserId = user.Id,
            RoleId = systemAdminRole.Id,
            AssignedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        var roles = new List<string> { systemAdminRole.Name };
        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user, organization, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        return BuildAuthResponse(user, organization, roles, accessToken, expiresAt, refreshToken);
    }

    public async Task<AuthResponseDto> GrantSystemAdminRoleAsync(GrantSystemAdminDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Organization)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower(), cancellationToken)
            ?? throw new KeyNotFoundException($"User with email '{dto.Email}' not found.");

        var systemAdminRole = await _context.Roles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.OrganizationId == user.OrganizationId && r.Name == "SystemAdmin", cancellationToken);

        if (systemAdminRole == null)
        {
            systemAdminRole = new Role
            {
                OrganizationId = user.OrganizationId,
                Name = "SystemAdmin",
                Description = "System-wide Platform Administrator with global access",
                IsSystemRole = true
            };
            _context.Roles.Add(systemAdminRole);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var existingUserRole = await _context.UserRoles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == systemAdminRole.Id, cancellationToken);

        if (existingUserRole == null)
        {
            _context.UserRoles.Add(new UserRole
            {
                OrganizationId = user.OrganizationId,
                UserId = user.Id,
                RoleId = systemAdminRole.Id,
                AssignedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync(cancellationToken);
        }

        var roles = await _context.UserRoles
            .IgnoreQueryFilters()
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user, user.Organization, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        return BuildAuthResponse(user, user.Organization, roles, accessToken, expiresAt, refreshToken);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Organization)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower(), cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!user.IsActive || user.Status != UserStatus.Active)
        {
            throw new UnauthorizedAccessException("Account is inactive or suspended.");
        }

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException($"Account is locked until {user.LockoutEnd.Value:g} UTC.");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(user, user.PasswordHash, dto.Password);
        if (!isPasswordValid)
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }
            await _context.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user, user.Organization, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        return BuildAuthResponse(user, user.Organization, roles, accessToken, expiresAt, refreshToken);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Refresh token store not yet implemented.");
    }

    public async Task LogoutAsync(int userId, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        if (!_passwordHasher.VerifyPassword(user, user.PasswordHash, dto.CurrentPassword))
        {
            throw new UnauthorizedAccessException("Current password is incorrect.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower(), cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task AssignOrgRolePermissionsAsync(int adminRoleId, int pmRoleId, int financeRoleId, int employeeRoleId, CancellationToken cancellationToken)
    {
        var allPermissions = await _context.Permissions
            .IgnoreQueryFilters()
            .ToDictionaryAsync(p => p.Key, p => p.Id, cancellationToken);

        var adminKeys = allPermissions.Keys
            .Where(k => !k.StartsWith("Platform."))
            .ToArray();

        string[] pmKeys =
        {
            "Project.Create", "Project.Read", "Project.Update",
            "Task.Create", "Task.Read", "Task.Update", "Task.Delete", "Task.Assign",
            "Budget.Read", "Expense.Read", "Report.View", "Notification.Read", "User.Read"
        };

        string[] financeKeys =
        {
            "Project.Read",
            "Budget.Create", "Budget.Read", "Budget.Approve",
            "Expense.Create", "Expense.Read",
            "Report.View", "Report.Export", "Notification.Read"
        };

        string[] employeeKeys =
        {
            "Project.Read", "Task.Read", "Task.Update", "Notification.Read"
        };

        var toAdd = new List<RolePermission>();

        foreach (var key in adminKeys)
            if (allPermissions.TryGetValue(key, out var id))
                toAdd.Add(new RolePermission { RoleId = adminRoleId, PermissionId = id });

        foreach (var key in pmKeys)
            if (allPermissions.TryGetValue(key, out var id))
                toAdd.Add(new RolePermission { RoleId = pmRoleId, PermissionId = id });

        foreach (var key in financeKeys)
            if (allPermissions.TryGetValue(key, out var id))
                toAdd.Add(new RolePermission { RoleId = financeRoleId, PermissionId = id });

        foreach (var key in employeeKeys)
            if (allPermissions.TryGetValue(key, out var id))
                toAdd.Add(new RolePermission { RoleId = employeeRoleId, PermissionId = id });

        _context.RolePermissions.AddRange(toAdd);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static AuthResponseDto BuildAuthResponse(User user, Organization org, List<string> roles, string accessToken, DateTime expiresAt, string refreshToken)
    {
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserProfileDto
            {
                Id = user.Id,
                OrganizationId = user.OrganizationId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = roles
            },
            Organization = new OrganizationSummaryDto
            {
                Id = org.Id,
                Name = org.Name,
                Email = org.Email
            }
        };
    }
}
