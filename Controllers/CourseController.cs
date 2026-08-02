using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Courses;
using UNIOOP.App.Helpers;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[ApiController]
[Route("[controller]")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly IDatabaseValidationHelper _validationHelper;
    public CourseController(ICourseService courseService, IDatabaseValidationHelper validationHelper)
    {
        _courseService = courseService;
        _validationHelper = validationHelper;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<List<CourseResponseDto>>> GetAll()
    {
        List<CourseResponseDto> courses = await _courseService.GetAllAsync();

        return Ok(courses);
    }

    [HttpGet("GetSingle/{courseId}")]
    public async Task<ActionResult<CourseResponseDto>> GetSingle(int courseId)
    {
        CourseResponseDto? course = await _courseService.GetSingleAsync(courseId);

        if (course is null)
        {
            return NotFound(new
            {
                message = $"Course with ID: {courseId} was not found."
            });
        }

        return Ok(course);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<CourseResponseDto>> Create(CreateCourseDto dto)
    {
        if (!await _validationHelper.UniversityExistsAsync(dto.UniversityID))
        {
            return BadRequest(new
            {
                message = "The selected university does not exist."
            });
        }

        if (dto.TeacherID.HasValue)
        {
            if (!await _validationHelper.TeacherExistsAsync(dto.TeacherID.Value))
            {
                return BadRequest(new
                {
                    message = "The selected teacher does not exist."
                });
            }

            if (!await _validationHelper.TeacherBelongsToUniversityAsync(dto.TeacherID.Value, dto.UniversityID))
            {
                return BadRequest(new
                {
                    message = "The teacher does not belong to this university."
                });
            }
        }

        if (await _validationHelper.CourseNameExistsAsync(dto.CourseName, dto.UniversityID))
        {
            return Conflict(new
            {
                message = "This course name already exists in the university."
            });
        }

        CourseResponseDto course = await _courseService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetSingle), new { courseId = course.CourseID }, course);
    }

    [HttpPut("Update/{courseId}")]
    public async Task<ActionResult> Update(int courseId, UpdateCourseDto dto)
    {
        if (!await _validationHelper.CourseExistsAsync(courseId))
        {
            return NotFound();
        }

        if (!await _validationHelper.UniversityExistsAsync(dto.UniversityID))
        {
            return BadRequest(new
            {
                message = "The selected university does not exist."
            });
        }

        if (dto.TeacherID.HasValue)
        {
            if (!await _validationHelper.TeacherExistsAsync(dto.TeacherID.Value))
            {
                return BadRequest(new
                {
                    message = "The selected teacher does not exist."
                });
            }

            if (!await _validationHelper.TeacherBelongsToUniversityAsync(dto.TeacherID.Value, dto.UniversityID))
            {
                return BadRequest(new
                {
                    message = "The teacher does not belong to this university."
                });
            }
        }

        if (await _validationHelper.CourseNameExistsAsync(dto.CourseName, dto.UniversityID, courseId))
        {
            return Conflict(new
            {
                message = "Another course already uses this name."
            });
        }

        bool updated = await _courseService.UpdateAsync(courseId, dto);

        if (!updated)
        {
            return NotFound(new
            {
                message = $"Course with ID: {courseId} was not found."
            });
        }

        return NoContent();
    }

    [HttpDelete("Delete/{courseId}")]
    public async Task<ActionResult> Delete(int courseId)
    {
        if (!await _validationHelper.CourseExistsAsync(courseId))
        {
            return NotFound();
        }

        if (await _validationHelper.CourseHasEnrollmentsAsync(courseId))
        {
            return Conflict(new
            {
                message = "The course cannot be deleted while students are enrolled."
            });
        }

        bool deleted = await _courseService.DeleteAsync(courseId);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Course with ID: {courseId} was not found."
            });
        }

        return NoContent();
    }

}