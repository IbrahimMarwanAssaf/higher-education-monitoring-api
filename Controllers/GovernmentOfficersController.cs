using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Filters;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[Authorize(Roles = "Admin")]
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
        List<GovernmentOfficerResponseDto> governmentOfficers =
            await _governmentOfficerService.GetAllAsync();
        return Ok(governmentOfficers);
    }

    [HttpGet("GetSingle/{governmentOfficerId}")]
    public async Task<ActionResult<GovernmentOfficerResponseDto>> GetSingle(int governmentOfficerId)
    {
        GovernmentOfficerResponseDto governmentOfficer =
            await _governmentOfficerService.GetSingleAsync(governmentOfficerId);
        return Ok(governmentOfficer);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<GovernmentOfficerResponseDto>> Create(CreateGovernmentOfficerDto dto)
    {
        GovernmentOfficerResponseDto governmentOfficer =
            await _governmentOfficerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetSingle), new
        {
            governmentOfficerId =
            governmentOfficer.OfficerID
        }, governmentOfficer);
    }

    [HttpPut("Update/{governmentOfficerId}")]
    public async Task<ActionResult> Update(int governmentOfficerId, UpdateGovernmentOfficerDto dto)
    {
        await _governmentOfficerService.UpdateAsync(governmentOfficerId, dto);
        return NoContent();
    }

    [ServiceFilter(typeof(AuditDeleteFilter))]
    [HttpDelete("Delete/{governmentOfficerId}")]
    public async Task<ActionResult> Delete(int governmentOfficerId)
    {
        await _governmentOfficerService.DeleteAsync(governmentOfficerId);
        return NoContent();
    }
}