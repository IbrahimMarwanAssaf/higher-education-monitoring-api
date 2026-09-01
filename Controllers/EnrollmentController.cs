using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Enrollments;
using UNIOOP.App.Filters;
using UNIOOP.App.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using UNIOOP.App.Constants;

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

        [Authorize(Policy = AuthorizationPolicies.UserAccess)]
        [HttpGet("GetSingle/{studentId}/{courseId}")]
        public async Task<ActionResult<EnrollmentResponseDto>> GetSingle(int studentId, int courseId)
        {
            EnrollmentResponseDto enrollment = await _enrollmentService.GetSingleAsync(studentId, courseId);
            return Ok(enrollment);
        }

        [Authorize(Policy = AuthorizationPolicies.UserAccess)]
        [HttpGet("GetStudentCourses/{studentId}")]
        public async Task<ActionResult<List<EnrollmentResponseDto>>> GetStudentCourses(int studentId)
        {
            List<EnrollmentResponseDto> enrollments = await _enrollmentService.GetStudentCoursesAsync(studentId);
            return Ok(enrollments);
        }

        [Authorize(Policy = AuthorizationPolicies.UserAccess)]
        [HttpGet("GetCourseStudents/{courseId}")]
        public async Task<ActionResult<List<EnrollmentResponseDto>>> GetCourseStudents(int courseId)
        {
            List<EnrollmentResponseDto> enrollments = await _enrollmentService.GetCourseStudentsAsync(courseId);
            return Ok(enrollments);
        }

        [Authorize(Policy = AuthorizationPolicies.ManagerAccess)]
        [HttpPost("Enroll")]
        public async Task<ActionResult<EnrollmentResponseDto>> Enroll(CreateEnrollmentDto dto)
        {
            EnrollmentResponseDto enrollment = await _enrollmentService.EnrollAsync(dto);
            return CreatedAtAction(
                nameof(GetSingle),
                new { studentId = enrollment.StudentID, courseId = enrollment.CourseID }, enrollment);
        }

        [Authorize(Policy = AuthorizationPolicies.AdminAccess)]
        [ServiceFilter(typeof(AuditDeleteFilter))]
        [HttpDelete("Unenroll/{studentId}/{courseId}")]
        public async Task<IActionResult> Unenroll(int studentId, int courseId)
        {
            await _enrollmentService.UnenrollAsync(studentId, courseId);
            return NoContent();
        }
    }
}