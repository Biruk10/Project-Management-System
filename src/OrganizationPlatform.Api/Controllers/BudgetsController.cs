using Microsoft.AspNetCore.Mvc;
using organizationPlatform.Application.Interface;
using organizationPlatform.Application.Dtos;
using organizationPlatform.Domain.Entities;
namespace organizationPlatform.Api.Controllers;
[Route("api/budgets")]
[ApiController]


public class BudgetsController(IBudgetsService budgetsService):ControllerBase{
     [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(
        int id,
        CancellationToken cn)
    {
        var result = await budgetsService.GetByIdAsync(id, cn);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
[HttpPost]
public async Task<IActionResult> CreateBudgetAsync(CreateBudgetDto budget, CancellationToken cn)
{
    var budgets = new Budget
    {
        Id = budget.Id,
        OrganizId = budget.OrganizId,
         ProjectId = budget.ProjectId,
        TotalAmount = budget.TotalAmount,
        ApprovedBy = budget.ApprovedBy,
        ApprovedAt = budget.ApprovedAt,
    };


    // save budget...
var result = await budgetsService.CreateBudgetAsync(budget, cn);
        return
        CreatedAtAction(nameof(GetByIdAsync),
        new { id = result.Id }, result);
}
}