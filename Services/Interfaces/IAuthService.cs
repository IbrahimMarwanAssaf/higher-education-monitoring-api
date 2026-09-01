using UNIOOP.App.Dtos.Auth;

namespace UNIOOP.App.Services.Interfaces
{
    public interface IAuthService
    {
        Task CreateGovernmentOfficerAccountAsync(SignUpDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
    }
}