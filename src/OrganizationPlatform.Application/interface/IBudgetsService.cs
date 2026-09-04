
using organizationPlatform.Domain.Entities;
namespace organizationPlatform.Application.Interface;
using organizationPlatform.Application.Dtos;

public interface IBudgetsService
{
    
    Task<BudgetResponseDto?> CreateBudgetAsync(CreateBudgetDto budget, CancellationToken cn);
     Task<BudgetResponseDto?> GetByIdAsync(int id, CancellationToken cn);

}