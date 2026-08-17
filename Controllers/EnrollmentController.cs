using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Enrollments;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet("GetSingle/{studentId}/{courseId}")]
        public async Task<ActionResult<EnrollmentResponseDto>> GetSingle(int studentId, int courseId)
        {
            EnrollmentResponseDto enrollment = await _enrollmentService.GetSingleAsync(studentId, courseId);
            return Ok(enrollment);
        }

        [HttpGet("GetStudentCourses/{studentId}")]
        public async Task<ActionResult<List<EnrollmentResponseDto>>> GetStudentCourses(int studentId)
        {
            List<EnrollmentResponseDto> enrollments = await _enrollmentService.GetStudentCoursesAsync(studentId);
            return Ok(enrollments);
        }

        [HttpGet("GetCourseStudents/{courseId}")]
        public async Task<ActionResult<List<EnrollmentResponseDto>>> GetCourseStudents(int courseId)
        {
            List<EnrollmentResponseDto> enrollments = await _enrollmentService.GetCourseStudentsAsync(courseId);
            return Ok(enrollments);
        }

        [HttpPost("Enroll")]
        public async Task<ActionResult<EnrollmentResponseDto>> Enroll(CreateEnrollmentDto dto)
        {
            EnrollmentResponseDto enrollment = await _enrollmentService.EnrollAsync(dto);
            return CreatedAtAction(
                nameof(GetSingle),
                new { studentId = enrollment.StudentID, courseId = enrollment.CourseID }, enrollment);
        }

        [HttpDelete("Unenroll/{studentId}/{courseId}")]
        public async Task<IActionResult> Unenroll(int studentId, int courseId)
        {
            await _enrollmentService.UnenrollAsync(studentId, courseId);
            return NoContent();
        }
    }
}