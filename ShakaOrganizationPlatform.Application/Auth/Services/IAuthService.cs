using ShakaOrganizationPlatform.Application.Auth.DTOs;

namespace ShakaOrganizationPlatform.Application.Auth.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterOrganizationAsync(RegisterOrganizationDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RegisterSystemAdminAsync(RegisterSystemAdminDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> GrantSystemAdminRoleAsync(GrantSystemAdminDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
    Task LogoutAsync(int userId, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
    Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);
}

