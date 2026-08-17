using UNIOOP.App.Models;

namespace UNIOOP.App.Repositories.Interfaces
{
    public interface IUniversityRepository
    {
        Task<List<University>> GetAllAsync();
        Task<University?> GetByIdAsync(int universityId);
        Task<University?> GetByIdForUpdateAsync(int universityId);
        Task AddAsync(University university);
        void Remove(University university);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(int universityId);
        Task<bool> NameExistsAsync(string universityName, int? excludeUniversityId = null);
        Task<bool> HasDependenciesAsync(int universityId);
    }
}