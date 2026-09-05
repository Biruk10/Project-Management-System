using Microsoft.EntityFrameworkCore;
using organizationPlatform.Domain.Entities;
using organizationPlatform.Application.Interface;
namespace organizationPlatform.Infrastructure.Services;

using organizationPlatform.Application.Dtos;
using organizationPlatform.Infrastructure.Persistence;

public class BudgetService(AppDbContext context, ILogger<BudgetService>
logger) : IBudgetsService
{

     public Task<BudgetResponseDto?> GetById(int id,
        CancellationToken cn)
    {
        return context.Budgets
 .AsNoTracking()
 .Where(c => c.Id == id)
 .Select(c => new BudgetResponseDto(
 c.Id, c.TotalAmount))
 .FirstOrDefaultAsync(cn);
    }
public async Task<BudgetResponseDto?> CreateBudgetAsync(
        CreateBudgetDto Budget,
        CancellationToken cn)
    {
        var budget = new Budget
        {
            OrganizId = Budget.OrganizId,
            ProjectId = Budget.ProjectId,
            TotalAmount = Budget.TotalAmount,
            Status = Budget.Status,
            ApprovedBy = Budget.ApprovedBy,
            ApprovedAt = Budget.ApprovedAt,
        };
        context.Budgets.Add(budget);
        await context.SaveChangesAsync(cn);
        logger.LogInformation("Created budget {BudgetId} ({TotalAmount})", budget.Id, budget.TotalAmount);
        return (await GetById(budget.Id, cn))!;
    }
}