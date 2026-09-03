using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShakaOrganizationPlatform.Application.Organizations.DTOs;
using ShakaOrganizationPlatform.Application.Organizations.Services;

namespace ShakaOrganizationPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrganizationsController : ControllerBase
{
    private readonly IOrganizationService _organizationService;

    public OrganizationsController(IOrganizationService organizationService)
    {
        _organizationService = organizationService;
    }

    /// <summary>
    /// Retrieves an Organization by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrganizationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _organizationService.GetByIdAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all Organizations
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrganizationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrganizationDto>>> GetAll(CancellationToken cancellationToken)
    {
        var results = await _organizationService.GetAllAsync(cancellationToken);
        return Ok(results);
    }

    /// <summary>
    /// Updates an existing Organization
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrganizationDto>> Update(int id, [FromBody] UpdateOrganizationDto dto, CancellationToken cancellationToken)
    {
        var result = await _organizationService.UpdateAsync(id, dto, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes an Organization
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _organizationService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
