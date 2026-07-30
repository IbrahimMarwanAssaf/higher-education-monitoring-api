using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Universities;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[ApiController]
[Route("[controller]")]
public class UniversityController : ControllerBase
{
    private readonly IUniversityService _universityService;
    public UniversityController(IUniversityService universityService)
    {
        _universityService = universityService;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<University>>> GetAll()
    {
        List<University> universities = await _universityService.GetAllAsync();

        return Ok(universities);
    }

    [HttpGet("GetSingle/{universityId}")]
    public async Task<ActionResult<University>> GetSingle(int universityId)
    {
        University? university = await _universityService.GetSingleAsync(universityId);

        if (university is null)
        {
            return NotFound(new
            {
                message = $"University with ID: {universityId} was not found."
            });
        }

        return Ok(university);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<University>> Create(UniversityDto dto)
    {
        University? university = await _universityService.CreateAsync(dto);

        if (university is null)
        {
            return BadRequest(new
            {
                message = "Unable to create university."
            });
        }

        return CreatedAtAction(nameof(GetSingle), new { universityId = university.UniversityID }, university); // [201 + location + body]
                                                                                                               //OR
                                                                                                               //return Ok(university); //[200 + body]
    }

    [HttpPut("Update")]
    public async Task<ActionResult> Update(int universityId, UniversityDto dto)
    {
        University? existingUniversity = await _universityService.GetSingleAsync(universityId);

        if (existingUniversity is null)
        {
            return NotFound(new
            {
                message = $"University {universityId} was not found."
            });
        }

        await _universityService.UpdateAsync(universityId, dto);

        return NoContent();
    }

    [HttpDelete("Delete/{universityId}")]
    public async Task<IActionResult> Delete(int universityId)
    {
        University? university = await _universityService.GetSingleAsync(universityId);
        if (university is null)
        {
            return NotFound(new
            {
                message = $"University {universityId} was not found."
            });
        }

        bool hasDependencies = await _universityService.HasDependenciesAsync(universityId);

        if (hasDependencies)
        {
            return Conflict(new
            {
                message = "This university cannot be deleted because students, teachers, or courses reference it."
            });
        }

        await _universityService.DeleteAsync(universityId);

        return NoContent();
    }
}