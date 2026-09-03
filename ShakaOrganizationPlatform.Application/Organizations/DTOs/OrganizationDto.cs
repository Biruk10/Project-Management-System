namespace ShakaOrganizationPlatform.Application.Organizations.DTOs;

public class OrganizationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "UTC";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateOrganizationDto
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "UTC";
}

public class UpdateOrganizationDto
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string Email { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "UTC";
    public bool IsActive { get; set; } = true;
}

