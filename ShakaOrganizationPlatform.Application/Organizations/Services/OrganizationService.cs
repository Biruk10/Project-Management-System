using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Organizations.DTOs;
using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Application.Organizations.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IAppDbContext _context;

    public OrganizationService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizationDto> CreateAsync(CreateOrganizationDto dto, CancellationToken cancellationToken = default)
    {
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

        return MapToDto(organization);
    }

    public async Task<OrganizationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var organization = await _context.Organizations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        return organization == null ? null : MapToDto(organization);
    }

    public async Task<List<OrganizationDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Organizations
            .AsNoTracking()
            .Select(o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                LogoUrl = o.LogoUrl,
                Address = o.Address,
                Phone = o.Phone,
                Email = o.Email,
                TimeZone = o.TimeZone,
                IsActive = o.IsActive,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<OrganizationDto?> UpdateAsync(int id, UpdateOrganizationDto dto, CancellationToken cancellationToken = default)
    {
        var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (organization == null)
        {
            return null;
        }

        organization.Name = dto.Name;
        organization.LogoUrl = dto.LogoUrl;
        organization.Address = dto.Address;
        organization.Phone = dto.Phone;
        organization.Email = dto.Email;
        organization.TimeZone = string.IsNullOrWhiteSpace(dto.TimeZone) ? "UTC" : dto.TimeZone;
        organization.IsActive = dto.IsActive;
        organization.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(organization);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (organization == null)
        {
            return false;
        }

        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static OrganizationDto MapToDto(Organization o)
    {
        return new OrganizationDto
        {
            Id = o.Id,
            Name = o.Name,
            LogoUrl = o.LogoUrl,
            Address = o.Address,
            Phone = o.Phone,
            Email = o.Email,
            TimeZone = o.TimeZone,
            IsActive = o.IsActive,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        };
    }
}

