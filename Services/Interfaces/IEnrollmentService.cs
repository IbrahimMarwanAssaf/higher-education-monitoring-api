using UNIOOP.App.Dtos.Enrollments;

namespace UNIOOP.App.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResponseDto?> GetSingleAsync(int studentId, int courseId);
        Task<List<EnrollmentResponseDto>> GetStudentCoursesAsync(int studentId);
        Task<List<EnrollmentResponseDto>> GetCourseStudentsAsync(int courseId);
        Task<EnrollmentResponseDto> EnrollAsync(CreateEnrollmentDto dto);
        Task<bool> UnenrollAsync(int studentId, int courseId);
    }
}