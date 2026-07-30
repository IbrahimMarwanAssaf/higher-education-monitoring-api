using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Students;
using UNIOOP.App.Models;
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
        StudentResponseDto? student = await _studentService.GetSingleAsync(studentId);

        if (student is null)
        {
            return NotFound(new
            {
                message = $"Student with ID: {studentId} was not found."
            });
        }

        return Ok(student);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<Student>> Create(CreateStudentDto dto)
    {
        StudentResponseDto? student = await _studentService.CreateAsync(dto);

        if (student is null)
        {
            return BadRequest(new
            {
                message = "Unable to create student."
            });
        }

        return CreatedAtAction(nameof(GetSingle), new { studentId = student.StudentID }, student);
    }

    [HttpPut("Update/{studentId}")]
    public async Task<ActionResult> Update(int studentId, UpdateStudentDto dto)
    {
        StudentResponseDto? existingStudent = await _studentService.GetSingleAsync(studentId);

        if (existingStudent is null)
        {
            return NotFound(new
            {
                message = $"Student with ID: {studentId} was not found."
            });
        }

        await _studentService.UpdateAsync(studentId, dto);

        return NoContent();
    }

    [HttpDelete("Delete/{studentId}")]
    public async Task<ActionResult> Delete(int studentId)
    {
        StudentResponseDto? existingStudent = await _studentService.GetSingleAsync(studentId);

        if (existingStudent is null)
        {
            return NotFound(new
            {
                message = $"Student with ID: {studentId} was not found."
            });
        }

        await _studentService.DeleteAsync(studentId);

        return NoContent();
    }

}