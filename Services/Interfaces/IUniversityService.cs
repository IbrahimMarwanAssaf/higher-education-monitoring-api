using UNIOOP.App.Dtos.Universities;
using UNIOOP.App.Models;

namespace UNIOOP.App.Services.Interfaces
{
    public interface IUniversityService
    {
        Task<List<University>> GetAllAsync();
        Task<University?> GetSingleAsync(int universityId);
        Task<University?> CreateAsync(UniversityDto dto);
        Task<bool> UpdateAsync(int universityId, UniversityDto dto);
        Task<bool> DeleteAsync(int universityId);
        Task<bool> HasDependenciesAsync(int universityId);
    }
}