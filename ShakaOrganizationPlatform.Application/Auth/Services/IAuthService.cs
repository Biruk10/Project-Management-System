using ShakaOrganizationPlatform.Application.Auth.DTOs;

namespace ShakaOrganizationPlatform.Application.Auth.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterOrganizationAsync(RegisterOrganizationDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
}

