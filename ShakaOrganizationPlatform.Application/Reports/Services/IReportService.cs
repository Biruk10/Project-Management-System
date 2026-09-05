using ShakaOrganizationPlatform.Application.Reports.DTOs;

namespace ShakaOrganizationPlatform.Application.Reports.Services;

public interface IReportService
{
    Task<List<ProjectReportDto>> GetProjectReportAsync(ReportFilterParams filters, CancellationToken cancellationToken = default);
    Task<List<BudgetReportDto>> GetBudgetReportAsync(ReportFilterParams filters, CancellationToken cancellationToken = default);
    Task<ExpenseReportDto> GetExpenseReportAsync(ReportFilterParams filters, CancellationToken cancellationToken = default);
    Task<TaskReportDto> GetTaskReportAsync(ReportFilterParams filters, CancellationToken cancellationToken = default);
}
