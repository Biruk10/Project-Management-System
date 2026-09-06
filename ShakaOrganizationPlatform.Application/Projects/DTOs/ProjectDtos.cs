using ShakaOrganizationPlatform.Domain.Enums;

namespace ShakaOrganizationPlatform.Application.Projects.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public int? ProjectManagerId { get; set; }
    public string? ProjectManagerName { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ProjectMemberDto> Members { get; set; } = new();
}

public class ProjectMemberDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string ProjectRole { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? ProjectManagerId { get; set; }
    public decimal? InitialBudget { get; set; }
    public string? BudgetCategory { get; set; }
}

public class UpdateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public int? ProjectManagerId { get; set; }
}

public class AddProjectMemberDto
{
    public int? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string ProjectRole { get; set; } = "Member";
}

public class ProjectFilterParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public ProjectStatus? Status { get; set; }
    public int? ProjectManagerId { get; set; }
}
