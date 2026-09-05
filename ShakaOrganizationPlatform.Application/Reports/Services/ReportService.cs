using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Reports.DTOs;
using ShakaOrganizationPlatform.Domain.Enums;
using TaskStatus = ShakaOrganizationPlatform.Domain.Enums.TaskStatus;

namespace ShakaOrganizationPlatform.Application.Reports.Services;

public class ReportService : IReportService
{
    private readonly IAppDbContext _context;

    public ReportService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProjectReportDto>> GetProjectReportAsync(ReportFilterParams filters, CancellationToken cancellationToken = default)
    {
        var query = _context.Projects
            .Include(p => p.ProjectManager)
            .Include(p => p.Members)
            .Include(p => p.Tasks)
            .Include(p => p.Budgets).ThenInclude(b => b.BudgetLines).ThenInclude(bl => bl.Expenses)
            .AsQueryable();

        if (filters.ProjectId.HasValue)
            query = query.Where(p => p.Id == filters.ProjectId.Value);

        var projects = await query.OrderBy(p => p.Name).ToListAsync(cancellationToken);
        var now = DateTime.UtcNow;

        return projects.Select(p =>
        {
            var approvedBudget = p.Budgets
                .Where(b => b.Status == BudgetStatus.Approved)
                .Sum(b => b.TotalAmount);

            var totalExpenses = p.Budgets
                .SelectMany(b => b.BudgetLines)
                .Sum(bl => bl.Expenses.Sum(e => e.Amount));

            return new ProjectReportDto
            {
                ProjectId = p.Id,
                ProjectName = p.Name,
                Status = p.Status.ToString(),
                ProgressPercentage = p.ProgressPercentage,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ProjectManagerName = p.ProjectManager != null
                    ? $"{p.ProjectManager.FirstName} {p.ProjectManager.LastName}"
                    : null,
                TotalMembers = p.Members.Count,
                TotalTasks = p.Tasks.Count,
                CompletedTasks = p.Tasks.Count(t => t.Status == TaskStatus.Completed),
                OverdueTasks = p.Tasks.Count(t => t.DueDate.HasValue && t.DueDate < now && t.Status != TaskStatus.Completed),
                TotalBudget = approvedBudget,
                TotalExpenses = totalExpenses,
                RemainingBudget = approvedBudget - totalExpenses,
                BudgetUtilization = approvedBudget > 0
                    ? Math.Round((totalExpenses / approvedBudget) * 100, 2)
                    : 0
            };
        }).ToList();
    }

    public async Task<List<BudgetReportDto>> GetBudgetReportAsync(ReportFilterParams filters, CancellationToken cancellationToken = default)
    {
        var query = _context.Budgets
            .Include(b => b.Project)
            .Include(b => b.BudgetLines).ThenInclude(bl => bl.Expenses)
            .Where(b => b.Status == BudgetStatus.Approved)
            .AsQueryable();

        if (filters.ProjectId.HasValue)
            query = query.Where(b => b.ProjectId == filters.ProjectId.Value);

        var budgets = await query.ToListAsync(cancellationToken);

        return budgets.Select(b =>
        {
            var spent = b.BudgetLines.Sum(bl => bl.Expenses.Sum(e => e.Amount));
            var remaining = b.TotalAmount - spent;

            return new BudgetReportDto
            {
                ProjectId = b.ProjectId,
                ProjectName = b.Project?.Name ?? string.Empty,
                TotalApprovedBudget = b.TotalAmount,
                TotalExpenses = spent,
                RemainingBudget = remaining,
                UtilizationPercentage = b.TotalAmount > 0
                    ? Math.Round((spent / b.TotalAmount) * 100, 2)
                    : 0,
                Lines = b.BudgetLines.Select(bl => new BudgetLineReportDto
                {
                    Category = bl.Category,
                    AllocatedAmount = bl.AllocatedAmount,
                    SpentAmount = bl.Expenses.Sum(e => e.Amount),
                    Remaining = bl.AllocatedAmount - bl.Expenses.Sum(e => e.Amount)
                }).ToList()
            };
        }).ToList();
    }

    public async Task<ExpenseReportDto> GetExpenseReportAsync(ReportFilterParams filters, CancellationToken cancellationToken = default)
    {
        var from = filters.From ?? DateTime.UtcNow.AddMonths(-1);
        var to = filters.To ?? DateTime.UtcNow;

        var query = _context.Expenses
            .Include(e => e.Project)
            .Include(e => e.BudgetLine)
            .Include(e => e.RecordedByUser)
            .Where(e => e.ExpenseDate >= from && e.ExpenseDate <= to);

        if (filters.ProjectId.HasValue)
            query = query.Where(e => e.ProjectId == filters.ProjectId.Value);

        var expenses = await query.OrderByDescending(e => e.ExpenseDate).ToListAsync(cancellationToken);

        return new ExpenseReportDto
        {
            From = from,
            To = to,
            TotalAmount = expenses.Sum(e => e.Amount),
            Expenses = expenses.Select(e => new ExpenseReportItemDto
            {
                Id = e.Id,
                ProjectName = e.Project?.Name ?? string.Empty,
                BudgetLineCategory = e.BudgetLine?.Category,
                Amount = e.Amount,
                Description = e.Description,
                ExpenseDate = e.ExpenseDate,
                RecordedBy = e.RecordedByUser != null
                    ? $"{e.RecordedByUser.FirstName} {e.RecordedByUser.LastName}"
                    : null
            }).ToList()
        };
    }

    public async Task<TaskReportDto> GetTaskReportAsync(ReportFilterParams filters, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var query = _context.TaskItems
            .Include(t => t.Project)
            .Include(t => t.AssignedToUser)
            .AsQueryable();

        if (filters.ProjectId.HasValue)
            query = query.Where(t => t.ProjectId == filters.ProjectId.Value);

        var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync(cancellationToken);

        return new TaskReportDto
        {
            TotalTasks = tasks.Count,
            TodoTasks = tasks.Count(t => t.Status == TaskStatus.Todo),
            InProgressTasks = tasks.Count(t => t.Status == TaskStatus.InProgress),
            CompletedTasks = tasks.Count(t => t.Status == TaskStatus.Completed),
            OverdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate < now && t.Status != TaskStatus.Completed),
            Tasks = tasks.Select(t => new TaskReportItemDto
            {
                Id = t.Id,
                ProjectName = t.Project?.Name ?? string.Empty,
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                AssignedTo = t.AssignedToUser != null
                    ? $"{t.AssignedToUser.FirstName} {t.AssignedToUser.LastName}"
                    : null,
                DueDate = t.DueDate,
                CompletionPercentage = t.CompletionPercentage
            }).ToList()
        };
    }
}
