using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Infrastructure.Persistence;
using ShakaOrganizationPlatform.Infrastructure.Persistence.Seed;

namespace ShakaOrganizationPlatform.Api.Controllers.Platform;

[Authorize(Roles = "SystemAdmin")]
[ApiController]
[Route("api/platform/dashboard")]
public class PlatformDashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public PlatformDashboardController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var orgs = await _db.Organizations
            .IgnoreQueryFilters()
            .Where(o => o.Name != SystemAdminSeeder.SystemOrgName)
            .Select(o => new
            {
                o.Id, o.Name, o.Email, o.IsActive, o.CreatedAt,
                TotalUsers    = o.Users.Count,
                TotalProjects = o.Projects.Count,
                TotalTasks    = o.Projects.SelectMany(p => p.Tasks).Count(),
                TotalExpenses = (decimal?)o.Projects.SelectMany(p => p.Expenses).Sum(e => e.Amount) ?? 0m,
                TotalBudget   = (decimal?)o.Projects.SelectMany(p => p.Budgets).Sum(b => b.TotalAmount) ?? 0m
            })
            .ToListAsync(cancellationToken);

        var recentOrgs = await _db.Organizations
            .IgnoreQueryFilters()
            .Where(o => o.Name != SystemAdminSeeder.SystemOrgName)
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new { o.Id, o.Name, o.Email, o.IsActive, o.CreatedAt })
            .ToListAsync(cancellationToken);

        var recentActivity = await _db.AuditLogs
            .IgnoreQueryFilters()
            .Include(a => a.User)
            .Include(a => a.Organization)
            .Where(a => a.Organization.Name != SystemAdminSeeder.SystemOrgName)
            .OrderByDescending(a => a.OccurredAt)
            .Take(8)
            .Select(a => new
            {
                a.Action, a.EntityName,
                OrgName  = a.Organization.Name,
                UserName = a.User != null ? a.User.FirstName + " " + a.User.LastName : null,
                a.OccurredAt
            })
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            TotalOrganizations     = orgs.Count,
            ActiveOrganizations    = orgs.Count(o => o.IsActive),
            SuspendedOrganizations = orgs.Count(o => !o.IsActive),
            TotalUsers             = orgs.Sum(o => o.TotalUsers),
            TotalProjects          = orgs.Sum(o => o.TotalProjects),
            TotalTasks             = orgs.Sum(o => o.TotalTasks),
            TotalBudget            = orgs.Sum(o => o.TotalBudget),
            TotalExpenses          = orgs.Sum(o => o.TotalExpenses),
            RecentOrganizations    = recentOrgs,
            RecentActivity         = recentActivity
        });
    }
}
