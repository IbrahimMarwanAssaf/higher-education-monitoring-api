using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Students;
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

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<StudentResponseDto>>> GetAll()
    {
        List<StudentResponseDto> students = await _studentService.GetAllAsync();
        return Ok(students);
    }

    [HttpGet("GetSingle/{studentId}")]
    public async Task<ActionResult<StudentResponseDto>> GetSingle(int studentId)
    {
        StudentResponseDto student = await _studentService.GetSingleAsync(studentId);
        return Ok(student);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<StudentResponseDto>> Create(CreateStudentDto dto)
    {
        StudentResponseDto student = await _studentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetSingle), new { studentId = student.StudentID }, student);
    }

    [HttpPut("Update/{studentId}")]
    public async Task<ActionResult> Update(int studentId, UpdateStudentDto dto)
    {
        await _studentService.UpdateAsync(studentId, dto);
        return NoContent();
    }

    [HttpDelete("Delete/{studentId}")]
    public async Task<ActionResult> Delete(int studentId)
    {
        await _studentService.DeleteAsync(studentId);
        return NoContent();
    }
}