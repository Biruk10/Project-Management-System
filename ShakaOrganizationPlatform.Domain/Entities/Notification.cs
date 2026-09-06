using ShakaOrganizationPlatform.Domain.Common;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class Notification : AuditableEntity
{
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? Link { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    public Organization Organization { get; set; } = null!;
    public User User { get; set; } = null!;
}
