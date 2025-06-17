using kol1_poprawa.Models.DTOs;

namespace kol1_poprawa.Services;

public interface IDbService
{
    Task<GetProjectsDto> GetProjects(int Id);
    Task AddArtifact(AddArtifacts artifact);
}