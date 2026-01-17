namespace TaskManagement.Infrastructure.Auth
{
    public class JwtOptions
    {
        public string Issuer { get; init; } = null!;
        public string Audience { get; init; } = null!;
        public string Key { get; init; } = null!;
        public int ExpirationMinutes { get; init; }
    }
}
