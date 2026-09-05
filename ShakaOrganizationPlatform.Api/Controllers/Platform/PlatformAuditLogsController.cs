using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Infrastructure.Persistence;
using ShakaOrganizationPlatform.Infrastructure.Persistence.Seed;

namespace ShakaOrganizationPlatform.Api.Controllers.Platform;

[Authorize(Roles = "SystemAdmin")]
[ApiController]
[Route("api/platform/audit-logs")]
public class PlatformAuditLogsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PlatformAuditLogsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? entityName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _db.AuditLogs
            .IgnoreQueryFilters()
            .Include(a => a.User)
            .Include(a => a.Organization)
            .Where(a => a.Organization.Name != SystemAdminSeeder.SystemOrgName);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(a =>
                a.Action.ToLower().Contains(s) ||
                a.EntityName.ToLower().Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(a => a.EntityName == entityName);

        var total = await query.CountAsync(cancellationToken);

        var logs = await query
            .OrderByDescending(a => a.OccurredAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(a => new
            {
                a.Id, a.Action, a.EntityName, a.EntityId,
                a.IpAddress, a.OccurredAt,
                OrganizationName = a.Organization.Name,
                UserName  = a.User != null ? a.User.FirstName + " " + a.User.LastName : null,
                UserEmail = a.User != null ? a.User.Email : null
            })
            .ToListAsync(cancellationToken);

        return Ok(new { Items = logs, TotalCount = total, Page = page, PageSize = pageSize });
    }
}
