using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShakaOrganizationPlatform.Application.Reports.DTOs;
using ShakaOrganizationPlatform.Application.Reports.Services;

namespace ShakaOrganizationPlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("projects")]
    [Authorize(Policy = "Report.View")]
    public async Task<IActionResult> GetProjectReport([FromQuery] ReportFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetProjectReportAsync(filters, cancellationToken);
        return Ok(result);
    }

    [HttpGet("budgets")]
    [Authorize(Policy = "Report.View")]
    public async Task<IActionResult> GetBudgetReport([FromQuery] ReportFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetBudgetReportAsync(filters, cancellationToken);
        return Ok(result);
    }

    [HttpGet("expenses")]
    [Authorize(Policy = "Report.View")]
    public async Task<IActionResult> GetExpenseReport([FromQuery] ReportFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetExpenseReportAsync(filters, cancellationToken);
        return Ok(result);
    }

    [HttpGet("tasks")]
    [Authorize(Policy = "Report.View")]
    public async Task<IActionResult> GetTaskReport([FromQuery] ReportFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _reportService.GetTaskReportAsync(filters, cancellationToken);
        return Ok(result);
    }
}
