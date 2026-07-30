using UNIOOP.App.DTOs.Enrollments;

namespace UNIOOP.App.Services.Interfaces
{
    public interface IEnrollmentService
    {
        // Task<bool> StudentExistsAsync(int studentId);
        // Task<bool> CourseExistsAsync(int courseId);
        // Task<bool> StudentAndCourseSameUniversityAsync(int studentId, int courseId);
        // Task<bool> EnrollmentExistsAsync(int studentId, int courseId);
        Task<EnrollmentResponseDto?> GetSingleAsync(int studentId, int courseId);
        Task<List<EnrollmentResponseDto>> GetStudentCoursesAsync(int studentId);
        Task<List<EnrollmentResponseDto>> GetCourseStudentsAsync(int courseId);
        Task<EnrollmentResponseDto> EnrollAsync(CreateEnrollmentDto dto);
        Task<bool> UnenrollAsync(int studentId, int courseId);
    }
}