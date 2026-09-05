using Microsoft.AspNetCore.Mvc;
using organizationPlatform.Application.Interface;
using organizationPlatform.Application.Dtos;
using organizationPlatform.Domain.Entities;
namespace organizationPlatform.Api.Controllers;
[Route("api/budgets")]
[ApiController]


public class BudgetsController(IBudgetsService budgetsService):ControllerBase{
     [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cn)
    {
        var result = await budgetsService.GetById(id, cn);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
[HttpPost]
public async Task<IActionResult> CreateBudgetAsync(CreateBudgetDto budget, CancellationToken cn)
{


    // save budget...
var result = await budgetsService.CreateBudgetAsync(budget, cn);
        return
        CreatedAtAction(nameof(GetById),
        new { id = result.Id }, result);
}
}