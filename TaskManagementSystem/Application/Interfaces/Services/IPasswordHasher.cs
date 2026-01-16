namespace TaskManagement.Application.Interfaces.Services
{
    public interface IPasswordHasher
    {
        bool Verify(string password, string hash);
    }
}
