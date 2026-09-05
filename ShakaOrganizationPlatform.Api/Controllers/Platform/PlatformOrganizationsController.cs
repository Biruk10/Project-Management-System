using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Infrastructure.Persistence;
using ShakaOrganizationPlatform.Infrastructure.Persistence.Seed;

namespace ShakaOrganizationPlatform.Api.Controllers.Platform;

[Authorize(Roles = "SystemAdmin")]
[ApiController]
[Route("api/platform/organizations")]
public class PlatformOrganizationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PlatformOrganizationsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var query = _db.Organizations
            .IgnoreQueryFilters()
            .Where(o => o.Name != SystemAdminSeeder.SystemOrgName);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(o => o.Name.ToLower().Contains(s) || o.Email.ToLower().Contains(s));
        }

        var result = await query
            .OrderBy(o => o.Name)
            .Select(o => new
            {
                o.Id, o.Name, o.Email, o.Phone, o.Address, o.TimeZone, o.IsActive, o.CreatedAt,
                TotalUsers    = o.Users.Count,
                TotalProjects = o.Projects.Count,
                TotalTasks    = o.Projects.SelectMany(p => p.Tasks).Count(),
                TotalExpenses = (decimal?)o.Projects.SelectMany(p => p.Expenses).Sum(e => e.Amount) ?? 0m,
                TotalBudget   = (decimal?)o.Projects.SelectMany(p => p.Budgets).Sum(b => b.TotalAmount) ?? 0m
            })
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var org = await _db.Organizations
            .IgnoreQueryFilters()
            .Where(o => o.Id == id && o.Name != SystemAdminSeeder.SystemOrgName)
            .Select(o => new
            {
                o.Id, o.Name, o.Email, o.Phone, o.Address, o.TimeZone, o.IsActive, o.CreatedAt,
                TotalUsers    = o.Users.Count,
                TotalProjects = o.Projects.Count,
                TotalTasks    = o.Projects.SelectMany(p => p.Tasks).Count(),
                TotalExpenses = (decimal?)o.Projects.SelectMany(p => p.Expenses).Sum(e => e.Amount) ?? 0m,
                TotalBudget   = (decimal?)o.Projects.SelectMany(p => p.Budgets).Sum(b => b.TotalAmount) ?? 0m,
                Users = o.Users
                    .OrderBy(u => u.FirstName)
                    .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email, u.IsActive, u.LastLoginAt, u.CreatedAt })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (org == null) return NotFound();
        return Ok(org);
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        var org = await _db.Organizations.IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (org == null) return NotFound();

        org.IsActive  = true;
        org.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:int}/suspend")]
    public async Task<IActionResult> Suspend(int id, CancellationToken cancellationToken)
    {
        var org = await _db.Organizations.IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        if (org == null) return NotFound();
        if (org.Name == SystemAdminSeeder.SystemOrgName)
            return BadRequest("Cannot suspend the system organisation.");

        org.IsActive  = false;
        org.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
