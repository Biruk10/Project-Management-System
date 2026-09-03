using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShakaOrganizationPlatform.Application.Auth.DTOs;
using ShakaOrganizationPlatform.Application.Auth.Services;

namespace ShakaOrganizationPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new Organization and its initial Administrator user
    /// </summary>
    [HttpPost("register-organization")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> RegisterOrganization([FromBody] RegisterOrganizationDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new { message = "Organization name is required." });
        }

        if (string.IsNullOrWhiteSpace(dto.AdminEmail) || string.IsNullOrWhiteSpace(dto.AdminPassword))
        {
            return BadRequest(new { message = "Administrator email and password are required." });
        }

        try
        {
            var result = await _authService.RegisterOrganizationAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(RegisterOrganization), result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticates a user and generates JWT access token
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        try
        {
            var result = await _authService.LoginAsync(dto, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}

