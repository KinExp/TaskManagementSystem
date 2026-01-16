using TaskManagement.Application.DTOs.Auth;

namespace TaskManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResultDto> LoginAsync(string email, string password);
    }
}
