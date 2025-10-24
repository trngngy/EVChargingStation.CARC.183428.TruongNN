using EVChargingStation.CARC.Application.TruongNN.Interfaces;
using EVChargingStation.CARC.Application.TruongNN.Utils;
using EVChargingStation.CARC.Domain.TruongNN.DTOs.AuthDTOs;
using EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EVChargingStation.CARC.WebAPI.TruongNN.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IClaimsService _claimsService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IClaimsService claimsService, IConfiguration configuration)
        {
            _authService = authService;
            _claimsService = claimsService;
            _configuration = configuration;
        }

        /// <summary>
        /// User login.
        /// </summary>
        /// <param name="dto">Login credentials.</param>
        /// <returns>JWT access and refresh tokens.</returns>
        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "User login",
            Description = "Authenticate user and return JWT tokens."
        )]
        [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), 400)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto, _configuration);
                return Ok(ApiResult<LoginResponseDto>.Success(result!, "200", "Login successful."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<LoginResponseDto>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        /// <summary>
        /// Logout the current user.
        /// </summary>
        /// <returns>Logout result.</returns>
        [HttpPost("logout")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Logout user",
            Description = "Logs out the currently authenticated user."
        )]
        [ProducesResponseType(typeof(ApiResult<object>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        [ProducesResponseType(typeof(ApiResult<object>), 500)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = _claimsService.GetCurrentUserId;
                var result = await _authService.LogoutAsync(userId);
                return Ok(ApiResult<object>.Success(result!, "200", "Loged out successfully."));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<object>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }

        /// <summary>
        /// Refresh JWT access token using a valid refresh token.
        /// </summary>
        /// <param name="requestToken">Refresh token data.</param>
        /// <returns>New JWT tokens.</returns>
        [HttpPost("refresh-token")]
        [SwaggerOperation(
            Summary = "Refresh JWT token",
            Description = "Refresh JWT access token using a valid refresh token."
        )]
        [ProducesResponseType(typeof(ApiResult<LoginResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResult<object>), 400)]
        [ProducesResponseType(typeof(ApiResult<object>), 401)]
        [ProducesResponseType(typeof(ApiResult<object>), 500)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequestDto requestToken)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(requestToken, _configuration);
                return Ok(ApiResult<object>.Success(result!, "200", "Refresh Token successfully"));
            }
            catch (Exception ex)
            {
                var statusCode = ExceptionUtils.ExtractStatusCode(ex);
                var errorResponse = ExceptionUtils.CreateErrorResponse<object>(ex);
                return StatusCode(statusCode, errorResponse);
            }
        }
    }
}
