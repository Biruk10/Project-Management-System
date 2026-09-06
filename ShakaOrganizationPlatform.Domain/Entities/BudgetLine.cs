using ShakaOrganizationPlatform.Domain.Common;

namespace ShakaOrganizationPlatform.Domain.Entities;

public class BudgetLine : BaseEntity
{
    public int BudgetId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal AllocatedAmount { get; set; }

    public Budget Budget { get; set; } = null!;
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
