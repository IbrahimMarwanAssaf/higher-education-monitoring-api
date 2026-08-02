using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Teachers;
using UNIOOP.App.Helpers;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[ApiController]
[Route("[controller]")]
public class TeacherController : ControllerBase
{
    private readonly ITeacherService _teacherService;
    private readonly IDatabaseValidationHelper _validationHelper;
    public TeacherController(ITeacherService teacherService, IDatabaseValidationHelper validationHelper)
    {
        _teacherService = teacherService;
        _validationHelper = validationHelper;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<TeacherResponseDto>>> GetAll()
    {
        List<TeacherResponseDto> teachers = await _teacherService.GetAllAsync();

        return Ok(teachers);
    }

    [HttpGet("GetSingle/{teacherId}")]
    public async Task<ActionResult<TeacherResponseDto>> GetSingle(int teacherId)
    {
        TeacherResponseDto? teacher = await _teacherService.GetSingleAsync(teacherId);

        if (teacher is null)
        {
            return NotFound(new
            {
                message = $"Teacher with ID: {teacherId} was not found."
            });
        }

        return Ok(teacher);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<TeacherResponseDto>> Create(CreateTeacherDto dto)
    {

        if (await _validationHelper.SSNExistsAsync(dto.SSN))
        {
            return Conflict(new
            {
                message = "The SSN is already in use."
            });
        }

        if (await _validationHelper.TeacherEmailExistsAsync(dto.Email))
        {
            return Conflict(new
            {
                message = "The email is already in use."
            });
        }

        if (!await _validationHelper.UniversityExistsAsync(dto.UniversityID))
        {
            return BadRequest(new
            {
                message = "The selected university does not exist."
            });
        }

        TeacherResponseDto teacher = await _teacherService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetSingle), new { teacherId = teacher.TeacherID }, teacher);
    }

    [HttpPut("Update/{teacherId}")]
    public async Task<ActionResult> Update(int teacherId, UpdateTeacherDto dto)
    {
        if (!await _validationHelper.TeacherExistsAsync(teacherId))
        {
            return NotFound();
        }

        if (await _validationHelper.TeacherEmailExistsAsync(dto.Email, teacherId))
        {
            return Conflict(new
            {
                message =
                    "Another person already uses this email."
            });
        }

        if (!await _validationHelper.UniversityExistsAsync(
                dto.UniversityID))
        {
            return BadRequest(new
            {
                message = "The selected university does not exist."
            });
        }

        bool updated = await _teacherService.UpdateAsync(teacherId, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = $"Teacher with ID: {teacherId} was not found."
            });
        }

        return NoContent();
    }

    [HttpDelete("Delete/{teacherId}")]
    public async Task<ActionResult> Delete(int teacherId)
    {
        if (!await _validationHelper.TeacherExistsAsync(teacherId))
        {
            return NotFound();
        }

        await _teacherService.DeleteAsync(teacherId);

        return NoContent();
    }
}