using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Application.Auth.Services;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string hashedPassword, string providedPassword);
}

