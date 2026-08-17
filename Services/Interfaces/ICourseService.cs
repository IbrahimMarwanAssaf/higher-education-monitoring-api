using UNIOOP.App.Dtos.Courses;

namespace UNIOOP.App.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseResponseDto>> GetAllAsync();
        Task<CourseResponseDto> GetSingleAsync(int courseId);
        Task<CourseResponseDto> CreateAsync(CreateCourseDto dto);
        Task UpdateAsync(int courseId, UpdateCourseDto dto);
        Task DeleteAsync(int courseId);
    }
}