using kol1_poprawa.Services;
using Microsoft.AspNetCore.Mvc;

namespace kol1_poprawa.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IDbService _dbService;
    
    public ProjectsController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        try
        {
            var c = await _dbService.GetProjects(id);
            return Ok(c);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}