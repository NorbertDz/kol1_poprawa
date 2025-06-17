using kol1_poprawa.Models.DTOs;
using kol1_poprawa.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace kol1_poprawa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArtifactsController : ControllerBase
{
    private readonly IDbService _dbService;
    
    public ArtifactsController(IDbService dbService)
    {
        _dbService = dbService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddArtifacts([FromBody] AddArtifacts dto)
    {
        if (dto is null)
        {
            return BadRequest("Nie prawidłowe dane");
        }

        try
        {
            await _dbService.AddArtifact(dto);
            return Created($"/api/clients/{dto.Artifact.Name}", dto);
            
        }catch(Exception ex)
        {
            if (ex.Message.Contains("istnieje"))
                return Conflict(ex.Message);
            
            if (ex.Message.Contains("Nie znaleziono"))
                return NotFound(ex.Message);
            
            return BadRequest(ex.Message);
        }
    }
}