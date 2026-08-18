namespace UNIOOP.App.Repositories.Interfaces
{
    public interface IPersonnelRepository
    {
        Task<bool> SSNExistsAsync(string ssn);
    }
}