using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Organizations.DTOs;
using ShakaOrganizationPlatform.Application.Organizations.Services;
using ShakaOrganizationPlatform.Infrastructure.Persistence;
using ShakaOrganizationPlatform.Infrastructure.Persistence.Seed;

namespace ShakaOrganizationPlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/organizations")]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;
    private readonly AppDbContext _db;

    public OrganizationsController(IOrganizationService organizationService, AppDbContext db)
    {
        _organizationService = organizationService;
        _db = db;
    }

    [HttpGet]
    [Authorize(Policy = "Organization.Read")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var result = await _organizationService.GetCurrentAsync(cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Policy = "Organization.Update")]
    public async Task<IActionResult> Update([FromBody] UpdateOrganizationDto dto, CancellationToken cancellationToken)
    {
        var result = await _organizationService.UpdateAsync(dto, cancellationToken);
        return Ok(result);
    }

    [HttpGet("all")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var orgs = await _db.Organizations
            .IgnoreQueryFilters()
            .Where(o => o.Name != SystemAdminSeeder.SystemOrgName)
            .Select(o => new
            {
                o.Id,
                o.Name,
                o.Email,
                o.IsActive,
                o.CreatedAt,
                TotalUsers = o.Users.Count,
                TotalProjects = o.Projects.Count,
                TotalTasks = o.Projects.SelectMany(p => p.Tasks).Count(),
                TotalExpenses = o.Projects
                    .SelectMany(p => p.Expenses)
                    .Sum(e => (decimal?)e.Amount) ?? 0m,
                TotalBudget = o.Projects
                    .SelectMany(p => p.Budgets)
                    .Sum(b => (decimal?)b.TotalAmount) ?? 0m
            })
            .OrderBy(o => o.Name)
            .ToListAsync(cancellationToken);

        return Ok(orgs);
    }
}
