using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Universities;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[ApiController]
[Route("[controller]")]
public class UniversityController : ControllerBase
{
    private readonly IUniversityService _universityService;
    private readonly IDatabaseValidationHelper _validationHelper;
    public UniversityController(IUniversityService universityService, IDatabaseValidationHelper validationHelper)
    {
        _universityService = universityService;
        _validationHelper = validationHelper;
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

        if (await _validationHelper.UniversityNameExistsAsync(dto.UniversityName))
        {
            return Conflict(new
            {
                message = "A university with this name already exists."
            });
        }

        University university = await _universityService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetSingle), new { universityId = university.UniversityID }, university); // [201 + location + body]
                                                                                                               //OR
                                                                                                               //return Ok(university); //[200 + body]
    }

    [HttpPut("Update/{universityId}")]
    public async Task<ActionResult> Update(int universityId, UniversityDto dto)
    {

        if (!await _validationHelper.UniversityExistsAsync(universityId))
        {
            return NotFound();
        }

        if (await _validationHelper.UniversityNameExistsAsync(dto.UniversityName, universityId))
        {
            return Conflict(new
            {
                message = "Another university already uses this name."
            });
        }

        bool updated = await _universityService.UpdateAsync(universityId, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = $"University with ID: {universityId} was not found."
            });
        }

        return NoContent();
    }

    [HttpDelete("Delete/{universityId}")]
    public async Task<IActionResult> Delete(int universityId)
    {
        if (!await _validationHelper.UniversityExistsAsync(universityId))
        {
            return NotFound();
        }

        if (await _validationHelper.UniversityHasDependenciesAsync(universityId))
        {
            return Conflict(new
            {
                message = "The university has related students, teachers or courses."
            });
        }

        bool deleted = await _universityService.DeleteAsync(universityId);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"University with ID: {universityId} was not found."
            });
        }

        return NoContent();
    }
}