using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Courses;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.APP.Controllers;

[ApiController]
[Route("[controller]")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
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
        CourseResponseDto course = await _courseService.GetSingleAsync(courseId);
        return Ok(course);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<CourseResponseDto>> Create(CreateCourseDto dto)
    {
        CourseResponseDto course = await _courseService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetSingle), new { courseId = course.CourseID }, course);
    }

    [HttpPut("Update/{courseId}")]
    public async Task<ActionResult> Update(int courseId, UpdateCourseDto dto)
    {
        await _courseService.UpdateAsync(courseId, dto);
        return NoContent();
    }

    [HttpDelete("Delete/{courseId}")]
    public async Task<ActionResult> Delete(int courseId)
    {
        await _courseService.DeleteAsync(courseId);
        return NoContent();
    }
}