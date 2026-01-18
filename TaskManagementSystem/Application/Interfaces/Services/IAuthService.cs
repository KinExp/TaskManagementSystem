using TaskManagement.Application.DTOs.Auth;

namespace TaskManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResultDto> LoginAsync(string email, string password, string deviceId);
        Task<LoginResultDto> RefreshAsync(string refreshToken, string deviceId);
        Task LogoutAsync(string refreshToken);
        Task LogoutAllAsync(Guid userId);
    }
}
