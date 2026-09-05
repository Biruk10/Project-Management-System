namespace ShakaOrganizationPlatform.Application.AuditLogs.DTOs;

public class AuditLogDto
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public DateTime OccurredAt { get; set; }
}

public class AuditLogFilterParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? EntityName { get; set; }
    public string? Action { get; set; }
    public int? UserId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
