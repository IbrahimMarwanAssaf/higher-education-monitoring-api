using UNIOOP.App.Dtos.GovernmentOfficers;

namespace UNIOOP.App.Services.Interfaces
{
    public interface IGovernmentOfficerService
    {
        Task<List<GovernmentOfficerResponseDto>> GetAllAsync();
        Task<GovernmentOfficerResponseDto> GetSingleAsync(int OfficerID);
        Task<GovernmentOfficerResponseDto> CreateAsync(CreateGovernmentOfficerDto dto);
        Task UpdateAsync(int OfficerID, UpdateGovernmentOfficerDto dto);
        Task DeleteAsync(int OfficerID);
    }
}