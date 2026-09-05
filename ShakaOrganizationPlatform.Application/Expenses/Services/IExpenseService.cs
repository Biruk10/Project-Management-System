using ShakaOrganizationPlatform.Application.Common.Models;
using ShakaOrganizationPlatform.Application.Expenses.DTOs;

namespace ShakaOrganizationPlatform.Application.Expenses.Services;

public interface IExpenseService
{
    Task<PagedResult<ExpenseDto>> GetAllAsync(ExpenseFilterParams filters, CancellationToken cancellationToken = default);
    Task<ExpenseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ExpenseDto> CreateAsync(CreateExpenseDto dto, CancellationToken cancellationToken = default);
    Task<ExpenseDto> UpdateAsync(int id, UpdateExpenseDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
