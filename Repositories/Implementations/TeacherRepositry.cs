using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;

namespace UNIOOP.App.Repositories.Implementations
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly DataContextEF _entityFramework;

        public TeacherRepository(DataContextEF context)
        {
            _entityFramework = context;
        }
        public async Task<List<Teacher>> GetAllAsync()
        {
            return await _entityFramework.Teachers.AsNoTracking()
                .Include(s => s.University)
                .OrderBy(s => s.TeacherID).ToListAsync();
        }
        public async Task<Teacher?> GetByIdAsync(int teacherId)
        {
            return await _entityFramework.Teachers.AsNoTracking()
               .Where(teacher => teacher.TeacherID == teacherId)
               .Include(s => s.University)
               .SingleOrDefaultAsync();
        }

        public async Task<Teacher?> GetByIdForUpdateAsync(int teacherId)
        {
            return await _entityFramework.Teachers.AsTracking()
               .Where(teacher => teacher.TeacherID == teacherId)
               .Include(s => s.University)
               .SingleOrDefaultAsync();
        }

        public async Task AddAsync(Teacher teacher)
        {
            await _entityFramework.Teachers.AddAsync(teacher);
        }

        public void Remove(Teacher teacher)
        {
            _entityFramework.Teachers.Remove(teacher);
        }

        public async Task SaveChangesAsync()
        {
            await _entityFramework.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int teacherId)
        {
            return await _entityFramework.Teachers.AnyAsync(t => t.TeacherID == teacherId);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeTeacherId = null)
        {
            return await _entityFramework.Teachers
                .AnyAsync(t =>
                    t.Email == email &&
                    (!excludeTeacherId.HasValue ||
                     t.TeacherID != excludeTeacherId.Value));
        }

        public async Task<bool> BelongsToUniversityAsync(int teacherId, int universityId)
        {
            return await _entityFramework.Teachers.AnyAsync(t => t.TeacherID == teacherId && t.UniversityID == universityId);
        }

    }
}
