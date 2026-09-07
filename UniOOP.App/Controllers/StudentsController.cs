using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Constants;
using UNIOOP.App.Dtos.Students;
using UNIOOP.App.Filters;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;
    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [Authorize(Policy = AuthorizationPolicies.UserAccess)]
    [HttpGet("GetAll")]
    public async Task<ActionResult<List<StudentResponseDto>>> GetAll()
    {
        List<StudentResponseDto> students = await _studentService.GetAllAsync();
        return Ok(students);
    }

    [Authorize(Policy = AuthorizationPolicies.UserAccess)]
    [HttpGet("GetSingle/{studentId}")]
    public async Task<ActionResult<StudentResponseDto>> GetSingle(int studentId)
    {
        StudentResponseDto student = await _studentService.GetSingleAsync(studentId);
        return Ok(student);
    }

    [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
    [HttpPost("Create")]
    public async Task<ActionResult<StudentResponseDto>> Create(CreateStudentDto dto)
    {
        StudentResponseDto student = await _studentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetSingle), new { studentId = student.StudentID }, student);
    }

    [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
    [HttpPut("Update/{studentId}")]
    public async Task<ActionResult> Update(int studentId, UpdateStudentDto dto)
    {
        await _studentService.UpdateAsync(studentId, dto);
        return NoContent();
    }

    [Authorize(Policy = AuthorizationPolicies.AdminAccess)]
    [ServiceFilter(typeof(AuditDeleteFilter))]
    [HttpDelete("Delete/{studentId}")]
    public async Task<ActionResult> Delete(int studentId)
    {
        await _studentService.DeleteAsync(studentId);
        return NoContent();
    }
}
