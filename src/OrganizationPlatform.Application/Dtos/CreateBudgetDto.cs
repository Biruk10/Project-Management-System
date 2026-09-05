namespace organizationPlatform.Application.Dtos;
public class CreateBudgetDto
{
   
    public required string OrganizId { get; set; }
    public required string ProjectId { get; set; }
    public int TotalAmount { get; set; }
    public BudgetStatus Status {get; set;}
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt  { get; set; }
}