namespace kol1_poprawa.Models.DTOs;

public class GetProjectsDto
{
    public int ProjectId { get; set; }
    public string Objective { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ArtifactDto Artifact { get; set; }
    public List<StaffAssignmentsDto> StaffAssignments { get; set; }
}

public class ArtifactDto
{
    public string Name { get; set; }
    public DateTime OriginDate { get; set; }
    public InstitutionDto Institution { get; set; }
}

public class InstitutionDto
{
    public int InstitutionId { get; set; }
    public string Name { get; set; }
    public int FoundedYear { get; set; }
}

public class StaffAssignmentsDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime HireDate { get; set; }
    public string Role { get; set; }
}