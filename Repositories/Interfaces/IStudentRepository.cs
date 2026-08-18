using UNIOOP.App.Models;

namespace UNIOOP.App.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int studentId);
        Task<Student?> GetByIdForUpdateAsync(int studentId);
        Task AddAsync(Student student);
        void Remove(Student student);
        Task SaveChangesAsync();
        Task<bool> ExistsAsync(int studentId);
        Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null);
        Task<bool> HasEnrollmentsAsync(int studentId);
    }
}