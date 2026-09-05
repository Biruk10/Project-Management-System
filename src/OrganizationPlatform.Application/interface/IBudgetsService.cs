
using organizationPlatform.Domain.Entities;
namespace organizationPlatform.Application.Interface;
using organizationPlatform.Application.Dtos;

public interface IBudgetsService
{
    
    Task<BudgetResponseDto?> CreateBudgetAsync(CreateBudgetDto budget, CancellationToken cn);
     Task<BudgetResponseDto?> GetById(int id, CancellationToken cn);

}