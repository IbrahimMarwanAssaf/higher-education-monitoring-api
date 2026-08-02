using UNIOOP.App.Dtos.Students;

namespace UNIOOP.App.Services.Interfaces
{
    public interface IStudentService
    {
        Task<List<StudentResponseDto>> GetAllAsync();
        Task<StudentResponseDto?> GetSingleAsync(int studentId);
        Task<StudentResponseDto> CreateAsync(CreateStudentDto dto);
        Task<bool> UpdateAsync(int studentId, UpdateStudentDto dto);
        Task<bool> DeleteAsync(int studentId);
    }
}