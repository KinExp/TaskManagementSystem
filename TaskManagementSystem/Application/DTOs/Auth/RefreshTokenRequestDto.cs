namespace TaskManagement.Application.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = null!;
        public string DeviceId { get; set; } = null!;
    }
}
