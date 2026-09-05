using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Application.Budgets.DTOs;

public class BudgetDto
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public BudgetStatus Status { get; set; }
    public int? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal RemainingBudget { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<BudgetLineDto> BudgetLines { get; set; } = new();
}

public class BudgetLineDto
{
    public int Id { get; set; }
    public int BudgetId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
}

public class CreateBudgetDto
{
    public int ProjectId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CreateBudgetLineDto> BudgetLines { get; set; } = new();
}

public class CreateBudgetLineDto
{
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal AllocatedAmount { get; set; }
}

public class UpdateBudgetDto
{
    public decimal TotalAmount { get; set; }
}

public class ApproveBudgetDto
{
    public bool Approve { get; set; }
}

public class AddBudgetLineDto
{
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal AllocatedAmount { get; set; }
}
