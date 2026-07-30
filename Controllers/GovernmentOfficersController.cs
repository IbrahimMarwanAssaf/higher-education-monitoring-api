using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Models;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[ApiController]
[Route("[controller]")]
public class GovernmentOfficersController : ControllerBase
{
    private readonly IGovernmentOfficerService _governmentOfficerService;
    public GovernmentOfficersController(IGovernmentOfficerService governmentOfficerService)
    {
        _governmentOfficerService = governmentOfficerService;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<GovernmentOfficerResponseDto>>> GetAll()
    {
        List<GovernmentOfficerResponseDto> governmentOfficers = await _governmentOfficerService.GetAllAsync();

        return Ok(governmentOfficers);
    }

    [HttpGet("GetSingle/{governmentOfficerId}")]
    public async Task<ActionResult<GovernmentOfficerResponseDto>> GetSingle(int governmentOfficerId)
    {
        GovernmentOfficerResponseDto? governmentOfficer = await _governmentOfficerService.GetSingleAsync(governmentOfficerId);

        if (governmentOfficer is null)
        {
            return NotFound(new
            {
                message = $"GovernmentOfficer with ID: {governmentOfficerId} was not found."
            });
        }

        return Ok(governmentOfficer);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<GovernmentOfficer>> Create(CreateGovernmentOfficerDto dto)
    {
        GovernmentOfficerResponseDto? governmentOfficer = await _governmentOfficerService.CreateAsync(dto);

        if (governmentOfficer is null)
        {
            return BadRequest(new
            {
                message = "Unable to create governmentOfficer."
            });
        }

        return CreatedAtAction(nameof(GetSingle), new { governmentOfficerId = governmentOfficer.OfficerID }, governmentOfficer);
    }

    [HttpPut("Update/{governmentOfficerId}")]
    public async Task<ActionResult> Update(int governmentOfficerId, UpdateGovernmentOfficerDto dto)
    {
        GovernmentOfficerResponseDto? existingGovernmentOfficer = await _governmentOfficerService.GetSingleAsync(governmentOfficerId);

        if (existingGovernmentOfficer is null)
        {
            return NotFound(new
            {
                message = $"GovernmentOfficer with ID: {governmentOfficerId} was not found."
            });
        }

        await _governmentOfficerService.UpdateAsync(governmentOfficerId, dto);

        return NoContent();
    }

    [HttpDelete("Delete/{governmentOfficerId}")]
    public async Task<ActionResult> Delete(int governmentOfficerId)
    {
        GovernmentOfficerResponseDto? existingGovernmentOfficer = await _governmentOfficerService.GetSingleAsync(governmentOfficerId);

        if (existingGovernmentOfficer is null)
        {
            return NotFound(new
            {
                message = $"GovernmentOfficer with ID: {governmentOfficerId} was not found."
            });
        }

        await _governmentOfficerService.DeleteAsync(governmentOfficerId);

        return NoContent();
    }

}