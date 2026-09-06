using ShakaOrganizationPlatform.Application.Budgets.DTOs;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Application.Budgets.Services;

public interface IBudgetRequestService
{
    Task<List<BudgetRequestDto>> GetByProjectAsync(int projectId, CancellationToken cancellationToken = default);
    Task<List<BudgetRequestDto>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<List<BudgetRequestDto>> GetAllAsync(int? projectId = null, BudgetRequestStatus? status = null, CancellationToken cancellationToken = default);
    Task<BudgetRequestDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BudgetRequestDto> CreateAsync(CreateBudgetRequestDto dto, CancellationToken cancellationToken = default);
    Task<BudgetRequestDto> ReviewAsync(int id, ReviewBudgetRequestDto dto, CancellationToken cancellationToken = default);
}
