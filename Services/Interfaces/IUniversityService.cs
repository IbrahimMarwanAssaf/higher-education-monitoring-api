using UNIOOP.App.Dtos.Universities;

namespace UNIOOP.App.Services.Interfaces
{
    public interface IUniversityService
    {
        Task<List<UniversityResponseDto>> GetAllAsync();
        Task<UniversityResponseDto?> GetSingleAsync(int universityId);
        Task<UniversityResponseDto> CreateAsync(UniversityCreateUpdateDto dto);
        Task<bool> UpdateAsync(int universityId, UniversityCreateUpdateDto dto);
        Task<bool> DeleteAsync(int universityId);
    }
}