namespace organizationPlatform.Domain.Entities;
public class BudgetLine{
    public int Id {get;set;}
    
public int BudgetId { get; set; }

    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal AllocatedAmount { get; set; }
    
}