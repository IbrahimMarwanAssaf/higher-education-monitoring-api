namespace UNIOOP.App.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(long personnelId);
    }
}