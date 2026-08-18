using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Repositories.Interfaces;

namespace UNIOOP.App.Repositories.Implementations
{
    public class PersonnelRepository : IPersonnelRepository
    {
        private readonly DataContextEF _entityFramework;

        public PersonnelRepository(DataContextEF context)
        {
            _entityFramework = context;
        }

        public async Task<bool> SSNExistsAsync(string ssn)
        {
            return await _entityFramework.Personnels
                .AnyAsync(p => p.SSN == ssn);
        }
    }
}