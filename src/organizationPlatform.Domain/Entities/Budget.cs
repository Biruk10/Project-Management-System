namespace organizationPlatform.Domain.Entities;
public class Budget
{
    public int Id {get; set;}
    public required string OrganizId{get; set;}
    public required string ProjectId{get; set;}
    public decimal TotalAmount{get; set;}
    public BudgetStatus Status{get; set;}
    public string? ApprovedBy{get; set;}
    public DateTime? ApprovedAt {get; set;}= null;

}