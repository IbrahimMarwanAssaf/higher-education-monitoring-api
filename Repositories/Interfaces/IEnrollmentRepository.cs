using UNIOOP.App.Dtos.Enrollments;
using UNIOOP.App.Models;

namespace UNIOOP.App.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<StudentCourse?> GetSingleAsync(int studentId, int courseId);
        Task<List<StudentCourse>> GetStudentCoursesAsync(int studentId);
        Task<List<StudentCourse>> GetCourseStudentsAsync(int courseId);
        Task AddAsync(StudentCourse enrollment);
        void Remove(StudentCourse enrollment);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(int studentId, int courseId);
        Task<bool> StudentAndCourseSameUniversityAsync(int studentId, int courseId);
    }
}