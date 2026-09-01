using ShakaOrganizationPlatform.Domain.Common;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class Expense : AuditableEntity
{
    public int ProjectId { get; set; }
    public int? BudgetLineId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
    public int? RecordedByUserId { get; set; }

    // Navigation Properties
    public Organization Organization { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public BudgetLine? BudgetLine { get; set; }
    public User? RecordedByUser { get; set; }
}
