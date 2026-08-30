using UNIOOP.App.Models;

namespace UNIOOP.App.Repositories.Interfaces
{
    public interface IUserAccountRepository
    {
        Task<UserAccount?> GetByPersonnelIdAsync(long personnelId);
        Task<GovernmentOfficer?> GetGovernmentOfficerByEmailAsync(string email);
        Task<UserAccount?> GetByEmailAsync(string email);
        Task AddAsync(UserAccount userAccount);
        Task SaveChangesAsync();
    }
}