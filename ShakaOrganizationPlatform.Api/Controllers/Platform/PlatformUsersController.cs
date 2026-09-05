using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Infrastructure.Persistence;
using ShakaOrganizationPlatform.Infrastructure.Persistence.Seed;

namespace ShakaOrganizationPlatform.Api.Controllers.Platform;

[Authorize(Roles = "SystemAdmin")]
[ApiController]
[Route("api/platform/users")]
public class PlatformUsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public PlatformUsersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Users
            .IgnoreQueryFilters()
            .Include(u => u.Organization)
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .Where(u => u.Organization.Name != SystemAdminSeeder.SystemOrgName);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(s) ||
                u.LastName.ToLower().Contains(s)  ||
                u.Email.ToLower().Contains(s));
        }

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(u => u.Organization.Name).ThenBy(u => u.FirstName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(u => new
            {
                u.Id, u.FirstName, u.LastName, u.Email,
                u.IsActive, u.Status, u.LastLoginAt,
                u.FailedLoginAttempts, u.LockoutEnd, u.CreatedAt,
                OrganizationId   = u.OrganizationId,
                OrganizationName = u.Organization.Name,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            })
            .ToListAsync(cancellationToken);

        return Ok(new { Items = users, TotalCount = total, Page = page, PageSize = pageSize });
    }

    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        var user = await _db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null) return NotFound();

        user.IsActive            = true;
        user.LockoutEnd          = null;
        user.FailedLoginAttempts = 0;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        var user = await _db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null) return NotFound();

        user.IsActive = false;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
