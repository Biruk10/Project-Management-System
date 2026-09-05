namespace ShakaOrganizationPlatform.Application.Dashboard.DTOs;

public class DashboardSummaryDto
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int TotalTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int UpcomingTasks { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal RemainingBudget { get; set; }
    public decimal BudgetUtilizationPercentage { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public List<ProjectProgressDto> ProjectProgress { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class ProjectProgressDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
    public decimal BudgetUtilization { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
}

public class RecentActivityDto
{
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public DateTime OccurredAt { get; set; }
}
