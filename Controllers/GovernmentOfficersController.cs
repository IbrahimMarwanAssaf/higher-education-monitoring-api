using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Helpers;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[ApiController]
[Route("[controller]")]
public class GovernmentOfficersController : ControllerBase
{
    private readonly IGovernmentOfficerService _governmentOfficerService;
    private readonly IDatabaseValidationHelper _validationHelper;
    public GovernmentOfficersController(IGovernmentOfficerService governmentOfficerService, IDatabaseValidationHelper validationHelper)
    {
        _governmentOfficerService = governmentOfficerService;
        _validationHelper = validationHelper;
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
    public async Task<ActionResult<GovernmentOfficerResponseDto>> Create(CreateGovernmentOfficerDto dto)
    {
        if (await _validationHelper.SSNExistsAsync(dto.SSN))
        {
            return Conflict(new
            {
                message = "The SSN is already in use."
            });
        }

        if (await _validationHelper.GovernmentOfficerEmailExistsAsync(dto.Email))
        {
            return Conflict(new
            {
                message = "The email is already in use."
            });
        }
        GovernmentOfficerResponseDto governmentOfficer = await _governmentOfficerService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetSingle), new { governmentOfficerId = governmentOfficer.OfficerID }, governmentOfficer);
    }

    [HttpPut("Update/{governmentOfficerId}")]
    public async Task<ActionResult> Update(int governmentOfficerId, UpdateGovernmentOfficerDto dto)
    {
        if (!await _validationHelper.GovernmentOfficerExistsAsync(governmentOfficerId))
        {
            return NotFound();
        }

        if (await _validationHelper.GovernmentOfficerEmailExistsAsync(dto.Email, governmentOfficerId))
        {
            return Conflict(new
            {
                message = "Another person already uses this email."
            });
        }

        bool updated = await _governmentOfficerService.UpdateAsync(governmentOfficerId, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = $"GovernmentOfficer with ID: {governmentOfficerId} was not found."
            });
        }

        return NoContent();
    }

    [HttpDelete("Delete/{governmentOfficerId}")]
    public async Task<ActionResult> Delete(int governmentOfficerId)
    {
        if (!await _validationHelper.GovernmentOfficerExistsAsync(governmentOfficerId))
        {
            return NotFound();
        }

        bool deleted = await _governmentOfficerService.DeleteAsync(governmentOfficerId);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"GovernmentOfficer with ID: {governmentOfficerId} was not found."
            });
        }

        return NoContent();
    }

}