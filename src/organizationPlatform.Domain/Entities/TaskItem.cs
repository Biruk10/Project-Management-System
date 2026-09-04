namespace OrganizationPlat.src.Domain;
public class TaskItem
{
    public int Id {get; set;}
    public required string OrganizId{get; set;}
    public required string ProjectId{get; set;}
    public required string Title{get; set;}
    public required string Description{get; set;}
    public required int AssignedUSerId{get; set;}
    public required string Priority{get; set;}
    public required string Status {get; set;}
    public required string DueDate{get; set;}
    public required string ComplationPercent{get; set;}
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
   



}