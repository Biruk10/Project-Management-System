namespace ShakaOrganizationPlatform.Application.Permissions.DTOs;

public class PermissionDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
}
