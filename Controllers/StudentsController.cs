using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Students;
using UNIOOP.App.Helpers;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IDatabaseValidationHelper _validationHelper;
    public StudentController(IStudentService studentService, IDatabaseValidationHelper validationHelper)
    {
        _studentService = studentService;
        _validationHelper = validationHelper;
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
    public async Task<ActionResult<StudentResponseDto>> Create(CreateStudentDto dto)
    {
        if (!await _validationHelper.UniversityExistsAsync(dto.UniversityID))
        {
            return BadRequest(new { message = "The selected university does not exist." });
        }

        if (await _validationHelper.SSNExistsAsync(dto.SSN))
        {
            return Conflict(new { message = "The SSN is already in use." });
        }

        if (await _validationHelper.StudentEmailExistsAsync(dto.Email))
        {
            return Conflict(new { message = "The email is already in use." });
        }

        StudentResponseDto student = await _studentService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetSingle), new { studentId = student.StudentID }, student);
    }

    [HttpPut("Update/{studentId}")]
    public async Task<ActionResult> Update(int studentId, UpdateStudentDto dto)
    {
        if (!await _validationHelper.StudentExistsAsync(studentId))
        {
            return NotFound();
        }

        if (await _validationHelper.StudentEmailExistsAsync(dto.Email, studentId))
        {
            return Conflict(new
            {
                message = "Another person already uses this email."
            });
        }

        if (!await _validationHelper.UniversityExistsAsync(dto.UniversityID))
        {
            return BadRequest(new
            {
                message = "The selected university does not exist."
            });
        }

        bool updated = await _studentService.UpdateAsync(studentId, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = $"Student with ID: {studentId} was not found."
            });
        }

        return NoContent();
    }

    [HttpDelete("Delete/{studentId}")]
    public async Task<ActionResult> Delete(int studentId)
    {
        if (!await _validationHelper.StudentExistsAsync(studentId))
        {
            return NotFound();
        }

        if (await _validationHelper.StudentHasEnrollmentsAsync(studentId))
        {
            return Conflict(new
            {
                message = "The student cannot be deleted while enrolled in courses."
            });
        }

        bool deleted = await _studentService.DeleteAsync(studentId);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Student with ID: {studentId} was not found."
            });
        }

        return NoContent();
    }

}