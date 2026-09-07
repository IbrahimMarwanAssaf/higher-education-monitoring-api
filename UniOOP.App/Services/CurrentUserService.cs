using System.Security.Claims;
using UNIOOP.App.Services.Interfaces;

namespace UNIOOP.App.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? Role => _httpContextAccessor.HttpContext?
            .User.FindFirstValue(ClaimTypes.Role);

        public long? PersonnelID
        {
            get
            {
                string? value = _httpContextAccessor.HttpContext?.User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (long.TryParse(value, out long personnelId))
                {
                    return personnelId;
                }

                return null;
            }
        }
    }
}