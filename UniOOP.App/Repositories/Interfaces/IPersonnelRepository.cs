namespace UNIOOP.App.Repositories.Interfaces
{
    public interface IPersonnelRepository
    {
        Task<bool> SSNExistsAsync(string ssn);

        Task<bool> EmailExistsAsync(string email, long? excludePersonnelId = null);
    }
}