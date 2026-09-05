using ShakaOrganizationPlatform.Domain.Enums;
using TaskStatus = ShakaOrganizationPlatform.Domain.Enums.TaskStatus;

namespace ShakaOrganizationPlatform.Application.Reports.DTOs;

public class ProjectReportDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ProjectManagerName { get; set; }
    public int TotalMembers { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal RemainingBudget { get; set; }
    public decimal BudgetUtilization { get; set; }
}

public class BudgetReportDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal TotalApprovedBudget { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal RemainingBudget { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public List<BudgetLineReportDto> Lines { get; set; } = new();
}

public class BudgetLineReportDto
{
    public string Category { get; set; } = string.Empty;
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal Remaining { get; set; }
}

public class ExpenseReportDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ExpenseReportItemDto> Expenses { get; set; } = new();
}

public class ExpenseReportItemDto
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? BudgetLineCategory { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; }
    public string? RecordedBy { get; set; }
}

public class TaskReportDto
{
    public int TotalTasks { get; set; }
    public int TodoTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public List<TaskReportItemDto> Tasks { get; set; } = new();
}

public class TaskReportItemDto
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string? AssignedTo { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal CompletionPercentage { get; set; }
}

public class ReportFilterParams
{
    public int? ProjectId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
