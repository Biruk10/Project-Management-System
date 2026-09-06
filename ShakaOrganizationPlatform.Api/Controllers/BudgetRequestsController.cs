using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShakaOrganizationPlatform.Application.Budgets.DTOs;
using ShakaOrganizationPlatform.Application.Budgets.Services;
using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/budget-requests")]
public class BudgetRequestsController : ControllerBase
{
    private readonly IBudgetRequestService _budgetRequestService;

    public BudgetRequestsController(IBudgetRequestService budgetRequestService)
    {
        _budgetRequestService = budgetRequestService;
    }

    [HttpGet("project/{projectId:int}")]
    public async Task<IActionResult> GetByProject(int projectId, CancellationToken cancellationToken)
    {
        var result = await _budgetRequestService.GetByProjectAsync(projectId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken)
    {
        var result = await _budgetRequestService.GetPendingAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? projectId,
        [FromQuery] BudgetRequestStatus? status,
        CancellationToken cancellationToken)
    {
        var result = await _budgetRequestService.GetAllAsync(projectId, status, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _budgetRequestService.GetByIdAsync(id, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBudgetRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _budgetRequestService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:int}/review")]
    public async Task<IActionResult> Review(int id, [FromBody] ReviewBudgetRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _budgetRequestService.ReviewAsync(id, dto, cancellationToken);
        return Ok(result);
    }
}
