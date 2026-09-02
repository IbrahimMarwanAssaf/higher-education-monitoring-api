using UNIOOP.App.Dtos.Auth;

namespace UNIOOP.App.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
    }
}