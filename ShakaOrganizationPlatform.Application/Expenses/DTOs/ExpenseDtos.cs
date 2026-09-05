namespace ShakaOrganizationPlatform.Application.Expenses.DTOs;

public class ExpenseDto
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int? BudgetLineId { get; set; }
    public string? BudgetLineCategory { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; }
    public int? RecordedByUserId { get; set; }
    public string? RecordedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateExpenseDto
{
    public int ProjectId { get; set; }
    public int? BudgetLineId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
}

public class UpdateExpenseDto
{
    public int? BudgetLineId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; }
}

public class ExpenseFilterParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int? ProjectId { get; set; }
    public int? BudgetLineId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
