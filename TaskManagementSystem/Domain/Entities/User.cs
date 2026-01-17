namespace TaskManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public string Role { get; private set; } = "User";

        public ICollection<RefreshToken> RefreshTokens { get; private set; }
            = new List<RefreshToken>();

        protected User() { }

        public User(string email, string passwordHash)
        {
            Email = email;
            PasswordHash = passwordHash;
        }
    }
}
