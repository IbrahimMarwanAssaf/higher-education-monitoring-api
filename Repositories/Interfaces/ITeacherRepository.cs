using UNIOOP.App.Models;

namespace UNIOOP.App.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        Task<List<Teacher>> GetAllAsync();
        Task<Teacher?> GetByIdAsync(int teacherId);
        Task<Teacher?> GetByIdForUpdateAsync(int teacherId);
        Task AddAsync(Teacher teacher);
        void Remove(Teacher teacher);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(int teacherId);
        Task<bool> EmailExistsAsync(string email, int? excludeTeacherId = null);
        Task<bool> BelongsToUniversityAsync(int teacherId, int universityId);
    }
}