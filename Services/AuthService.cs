using Microsoft.AspNetCore.Identity;
using UNIOOP.App.Dtos.Auth;
using UNIOOP.App.Helpers;
using UNIOOP.App.Models;
using UNIOOP.App.Repositories.Interfaces;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly ExceptionHelper _exceptionHelper;
        private readonly PasswordHasher<UserAccount> _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(IUserAccountRepository userAccountRepository,
            ExceptionHelper exceptionHelper,
            PasswordHasher<UserAccount> passwordHasher,
            ITokenService tokenService)
        {
            _userAccountRepository = userAccountRepository;
            _exceptionHelper = exceptionHelper;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var userAccount = await _userAccountRepository.GetByEmailAsync(dto.Email);

            if (userAccount is null)
            {
                throw _exceptionHelper.BadRequest("Invalid email or password.");
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(userAccount,
                userAccount.PasswordHash,
                dto.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                throw _exceptionHelper.BadRequest("Invalid email or password.");
            }

            return new LoginResponseDto
            {
                AccessToken = _tokenService.GenerateAccessToken(userAccount.PersonnelID, userAccount.Role)
            };
        }
    }
}