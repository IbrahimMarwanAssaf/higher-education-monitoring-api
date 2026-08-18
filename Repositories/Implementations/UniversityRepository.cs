using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;

namespace UNIOOP.App.Repositories.Implementations
{
    public class UniversityRepository : IUniversityRepository
    {
        private readonly DataContextEF _entityFramework;

        public UniversityRepository(DataContextEF context)
        {
            _entityFramework = context;
        }

        public async Task<List<University>> GetAllAsync()
        {
            return await _entityFramework.Universities.AsNoTracking().ToListAsync();
        }

        public async Task<University?> GetByIdAsync(int universityId)
        {
            return await _entityFramework.Universities.AsNoTracking()
                .SingleOrDefaultAsync(u => u.UniversityID == universityId);
        }

        public async Task<University?> GetByIdForUpdateAsync(int universityId)
        {
            return await _entityFramework.Universities.AsTracking()
                .SingleOrDefaultAsync(u => u.UniversityID == universityId);
        }

        public async Task AddAsync(University university)
        {
            await _entityFramework.Universities.AddAsync(university);
        }

        public void Remove(University university)
        {
            _entityFramework.Universities.Remove(university);
        }

        public async Task SaveChangesAsync()
        {
            await _entityFramework.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int universityId)
        {
            return await _entityFramework.Universities.AnyAsync(u => u.UniversityID == universityId);
        }

        public async Task<bool> NameExistsAsync(string universityName, int? excludeUniversityId = null)
        {
            string normalizedName = InputNormalizationHelper
                .NormalizeText(universityName)
                .ToLowerInvariant();
            return await _entityFramework.Universities.AnyAsync(u => u.UniversityName.ToLower() == normalizedName &&
                (!excludeUniversityId.HasValue || u.UniversityID != excludeUniversityId.Value));
        }

        public async Task<bool> HasDependenciesAsync(int universityId)
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

    }
}