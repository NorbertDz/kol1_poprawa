namespace kol1_poprawa.Models.DTOs;

public class AddArtifacts
{
    public ArtifactReq Artifact { get; set; }   
    public ProjectReq Project { get; set; }
}

public class ArtifactReq
{
    public int ArtifactId { get; set; }
    public string Name { get; set; }
    public DateTime OriginDate { get; set; }
    public int InstituionId { get; set; }
}

public class ProjectReq
{
    public int ProjectId { get; set; }
    public string Objective { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}