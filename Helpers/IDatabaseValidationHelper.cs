namespace UNIOOP.App.Helpers
{
    public interface IDatabaseValidationHelper
    {
        Task<bool> UniversityExistsAsync(int universityId);
        Task<bool> StudentExistsAsync(int studentId);
        Task<bool> TeacherExistsAsync(int teacherId);
        Task<bool> GovernmentOfficerExistsAsync(int officerId);
        Task<bool> CourseExistsAsync(int courseId);

        Task<bool> UniversityNameExistsAsync(string universityName, int? universityId = null);

        Task<bool> SSNExistsAsync(string ssn);

        Task<bool> StudentEmailExistsAsync(string email, int? excludeStudentId = null);

        Task<bool> TeacherEmailExistsAsync(string email, int? excludeTeacherId = null);

        Task<bool> GovernmentOfficerEmailExistsAsync(string email, int? excludeOfficerId = null);

        Task<bool> CourseNameExistsAsync(string courseName, int universityId, int? excludeCourseId = null);

        Task<bool> TeacherBelongsToUniversityAsync(int teacherId, int universityId);

        Task<bool> StudentAndCourseSameUniversityAsync(int studentId, int courseId);

        Task<bool> EnrollmentExistsAsync(int studentId, int courseId);

        Task<bool> UniversityHasDependenciesAsync(int universityId);
        Task<bool> StudentHasEnrollmentsAsync(int studentId);
        Task<bool> CourseHasEnrollmentsAsync(int courseId);
    }
}