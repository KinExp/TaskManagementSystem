namespace TaskManagement.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; private set; } = null!;
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public string DeviceId { get; private set; } = null!;

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        protected RefreshToken() { }

        public RefreshToken(Guid userId, string token, DateTime expiresAt, string deviceId)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            DeviceId = deviceId;
            IsRevoked = false;
        }

        public bool IsActive => !IsRevoked && !IsExpired;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public void Revoke()
        {
            IsRevoked = true;
        }

        public static RefreshToken Create(Guid userId, string deviceId)
        {
            return new RefreshToken(
                userId,
                Guid.NewGuid().ToString("N"),
                DateTime.UtcNow.AddDays(7),
                deviceId);
        }
    }
}
