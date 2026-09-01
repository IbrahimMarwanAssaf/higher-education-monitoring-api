using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Constants;
using UNIOOP.App.Dtos.Universities;
using UNIOOP.App.Filters;
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

    [Authorize(Policy = AuthorizationPolicies.UserAccess)]
    [HttpGet("GetAll")]
    public async Task<ActionResult<List<UniversityResponseDto>>> GetAll()
    {
        List<UniversityResponseDto> universities = await _universityService.GetAllAsync();
        return Ok(universities);
    }

    [Authorize(Policy = AuthorizationPolicies.UserAccess)]
    [HttpGet("GetSingle/{universityId}")]
    public async Task<ActionResult<UniversityResponseDto>> GetSingle(int universityId)
    {
        UniversityResponseDto university = await _universityService.GetSingleAsync(universityId);
        return Ok(university);
    }

    [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
    [HttpPost("Create")]
    public async Task<ActionResult<UniversityResponseDto>> Create(UniversityCreateUpdateDto dto)
    {
        UniversityResponseDto university = await _universityService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetSingle), new { universityId = university.UniversityID }, university);
    }

    [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
    [HttpPut("Update/{universityId}")]
    public async Task<ActionResult> Update(int universityId, UniversityCreateUpdateDto dto)
    {
        await _universityService.UpdateAsync(universityId, dto);
        return NoContent();
    }

    [Authorize(Policy = AuthorizationPolicies.AdminAccess)]
    [ServiceFilter(typeof(AuditDeleteFilter))]
    [HttpDelete("Delete/{universityId}")]
    public async Task<IActionResult> Delete(int universityId)
    {
        await _universityService.DeleteAsync(universityId);
        return NoContent();
    }
}