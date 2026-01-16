using TaskManagement.Application.Interfaces.Services;

namespace Infrastructure.Auth
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
