namespace organizationPlatform.Domain.Entities;
public class User
{
    public int Id {get; set;}
    public required string OrganizId{get; set;}
    public required string FirstName{get; set;}
    public required string LastName{get; set;}
    public required string Email{get; set;}
    public required string PasswordHash{get; set;}
    public string IsActive {get; set;}

}