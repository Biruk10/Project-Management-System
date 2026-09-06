using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShakaOrganizationPlatform.Application.Projects.DTOs;
using ShakaOrganizationPlatform.Application.Projects.Services;

namespace ShakaOrganizationPlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    [Authorize(Policy = "Project.Read")]
    public async Task<IActionResult> GetAll([FromQuery] ProjectFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetAllAsync(filters, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "Project.Read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _projectService.GetByIdAsync(id, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "Project.Create")]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto, CancellationToken cancellationToken)
    {
        var result = await _projectService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "Project.Update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto dto, CancellationToken cancellationToken)
    {
        var result = await _projectService.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "Project.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _projectService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/members")]
    [Authorize(Policy = "Project.Update")]
    public async Task<IActionResult> AddMember(int id, [FromBody] AddProjectMemberDto dto, CancellationToken cancellationToken)
    {
        var result = await _projectService.AddMemberAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:int}/members/{memberId:int}")]
    [Authorize(Policy = "Project.Update")]
    public async Task<IActionResult> RemoveMember(int id, int memberId, CancellationToken cancellationToken)
    {
        await _projectService.RemoveMemberAsync(id, memberId, cancellationToken);
        return NoContent();
    }
}
