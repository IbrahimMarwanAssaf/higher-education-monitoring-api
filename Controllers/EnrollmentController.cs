using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Enrollments;
using UNIOOP.App.Helpers;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IDatabaseValidationHelper _validationHelper;

        public EnrollmentController(IEnrollmentService enrollmentService, IDatabaseValidationHelper validationHelper)
        {
            _enrollmentService = enrollmentService;
            _validationHelper = validationHelper;
        }

        [HttpGet("GetById/{studentId}/{courseId}")]
        public async Task<ActionResult<EnrollmentResponseDto>> GetById(int studentId, int courseId)
        {
            EnrollmentResponseDto? enrollment = await _enrollmentService.GetSingleAsync(studentId, courseId);

            if (enrollment == null)
            {
                return NotFound(new
                {
                    message = "Enrollment was not found."
                });
            }

            return Ok(enrollment);
        }

        [HttpGet("GetStudentCourses/{studentId}")]
        public async Task<ActionResult<List<EnrollmentResponseDto>>> GetStudentCourses(int studentId)
        {
            if (!await _validationHelper.StudentExistsAsync(studentId))
            {
                return NotFound(new
                {
                    message = "The student does not exist."
                });
            }

            List<EnrollmentResponseDto> enrollments = await _enrollmentService.GetStudentCoursesAsync(studentId);

            return Ok(enrollments);
        }

        [HttpGet("GetCourseStudents/{courseId}")]
        public async Task<ActionResult<List<EnrollmentResponseDto>>> GetCourseStudents(int courseId)
        {
            if (!await _validationHelper.CourseExistsAsync(courseId))
            {
                return NotFound(new
                {
                    message = "The course does not exist."
                });
            }

            List<EnrollmentResponseDto> enrollments = await _enrollmentService.GetCourseStudentsAsync(courseId);

            return Ok(enrollments);
        }

        [HttpPost("Enroll")]
        public async Task<ActionResult<EnrollmentResponseDto>> Enroll(CreateEnrollmentDto dto)
        {
            if (!await _validationHelper.StudentExistsAsync(dto.StudentID))
            {
                return NotFound(new
                {
                    message = "The student does not exist."
                });
            }

            if (!await _validationHelper.CourseExistsAsync(dto.CourseID))
            {
                return NotFound(new
                {
                    message = "The course does not exist."
                });
            }

            if (!await _validationHelper.StudentAndCourseSameUniversityAsync(dto.StudentID, dto.CourseID))
            {
                return BadRequest(new
                {
                    message = "The student and course must belong to the same university."
                });
            }

            if (await _validationHelper.EnrollmentExistsAsync(dto.StudentID, dto.CourseID))
            {
                return Conflict(new
                {
                    message = "The student is already enrolled in this course."
                });
            }

            EnrollmentResponseDto enrollment = await _enrollmentService.EnrollAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    studentId = enrollment.StudentID,
                    courseId = enrollment.CourseID
                }, enrollment);
        }

        [HttpDelete("Unenroll/{studentId}/{courseId}")]
        public async Task<IActionResult> Unenroll(int studentId, int courseId)
        {
            if (!await _validationHelper.EnrollmentExistsAsync(studentId, courseId))
            {
                return NotFound(new
                {
                    message = "The enrollment does not exist."
                });
            }

            await _enrollmentService.UnenrollAsync(studentId, courseId);

            return NoContent();
        }
    }
}