using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShakaOrganizationPlatform.Application.AuditLogs.DTOs;
using ShakaOrganizationPlatform.Application.AuditLogs.Services;

namespace ShakaOrganizationPlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/audit-logs")]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    [Authorize(Policy = "AuditLog.Read")]
    public async Task<IActionResult> GetAll([FromQuery] AuditLogFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _auditLogService.GetAllAsync(filters, cancellationToken);
        return Ok(result);
    }
}
