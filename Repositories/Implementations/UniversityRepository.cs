using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
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
            return await _entityFramework.Universities
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

    }
}