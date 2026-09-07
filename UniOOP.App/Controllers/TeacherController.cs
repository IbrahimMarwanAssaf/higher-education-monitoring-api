using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Constants;
using UNIOOP.App.Dtos.Teachers;
using UNIOOP.App.Filters;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[ApiController]
[Route("[controller]")]
public class TeacherController : ControllerBase
{
    private readonly ITeacherService _teacherService;
    public TeacherController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    [Authorize(Policy = AuthorizationPolicies.UserAccess)]
    [HttpGet("GetAll")]
    public async Task<ActionResult<List<TeacherResponseDto>>> GetAll()
    {
        List<TeacherResponseDto> teachers = await _teacherService.GetAllAsync();
        return Ok(teachers);
    }

    [Authorize(Policy = AuthorizationPolicies.UserAccess)]
    [HttpGet("GetSingle/{teacherId}")]
    public async Task<ActionResult<TeacherResponseDto>> GetSingle(int teacherId)
    {
        TeacherResponseDto teacher = await _teacherService.GetSingleAsync(teacherId);
        return Ok(teacher);
    }

    [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
    [HttpPost("Create")]
    public async Task<ActionResult<TeacherResponseDto>> Create(CreateTeacherDto dto)
    {
        TeacherResponseDto teacher = await _teacherService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetSingle), new { teacherId = teacher.TeacherID }, teacher);
    }

    [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
    [HttpPut("Update/{teacherId}")]
    public async Task<ActionResult> Update(int teacherId, UpdateTeacherDto dto)
    {
        await _teacherService.UpdateAsync(teacherId, dto);
        return NoContent();
    }

    [Authorize(Policy = AuthorizationPolicies.AdminAccess)]
    [ServiceFilter(typeof(AuditDeleteFilter))]
    [HttpDelete("Delete/{teacherId}")]
    public async Task<ActionResult> Delete(int teacherId)
    {
        await _teacherService.DeleteAsync(teacherId);
        return NoContent();
    }
}