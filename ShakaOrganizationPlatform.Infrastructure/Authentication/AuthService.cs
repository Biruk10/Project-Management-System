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
        // 1. Check if user email already exists
        var existingUser = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.AdminEmail.ToLower(), cancellationToken);

        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email '{dto.AdminEmail}' already exists.");
        }

        // 2. Create Organization
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

        // 3. Seed default roles for this Organization
        var adminRole = new Role
        {
            OrganizationId = organization.Id,
            Name = "OrganizationAdmin",
            Description = "Organization Administrator with full access",
            IsSystemRole = true
        };

        var pmRole = new Role
        {
            OrganizationId = organization.Id,
            Name = "ProjectManager",
            Description = "Manages assigned projects, members, and tasks",
            IsSystemRole = true
        };

        var financeRole = new Role
        {
            OrganizationId = organization.Id,
            Name = "FinanceOfficer",
            Description = "Manages budgets, budget approvals, and expenses",
            IsSystemRole = true
        };

        var employeeRole = new Role
        {
            OrganizationId = organization.Id,
            Name = "Employee",
            Description = "Standard employee operating assigned tasks and projects",
            IsSystemRole = true
        };

        _context.Roles.AddRange(adminRole, pmRole, financeRole, employeeRole);
        await _context.SaveChangesAsync(cancellationToken);

        // 4. Create Administrator User
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

        // 5. Assign OrganizationAdmin Role
        var userRole = new UserRole
        {
            OrganizationId = organization.Id,
            UserId = adminUser.Id,
            RoleId = adminRole.Id,
            AssignedAt = DateTime.UtcNow
        };

        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync(cancellationToken);

        // 6. Generate Tokens & Response
        var rolesList = new List<string> { adminRole.Name };
        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(adminUser, organization, rolesList);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserProfileDto
            {
                Id = adminUser.Id,
                OrganizationId = organization.Id,
                FirstName = adminUser.FirstName,
                LastName = adminUser.LastName,
                Email = adminUser.Email,
                Roles = rolesList
            },
            Organization = new OrganizationSummaryDto
            {
                Id = organization.Id,
                Name = organization.Name,
                Email = organization.Email
            }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Organization)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
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

        // Reset lockout counters on success
        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var (accessToken, expiresAt) = _jwtTokenGenerator.GenerateAccessToken(user, user.Organization, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

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
                Id = user.Organization.Id,
                Name = user.Organization.Name,
                Email = user.Organization.Email
            }
        };
    }
}

