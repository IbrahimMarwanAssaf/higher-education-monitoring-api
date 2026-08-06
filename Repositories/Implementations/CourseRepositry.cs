using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;

namespace UNIOOP.App.Repositories.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly DataContextEF _entityFramework;

        public CourseRepository(DataContextEF context)
        {
            _entityFramework = context;
        }
        public async Task<List<Course>> GetAllAsync()
        {
            return await _entityFramework.Courses.AsNoTracking()
                .Include(s => s.University)
                .Include(s => s.Teacher)
                .OrderBy(s => s.CourseID).ToListAsync();
        }
        public async Task<Course?> GetByIdAsync(int courseId)
        {
            return await _entityFramework.Courses.AsNoTracking()
               .Where(course => course.CourseID == courseId)
               .Include(s => s.University)
               .Include(s => s.Teacher)
               .SingleOrDefaultAsync();
        }

        public async Task<Course?> GetByIdForUpdateAsync(int courseId)
        {
            return await _entityFramework.Courses.AsTracking()
               .Where(course => course.CourseID == courseId)
               .Include(s => s.University)
               .Include(s => s.Teacher)
               .SingleOrDefaultAsync();
        }

        public async Task AddAsync(Course course)
        {
            await _entityFramework.Courses.AddAsync(course);
        }

        public void Remove(Course course)
        {
            _entityFramework.Courses.Remove(course);
        }

        public async Task SaveChangesAsync()
        {
            await _entityFramework.SaveChangesAsync();
        }
    }
}
