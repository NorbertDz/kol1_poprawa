using System.Data.Common;
using kol1_poprawa.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace kol1_poprawa.Services;

public class DbService :IDbService
{
    private readonly string _connectionString;
    
    public DbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    }
    
    public async Task<GetProjectsDto> GetProjects(int Id)
    {
        var query =
            @"select 
            pp.ProjectId,pp.Objective,pp.StartDate,pp.EndDate,
            a.Name,a.OriginDate,
            i.InstitutionId,i.Name,i.FoundedYear,
            s.FirstName,s.LastName,s.HireDate,
            sa.Role 
                from Preservation_Project pp
                join Artifact a on pp.ArtifactId=a.ArtifactId
                join Institution i on a.InstitutionId=i.InstitutionId
                join Staff_Assignment sa on pp.ProjectId = sa.ProjectId 
                join Staff s on s.StaffId = sa.StaffId";
        
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        await connection.OpenAsync();

        GetProjectsDto? dto = null;
        
        command.Parameters.AddWithValue("@id", Id);
        var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            if (dto is null)
            {
                dto = new GetProjectsDto()
                {
                    ProjectId = reader.GetInt32(0),
                    Objective = reader.GetString(1),
                    StartDate = reader.GetDateTime(2),
                    Artifact = new ArtifactDto()
                    {
                        Name = reader.GetString(4),
                        OriginDate = reader.GetDateTime(5),
                        Institution = new InstitutionDto()
                        {
                            InstitutionId = reader.GetInt32(6),
                            Name = reader.GetString(7),
                            FoundedYear = reader.GetInt32(8)
                        }
                    },
                    StaffAssignments = new List<StaffAssignmentsDto>()
                };
                dto.StaffAssignments.Add(new StaffAssignmentsDto()
                {
                    FirstName = reader.GetString(9),
                    LastName = reader.GetString(10),
                    HireDate = reader.GetDateTime(11),
                    Role = reader.GetString(12)
                });

                if (reader.GetDateTime(3) != null)
                {
                    dto.EndDate = reader.GetDateTime(3);
                }
            }
        }
        if (dto is null)
        {
            throw new Exception("Nie znaleziono projectu o podanym ID");
        }

        return dto;
    }

    public async Task AddArtifact(AddArtifacts artifact)
    {
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        { 
            command.Parameters.Clear();
            command.CommandText = @"Select InstitutionId from Institution where InstitutionId=@InstitutionId";
            command.Parameters.AddWithValue("@InstitutionId", artifact.Artifact.InstituionId);
            var InstitutionIdResult = await command.ExecuteScalarAsync();
            if (InstitutionIdResult != null)
            {
                throw new Exception("Nie znaleziono Institution o podanym ID");
            }
            
            command.Parameters.Clear();
            command.CommandText = @"Select ArtifactId from Artifact where ArtifactId=@ArtifactId";
            command.Parameters.AddWithValue("@ArtifactId", artifact.Artifact.ArtifactId);
            var ArtifactIdResult = await command.ExecuteScalarAsync();
            if (ArtifactIdResult != null)
            {
                throw new Exception("Artifact istnieje o podanym ID");
            }
            
            command.Parameters.Clear();
            command.CommandText = @"Insert into Artifact (ArtifactId,Name,OriginDate,InstitutionId) values (@ArtifactId,@Name,@OriginDate,@InstitutionId)";
            command.Parameters.AddWithValue("@ArtifactId", ArtifactIdResult);
            command.Parameters.AddWithValue("@Name",artifact.Artifact.Name);
            command.Parameters.AddWithValue("@OriginDate",artifact.Artifact.OriginDate);
            command.Parameters.AddWithValue("@InstitutionId",InstitutionIdResult);
            
            command.Parameters.Clear();
            command.CommandText = @"Select ProjectId from Preservation_Project where ProjectId=@ProjectId";
            command.Parameters.AddWithValue("@ProjectId", artifact.Project.ProjectId);
            var projectIdResult = await command.ExecuteScalarAsync();
            if (projectIdResult == null)
            {
                throw new Exception("Istnieje projekt o podanym ID");
            }
            
            command.Parameters.Clear();
            command.CommandText = @"Insert into Preservation_Project (ProjectId,ArtifactId,StartDate,EndDate,Objective) values (@ProjectId,@ArtifactId,@StartDate,@EndDate,@Objective)";
            command.Parameters.AddWithValue("@ProjectId", projectIdResult);
            command.Parameters.AddWithValue("@ArtifactId", artifact.Artifact.ArtifactId);
            command.Parameters.AddWithValue("@StartDate", artifact.Project.StartDate);
            if (artifact.Project.EndDate != null)
            {
                command.Parameters.AddWithValue("@EndDate", artifact.Project.EndDate);
            }
            command.Parameters.AddWithValue("@Objective", artifact.Project.Objective);
            
            await command.ExecuteNonQueryAsync();
            
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}