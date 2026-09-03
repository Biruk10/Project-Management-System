using ShakaOrganizationPlatform.Application.Auth.DTOs;
using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Application.Auth.Services;

public interface IJwtTokenGenerator
{
    (string token, DateTime expiresAt) GenerateAccessToken(User user, Organization organization, IEnumerable<string> roles);
    string GenerateRefreshToken();
}

