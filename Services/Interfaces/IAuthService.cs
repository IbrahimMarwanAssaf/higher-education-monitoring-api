using UNIOOP.App.Dtos.Auth;

namespace UNIOOP.App.Services.Interfaces
{
    public interface IAuthService
    {
        Task SignUpAsync(SignUpDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
    }
}