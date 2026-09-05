using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShakaOrganizationPlatform.Application.Budgets.DTOs;
using ShakaOrganizationPlatform.Application.Budgets.Services;

namespace ShakaOrganizationPlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/budgets")]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet("project/{projectId:int}")]
    [Authorize(Policy = "Budget.Read")]
    public async Task<IActionResult> GetByProject(int projectId, CancellationToken cancellationToken)
    {
        var result = await _budgetService.GetByProjectAsync(projectId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "Budget.Read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _budgetService.GetByIdAsync(id, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Budget.Create")]
    public async Task<IActionResult> Create([FromBody] CreateBudgetDto dto, CancellationToken cancellationToken)
    {
        var result = await _budgetService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "Budget.Create")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBudgetDto dto, CancellationToken cancellationToken)
    {
        var result = await _budgetService.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Policy = "Budget.Approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        var result = await _budgetService.ApproveAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:int}/reject")]
    [Authorize(Policy = "Budget.Approve")]
    public async Task<IActionResult> Reject(int id, CancellationToken cancellationToken)
    {
        var result = await _budgetService.RejectAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "Budget.Create")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _budgetService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/lines")]
    [Authorize(Policy = "Budget.Create")]
    public async Task<IActionResult> AddLine(int id, [FromBody] AddBudgetLineDto dto, CancellationToken cancellationToken)
    {
        var result = await _budgetService.AddBudgetLineAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}/lines/{lineId:int}")]
    [Authorize(Policy = "Budget.Create")]
    public async Task<IActionResult> RemoveLine(int id, int lineId, CancellationToken cancellationToken)
    {
        await _budgetService.RemoveBudgetLineAsync(id, lineId, cancellationToken);
        return NoContent();
    }
}
