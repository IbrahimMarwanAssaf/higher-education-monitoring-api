using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;

namespace UNIOOP.App.Repositories.Implementations
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly DataContextEF _entityFramework;

        public EnrollmentRepository(DataContextEF context)
        {
            _entityFramework = context;
        }

        public async Task<StudentCourse?> GetSingleAsync(int studentId, int courseId)
        {
            return await GetEnrollmentQuery()
                .Where(e => e.Student.StudentID == studentId && e.Course.CourseID == courseId)
                .SingleOrDefaultAsync();
        }
        public async Task<List<StudentCourse>> GetStudentCoursesAsync(int studentId)
        {
            return await GetEnrollmentQuery()
                .Where(enrollment => enrollment.Student.StudentID == studentId)
                .OrderBy(enrollment => enrollment.Course.CourseName)
                .ToListAsync();
        }
        public async Task<List<StudentCourse>> GetCourseStudentsAsync(int courseId)
        {
            return await GetEnrollmentQuery()
                .Where(enrollment => enrollment.Course.CourseID == courseId)
                .OrderBy(enrollment => enrollment.Student.StudentID)
                .ToListAsync();
        }

        public IQueryable<StudentCourse> GetEnrollmentQuery()
        {
            return _entityFramework.StudentCourses
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Course)
                .ThenInclude(c => c.University);
        }

        public async Task AddAsync(StudentCourse enrollment)
        {
            await _entityFramework.StudentCourses.AddAsync(enrollment);
        }
        public void Remove(StudentCourse enrollment)
        {
            _entityFramework.StudentCourses.Remove(enrollment);
        }
        public async Task SaveChangesAsync()
        {
            await _entityFramework.SaveChangesAsync();
        }
    }
}