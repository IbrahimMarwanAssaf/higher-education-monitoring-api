using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;

namespace UNIOOP.App.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DataContextEF _entityFramework;

        public StudentRepository(DataContextEF context)
        {
            _entityFramework = context;
        }
        public async Task<List<Student>> GetAllAsync()
        {
            return await _entityFramework.Students.AsNoTracking()
                .Include(s => s.University)
                .OrderBy(s => s.StudentID).ToListAsync();
        }
        public async Task<Student?> GetByIdAsync(int studentId)
        {
            return await _entityFramework.Students.AsNoTracking()
               .Where(student => student.StudentID == studentId)
               .Include(s => s.University)
               .SingleOrDefaultAsync();
        }

        public async Task<Student?> GetByIdForUpdateAsync(int studentId)
        {
            return await _entityFramework.Students.AsTracking()
               .Where(student => student.StudentID == studentId)
               .Include(s => s.University)
               .SingleOrDefaultAsync();
        }

        public async Task AddAsync(Student student)
        {
            await _entityFramework.Students.AddAsync(student);
        }

        public void Remove(Student student)
        {
            _entityFramework.Students.Remove(student);
        }

        public async Task SaveChangesAsync()
        {
            await _entityFramework.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int studentId)
        {
            return await _entityFramework.Students.AnyAsync(s => s.StudentID == studentId);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null)
        {
            return await _entityFramework.Students
                .AnyAsync(s => s.Email == email &&
                    (!excludeStudentId.HasValue ||
                        s.StudentID != excludeStudentId.Value));
        }

        public async Task<bool> HasEnrollmentsAsync(int studentId)
        {
            return await _entityFramework.StudentCourses.AnyAsync(sc =>
                sc.Student.StudentID == studentId);
        }
    }
}
