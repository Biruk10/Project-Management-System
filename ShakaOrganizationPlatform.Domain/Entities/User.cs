

namespace ShakaOrganizationPlatform.Domain.Entities;



public class User : AuditableEntity
{
    public string FirstName { get; set;} = string.Empty;
    public string LastName { get; set;} = string.Empty;
    public string Email { get; set;} = string.Empty;
    public string PasswordHash { get; set;} = string.Empty; 
    public bool IsActive { get; set;} = true;

    public Organization Organization { get; set;} = null!;
}