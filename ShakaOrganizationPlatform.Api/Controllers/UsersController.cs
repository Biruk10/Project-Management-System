using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShakaOrganizationPlatform.Application.Users.DTOs;
using ShakaOrganizationPlatform.Application.Users.Services;

namespace ShakaOrganizationPlatform.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = "User.Read")]
    public async Task<IActionResult> GetAll([FromQuery] UserFilterParams filters, CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(filters, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = "User.Read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "User.Create")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "User.Update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:int}/activate")]
    [Authorize(Policy = "User.Update")]
    public async Task<IActionResult> Activate(int id, CancellationToken cancellationToken)
    {
        await _userService.ActivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/deactivate")]
    [Authorize(Policy = "User.Update")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        await _userService.DeactivateAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "User.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:int}/roles")]
    [Authorize(Policy = "Role.Update")]
    public async Task<IActionResult> AssignRoles(int id, [FromBody] AssignRolesDto dto, CancellationToken cancellationToken)
    {
        await _userService.AssignRolesAsync(id, dto, cancellationToken);
        return NoContent();
    }
}
