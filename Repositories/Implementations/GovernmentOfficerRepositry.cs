using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;

namespace UNIOOP.App.Repositories.Implementations
{
    public class GovernmentOfficerRepository : IGovernmentOfficerRepository
    {
        private readonly DataContextEF _entityFramework;

        public GovernmentOfficerRepository(DataContextEF context)
        {
            _entityFramework = context;
        }
        public async Task<List<GovernmentOfficer>> GetAllAsync()
        {
            return await _entityFramework.GovernmentOfficers.AsNoTracking()
                .OrderBy(s => s.OfficerID).ToListAsync();
        }
        public async Task<GovernmentOfficer?> GetByIdAsync(int OfficerId)
        {
            return await _entityFramework.GovernmentOfficers.AsNoTracking()
               .Where(g => g.OfficerID == OfficerId)
               .SingleOrDefaultAsync();
        }

        public async Task<GovernmentOfficer?> GetByIdForUpdateAsync(int OfficerId)
        {
            return await _entityFramework.GovernmentOfficers.AsTracking()
               .Where(g => g.OfficerID == OfficerId)
               .SingleOrDefaultAsync();
        }

        public async Task AddAsync(GovernmentOfficer governmentOfficer)
        {
            await _entityFramework.GovernmentOfficers.AddAsync(governmentOfficer);
        }

        public void Remove(GovernmentOfficer governmentOfficer)
        {
            _entityFramework.GovernmentOfficers.Remove(governmentOfficer);
        }

        public async Task SaveChangesAsync()
        {
            await _entityFramework.SaveChangesAsync();
        }
    }
}
