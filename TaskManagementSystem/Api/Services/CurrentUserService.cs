using System.Security.Claims;
using TaskManagement.Application.Interfaces.Services;

namespace TaskManagement.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                if (user?.Identity?.IsAuthenticated != true)
                    return null;

                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

                return userIdClaim != null
                    ? Guid.Parse(userIdClaim.Value)
                    : null;
            }
        }
    }
}
