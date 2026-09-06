using ShakaOrganizationPlatform.Domain.Common;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class Budget : AuditableEntity
{
    public int ProjectId { get; set; }
    public decimal TotalAmount { get; set; }
    public BudgetStatus Status { get; set; } = BudgetStatus.Draft;
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public Organization Organization { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public User? ApprovedByUser { get; set; }
    public ICollection<BudgetLine> BudgetLines { get; set; } = new List<BudgetLine>();
}
