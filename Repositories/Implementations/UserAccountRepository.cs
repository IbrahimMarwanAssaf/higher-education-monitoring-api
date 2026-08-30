using Microsoft.EntityFrameworkCore;
using UNIOOP.App.Data;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;

namespace UNIOOP.App.Repositories.Implementations
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly DataContextEF _context;

        public UserAccountRepository(DataContextEF context)
        {
            _context = context;
        }

        public async Task<UserAccount?> GetByEmailAsync(string email)
        {
            return await _context.UserAccounts.Include(u => u.Personnel)
                .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Personnel.Email, email));
        }

        public async Task<UserAccount?> GetByPersonnelIdAsync(long personnelId)
        {
            return await _context.UserAccounts.FirstOrDefaultAsync(u => u.PersonnelID == personnelId);
        }

        public async Task<GovernmentOfficer?> GetGovernmentOfficerByEmailAsync(string email)
        {
            return await _context.GovernmentOfficers.FirstOrDefaultAsync(g => EF.Functions.ILike(g.Email, email));
        }

        public async Task AddAsync(UserAccount userAccount)
        {
            await _context.UserAccounts.AddAsync(userAccount);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}