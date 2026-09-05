using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Organizations.DTOs;

namespace ShakaOrganizationPlatform.Application.Organizations.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IAppDbContext _context;
    private readonly ITenantService _tenantService;

    public OrganizationService(IAppDbContext context, ITenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<OrganizationDto?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId;
        if (!orgId.HasValue) return null;

        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == orgId.Value, cancellationToken);

        if (org == null) return null;

        return new OrganizationDto
        {
            Id = org.Id,
            Name = org.Name,
            LogoUrl = org.LogoUrl,
            Address = org.Address,
            Phone = org.Phone,
            Email = org.Email,
            TimeZone = org.TimeZone,
            IsActive = org.IsActive,
            CreatedAt = org.CreatedAt
        };
    }

    public async Task<OrganizationDto> UpdateAsync(UpdateOrganizationDto dto, CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == orgId, cancellationToken)
            ?? throw new KeyNotFoundException("Organization not found.");

        org.Name = dto.Name;
        org.LogoUrl = dto.LogoUrl;
        org.Address = dto.Address;
        org.Phone = dto.Phone;
        org.Email = dto.Email;
        org.TimeZone = string.IsNullOrWhiteSpace(dto.TimeZone) ? "UTC" : dto.TimeZone;
        org.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new OrganizationDto
        {
            Id = org.Id,
            Name = org.Name,
            LogoUrl = org.LogoUrl,
            Address = org.Address,
            Phone = org.Phone,
            Email = org.Email,
            TimeZone = org.TimeZone,
            IsActive = org.IsActive,
            CreatedAt = org.CreatedAt
        };
    }

    public async Task DeactivateAsync(CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var org = await _context.Organizations
            .FirstOrDefaultAsync(o => o.Id == orgId, cancellationToken)
            ?? throw new KeyNotFoundException("Organization not found.");

        org.IsActive = false;
        org.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
