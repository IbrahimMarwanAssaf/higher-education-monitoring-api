using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Helpers;

namespace UNIOOP.App.Helpers
{
    public class DatabaseValidationHelper : IDatabaseValidationHelper
    {
        private readonly DataContextEF _entityFramework;

        public DatabaseValidationHelper(DataContextEF context)
        {
            _entityFramework = context;
        }

        public async Task<bool> UniversityExistsAsync(int universityId)
        {
            return await _entityFramework.Universities.AnyAsync(u => u.UniversityID == universityId);
        }

        public async Task<bool> StudentExistsAsync(int studentId)
        {
            return await _entityFramework.Students.AnyAsync(s => s.StudentID == studentId);
        }

        public async Task<bool> TeacherExistsAsync(int teacherId)
        {
            return await _entityFramework.Teachers.AnyAsync(t => t.TeacherID == teacherId);
        }

        public async Task<bool> GovernmentOfficerExistsAsync(int officerId)
        {
            return await _entityFramework.GovernmentOfficers.AnyAsync(g => g.OfficerID == officerId);
        }

        public async Task<bool> CourseExistsAsync(int courseId)
        {
            return await _entityFramework.Courses.AnyAsync(c => c.CourseID == courseId);
        }

        public async Task<bool> UniversityNameExistsAsync(string universityName, int? excludeUniversityId = null)
        {
            string normalizedName = InputNormalizationHelper
                .NormalizeText(universityName)
                .ToLowerInvariant();
            return await _entityFramework.Universities.AnyAsync(u => u.UniversityName.ToLower() == normalizedName &&
                (!excludeUniversityId.HasValue || u.UniversityID != excludeUniversityId.Value));
        }

        public async Task<bool> SSNExistsAsync(string ssn)
        {
            string normalizedSsn = InputNormalizationHelper.NormalizeText(ssn);
            return await _entityFramework.Personnels.AnyAsync(p => p.SSN == normalizedSsn);
        }

        public async Task<bool> StudentEmailExistsAsync(string email, int? excludeStudentId = null)
        {
            long? excludedPersonnelId = null;

            if (excludeStudentId.HasValue)
            {
                excludedPersonnelId = await _entityFramework.Students
                    .Where(s => s.StudentID == excludeStudentId.Value)
                    .Select(s => (long?)s.PersonnelID)
                    .SingleOrDefaultAsync();
            }

            return await EmailExistsAsync(email, excludedPersonnelId);
        }

        public async Task<bool> TeacherEmailExistsAsync(string email, int? excludeTeacherId = null)
        {
            long? excludedPersonnelId = null;

            if (excludeTeacherId.HasValue)
            {
                excludedPersonnelId = await _entityFramework.Teachers
                    .Where(t => t.TeacherID == excludeTeacherId.Value)
                    .Select(t => (long?)t.PersonnelID)
                    .SingleOrDefaultAsync();
            }

            return await EmailExistsAsync(email, excludedPersonnelId);
        }

        public async Task<bool> GovernmentOfficerEmailExistsAsync(string email, int? excludeOfficerId = null)
        {
            long? excludedPersonnelId = null;

            if (excludeOfficerId.HasValue)
            {
                excludedPersonnelId = await _entityFramework.GovernmentOfficers
                    .Where(o => o.OfficerID == excludeOfficerId.Value)
                    .Select(o => (long?)o.PersonnelID)
                    .SingleOrDefaultAsync();
            }

            return await EmailExistsAsync(email, excludedPersonnelId);
        }

        private async Task<bool> EmailExistsAsync(string email, long? excludePersonnelId)
        {
            string normalizedEmail = InputNormalizationHelper.NormalizeEmail(email);
            return await _entityFramework.Personnels.AnyAsync(p => p.Email.ToLower() == normalizedEmail &&
                (!excludePersonnelId.HasValue || p.PersonnelID != excludePersonnelId.Value));
        }

        public async Task<bool> CourseNameExistsAsync(string courseName, int universityId, int? excludeCourseId = null)
        {
            string normalizedName = InputNormalizationHelper
                .NormalizeText(courseName)
                .ToLowerInvariant();
            return await _entityFramework.Courses.AnyAsync(c => c.CourseName.ToLower() == normalizedName &&
                c.UniversityID == universityId && (!excludeCourseId.HasValue || c.CourseID != excludeCourseId.Value));
        }

        public async Task<bool> TeacherBelongsToUniversityAsync(int teacherId, int universityId)
        {
            return await _entityFramework.Teachers.AnyAsync(t => t.TeacherID == teacherId && t.UniversityID == universityId);
        }

        public async Task<bool> StudentAndCourseSameUniversityAsync(int studentId, int courseId)
        {
            return await (
                from student in _entityFramework.Students
                join course in _entityFramework.Courses
                    on student.UniversityID
                    equals course.UniversityID
                where student.StudentID == studentId && course.CourseID == courseId
                select student.PersonnelID
            ).AnyAsync();
        }

        public async Task<bool> EnrollmentExistsAsync(int studentId, int courseId)
        {
            return await (
                from enrollment in _entityFramework.StudentCourses
                join student in _entityFramework.Students
                    on enrollment.StudentPersonnelID
                    equals student.PersonnelID
                where student.StudentID == studentId && enrollment.CourseID == courseId
                select enrollment
            ).AnyAsync();
        }

        public async Task<bool> UniversityHasDependenciesAsync(int universityId)
        {
            bool hasStudents = await _entityFramework.Students.AnyAsync(s => s.UniversityID == universityId);

            if (hasStudents)
            {
                return true;
            }

            bool hasTeachers = await _entityFramework.Teachers.AnyAsync(t => t.UniversityID == universityId);

            if (hasTeachers)
            {
                return true;
            }

            return await _entityFramework.Courses.AnyAsync(c => c.UniversityID == universityId);
        }

        public async Task<bool> StudentHasEnrollmentsAsync(int studentId)
        {
            return await (
                from enrollment in _entityFramework.StudentCourses
                join student in _entityFramework.Students
                    on enrollment.StudentPersonnelID
                    equals student.PersonnelID
                where student.StudentID == studentId
                select enrollment
            ).AnyAsync();
        }

        public async Task<bool> CourseHasEnrollmentsAsync(int courseId)
        {
            return await _entityFramework.StudentCourses.AnyAsync(sc => sc.CourseID == courseId);
        }
    }
}