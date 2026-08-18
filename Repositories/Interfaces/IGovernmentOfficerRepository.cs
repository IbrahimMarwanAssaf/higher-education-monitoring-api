using UNIOOP.App.Models;

namespace UNIOOP.App.Repositories.Interfaces
{
    public interface IGovernmentOfficerRepository
    {
        Task<List<GovernmentOfficer>> GetAllAsync();
        Task<GovernmentOfficer?> GetByIdAsync(int OfficerID);
        Task<GovernmentOfficer?> GetByIdForUpdateAsync(int OfficerID);
        Task AddAsync(GovernmentOfficer governmentOfficer);
        void Remove(GovernmentOfficer governmentOfficer);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(int governmentOfficerId);
    }
}