using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Constants;
using UNIOOP.App.Dtos.GovernmentOfficers;
using UNIOOP.App.Filters;
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

    [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
    [HttpGet("GetAll")]
    public async Task<ActionResult<List<GovernmentOfficerResponseDto>>> GetAll()
    {
        List<GovernmentOfficerResponseDto> governmentOfficers =
            await _governmentOfficerService.GetAllAsync();
        return Ok(governmentOfficers);
    }

    [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
    [HttpGet("GetSingle/{governmentOfficerId}")]
    public async Task<ActionResult<GovernmentOfficerResponseDto>> GetSingle(int governmentOfficerId)
    {
        GovernmentOfficerResponseDto governmentOfficer =
            await _governmentOfficerService.GetSingleAsync(governmentOfficerId);
        return Ok(governmentOfficer);
    }

    [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
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

    [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
    [HttpPut("Update/{governmentOfficerId}")]
    public async Task<ActionResult> Update(int governmentOfficerId, UpdateGovernmentOfficerDto dto)
    {
        await _governmentOfficerService.UpdateAsync(governmentOfficerId, dto);
        return NoContent();
    }

    [Authorize(Policy = AuthorizationPolicies.AdminAccess)]
    [ServiceFilter(typeof(AuditDeleteFilter))]
    [HttpDelete("Delete/{governmentOfficerId}")]
    public async Task<ActionResult> Delete(int governmentOfficerId)
    {
        await _governmentOfficerService.DeleteAsync(governmentOfficerId);
        return NoContent();
    }
}