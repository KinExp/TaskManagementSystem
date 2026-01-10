using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.Interfaces.Services;
using TaskManagement.Domain.Entities;
using BCrypt.Net;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            AppDbContext dbContext,
            IJwtTokenService jwtTokenService)
        {
            _dbContext = dbContext;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized();

            var token = _jwtTokenService.GenerateToken(user.Id, user.Email);

            return Ok(new { accessToken = token });
        }
    }
}