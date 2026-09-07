using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UNIOOP.App.Dtos.Auth;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);
            return Ok(response);
        }
    }
}