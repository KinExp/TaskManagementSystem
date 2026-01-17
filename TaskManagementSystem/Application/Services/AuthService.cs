using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Application.Interfaces.Services;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResultDto> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
                throw new UnauthorizedException("Invalid email or password");

            var accessToken = _jwtTokenService
                .GenerateToken(user.Id, user.Email);

            var refreshToken = RefreshToken.Create(user.Id);
            await _refreshTokenRepository.AddAsync(refreshToken);

            return new LoginResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<LoginResultDto> RefreshAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository
                .GetByTokenAsync(refreshToken);

            if (token == null)
                throw new UnauthorizedException("Invalid refresh token");

            if (token.IsExpired)
                throw new UnauthorizedException("Refresh token expired");

            if (token.IsRevoked)
            {
                await _refreshTokenRepository.RevokeAllAsync(token.UserId);
                throw new UnauthorizedException("Refresh token reuse detected");
            }

            token.Revoke();
            await _refreshTokenRepository.UpdateAsync(token);

            var user = token.User;

            var accessToken = _jwtTokenService
                .GenerateToken(user.Id, user.Email);

            var newRefreshToken = RefreshToken.Create(user.Id);
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            return new LoginResultDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _refreshTokenRepository
                .GetByTokenAsync(refreshToken);

            if (token == null || token.IsRevoked || token.IsExpired)
                return;

            token.Revoke();
            await _refreshTokenRepository.UpdateAsync(token);
        }

        public async Task LogoutAllAsync(Guid userId)
        {
            await _refreshTokenRepository.RevokeAllAsync(userId);
        }
    }
}
