using UNIOOP.App.Dtos.Teachers;

namespace UNIOOP.App.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<List<TeacherResponseDto>> GetAllAsync();
        Task<TeacherResponseDto?> GetSingleAsync(int teacherId);
        Task<TeacherResponseDto> CreateAsync(CreateTeacherDto dto);
        Task<bool> UpdateAsync(int teacherId, UpdateTeacherDto dto);
        Task<bool> DeleteAsync(int teacherId);
    }
}