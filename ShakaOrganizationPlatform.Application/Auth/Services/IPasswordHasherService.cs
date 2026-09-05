using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Application.Auth.Services;

public interface IPasswordHasherService
{
    string HashPassword(User user, string plainPassword);
    bool VerifyPassword(User user, string hashedPassword, string plainPassword);
}
