using ShakaOrganizationPlatform.Application.Budgets.DTOs;

namespace ShakaOrganizationPlatform.Application.Budgets.Services;

public interface IBudgetService
{
    Task<List<BudgetDto>> GetByProjectAsync(int projectId, CancellationToken cancellationToken = default);
    Task<BudgetDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BudgetDto> CreateAsync(CreateBudgetDto dto, CancellationToken cancellationToken = default);
    Task<BudgetDto> UpdateAsync(int id, UpdateBudgetDto dto, CancellationToken cancellationToken = default);
    Task<BudgetDto> ApproveAsync(int id, CancellationToken cancellationToken = default);
    Task<BudgetDto> RejectAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<BudgetDto> AddBudgetLineAsync(int budgetId, AddBudgetLineDto dto, CancellationToken cancellationToken = default);
    Task RemoveBudgetLineAsync(int budgetId, int lineId, CancellationToken cancellationToken = default);
}
