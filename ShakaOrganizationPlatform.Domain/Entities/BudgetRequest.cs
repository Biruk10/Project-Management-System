using ShakaOrganizationPlatform.Domain.Common;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class BudgetRequest : AuditableEntity
{
    public int ProjectId { get; set; }
    public int? BudgetLineId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal CurrentBudget { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public BudgetRequestStatus Status { get; set; } = BudgetRequestStatus.Pending;
    public int? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewComment { get; set; }

    // Navigation Properties
    public Organization Organization { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public BudgetLine? BudgetLine { get; set; }
    public User? ReviewedByUser { get; set; }
}
