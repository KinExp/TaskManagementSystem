using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.Interfaces.Services;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticate user and return JWT access token
        /// </summary>
        /// <response code="200">Login successful</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Invalid email or password</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto.Email, dto.Password);
            return Ok(result);
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <response code="200">Token refreshed</response>
        /// <response code="401">Invalid refresh token</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResultDto>> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            var result = await _authService.RefreshAsync(dto.RefreshToken);
            return Ok(result);
        }
    }
}