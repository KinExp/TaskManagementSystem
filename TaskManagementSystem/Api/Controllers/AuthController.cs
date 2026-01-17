using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        /// <summary>
        /// Logout user by revoking refresh token
        /// </summary>
        /// <response code="204">Logout successful</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto)
        {
            await _authService.LogoutAsync(dto.RefreshToken);
            return NoContent();
        }

        /// <summary>
        /// Logout user from all devices
        /// </summary>
        /// <response code="204">Logout successful</response>
        [Authorize]
        [HttpPost("logout-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            await _authService.LogoutAllAsync(Guid.Parse(userId));
            return NoContent();
        }
    }
}