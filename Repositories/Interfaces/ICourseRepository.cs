using UNIOOP.App.Models;

namespace UNIOOP.App.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int courseId);
        Task<Course?> GetByIdForUpdateAsync(int courseId);
        Task AddAsync(Course course);
        void Remove(Course course);
        Task SaveChangesAsync();
    }
}