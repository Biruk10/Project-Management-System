using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Dashboard.DTOs;
using ShakaOrganizationPlatform.Domain.Enums;
using TaskStatus = ShakaOrganizationPlatform.Domain.Enums.TaskStatus;

namespace ShakaOrganizationPlatform.Application.Dashboard.Services;

public class DashboardService : IDashboardService
{
    private readonly IAppDbContext _context;

    public DashboardService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var projects = await _context.Projects
            .Include(p => p.Tasks)
            .Include(p => p.Budgets).ThenInclude(b => b.BudgetLines).ThenInclude(bl => bl.Expenses)
            .ToListAsync(cancellationToken);

        var users = await _context.Users.ToListAsync(cancellationToken);

        var recentActivities = await _context.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.OccurredAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        var totalBudget = projects
            .SelectMany(p => p.Budgets.Where(b => b.Status == BudgetStatus.Approved))
            .Sum(b => b.TotalAmount);

        var totalExpenses = projects
            .SelectMany(p => p.Budgets)
            .SelectMany(b => b.BudgetLines)
            .Sum(bl => bl.Expenses.Sum(e => e.Amount));

        var upcomingTasks = await _context.TaskItems
            .CountAsync(t =>
                t.DueDate.HasValue &&
                t.DueDate.Value > now &&
                t.DueDate.Value <= now.AddDays(7) &&
                t.Status != TaskStatus.Completed, cancellationToken);

        var overdueTasks = await _context.TaskItems
            .CountAsync(t =>
                t.DueDate.HasValue &&
                t.DueDate.Value < now &&
                t.Status != TaskStatus.Completed, cancellationToken);

        var projectProgress = projects.Select(p =>
        {
            var approvedBudget = p.Budgets
                .Where(b => b.Status == BudgetStatus.Approved)
                .Sum(b => b.TotalAmount);

            var spentAmount = p.Budgets
                .SelectMany(b => b.BudgetLines)
                .Sum(bl => bl.Expenses.Sum(e => e.Amount));

            var utilization = approvedBudget > 0
                ? Math.Round((spentAmount / approvedBudget) * 100, 2)
                : 0;

            return new ProjectProgressDto
            {
                ProjectId = p.Id,
                ProjectName = p.Name,
                Status = p.Status.ToString(),
                ProgressPercentage = p.ProgressPercentage,
                BudgetUtilization = utilization,
                TotalTasks = p.Tasks.Count,
                CompletedTasks = p.Tasks.Count(t => t.Status == TaskStatus.Completed)
            };
        }).ToList();

        return new DashboardSummaryDto
        {
            TotalProjects = projects.Count,
            ActiveProjects = projects.Count(p => p.Status == ProjectStatus.InProgress),
            CompletedProjects = projects.Count(p => p.Status == ProjectStatus.Completed),
            TotalTasks = projects.Sum(p => p.Tasks.Count),
            OverdueTasks = overdueTasks,
            UpcomingTasks = upcomingTasks,
            TotalBudget = totalBudget,
            TotalExpenses = totalExpenses,
            RemainingBudget = totalBudget - totalExpenses,
            BudgetUtilizationPercentage = totalBudget > 0
                ? Math.Round((totalExpenses / totalBudget) * 100, 2)
                : 0,
            TotalUsers = users.Count,
            ActiveUsers = users.Count(u => u.IsActive),
            ProjectProgress = projectProgress,
            RecentActivities = recentActivities.Select(a => new RecentActivityDto
            {
                Action = a.Action,
                EntityName = a.EntityName,
                UserName = a.User != null ? $"{a.User.FirstName} {a.User.LastName}" : null,
                OccurredAt = a.OccurredAt
            }).ToList()
        };
    }
}
