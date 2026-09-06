using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Application.Budgets.DTOs;

public class BudgetRequestDto
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int? ProjectManagerId { get; set; }
    public string? ProjectManagerName { get; set; }
    public int? BudgetLineId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal CurrentBudget { get; set; }
    public decimal RequestedAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public BudgetRequestStatus Status { get; set; }
    public int? ReviewedBy { get; set; }
    public string? ReviewedByName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewComment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateBudgetRequestDto
{
    public int ProjectId { get; set; }
    public int? BudgetLineId { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ReviewBudgetRequestDto
{
    public bool Approve { get; set; }
    public string? ReviewComment { get; set; }
}
