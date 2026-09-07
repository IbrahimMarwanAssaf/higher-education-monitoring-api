namespace UNIOOP.App.Services.Interfaces
{
    public interface ICurrentUserService
    {
        string? Role { get; }
        long? PersonnelID { get; }
    }
}