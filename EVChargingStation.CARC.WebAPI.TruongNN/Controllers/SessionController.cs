using EVChargingStation.CARC.Application.TruongNN.Interfaces;
using EVChargingStation.CARC.Application.TruongNN.Interfaces.Commons;
using EVChargingStation.CARC.Application.TruongNN.Utils;
using EVChargingStation.CARC.Domain.TruongNN.DTOs.SessionDTOs;
using EVChargingStation.CARC.Domain.TruongNN.Enums;
using EVChargingStation.CARC.Infrastructure.TruongNN.Commons;
using EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVChargingStation.CARC.WebAPI.TruongNN.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClaimsService _claimsService;
        private readonly ILoggerService _logger;
        private readonly ISessionService _sessionService;

        public SessionController(
            IUnitOfWork unitOfWork,
            IClaimsService claimsService,
            ILoggerService logger,
            ISessionService sessionService)
        {
            _unitOfWork = unitOfWork;
            _claimsService = claimsService;
            _logger = logger;
            _sessionService = sessionService;
        }

        /// <summary>
        /// Get all session IDs of the current authenticated user
        /// </summary>
        /// <remarks>
        /// Returns list of all session IDs from the Session table for the currently logged-in user.
        /// Uses ClaimService to get the current user ID from JWT token.
        /// </remarks>
        /// <returns>List of session IDs</returns>
        [HttpGet("my-session-ids")]
        public async Task<IActionResult> GetMySessionIds()
        {
            try
            {
                // Lấy userId từ ClaimService
                var userId = _claimsService.GetCurrentUserId;

                if (userId == Guid.Empty)
                    throw ErrorHelper.Unauthorized("User not authenticated.");

                // Lấy tất cả SessionId từ bảng Session của user hiện tại
                var sessionIds = await _unitOfWork.Sessions
            .GetQueryable()
            .Where(s => s.UserId == userId
                && !s.IsDeleted
                && !s.InvoiceTruongNNId.HasValue
                && s.Status == SessionStatus.Stopped
                && s.Cost.HasValue)
            .OrderByDescending(s => s.StartTime)
            .Select(s => s.TruongNNID)
            .ToListAsync();

                _logger.Success($"Retrieved {sessionIds.Count} session IDs for user {userId}");

                return Ok(ApiResult<object>.Success(new
                {
                    UserId = userId,
                    TotalSessions = sessionIds.Count,
                    SessionIds = sessionIds
                }, "200", $"Retrieved {sessionIds.Count} session IDs successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting session IDs: {ex.Message}");
                var statusCode = ex.Data["StatusCode"] as int? ??500;
                return StatusCode(statusCode,
                    ApiResult<object>.Failure(statusCode.ToString(), ex.Message));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSessions(
            [FromQuery] string? search,
            [FromQuery] string? sortBy,
            [FromQuery] bool isDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _sessionService.GetAllSessionsAsync(
                    search,
                    sortBy,
                    isDescending,
                    page,
                    pageSize);

                return Ok(ApiResult<Pagination<SessionResponseDto>>.Success(
                    result,
                    "200",
                    $"Retrieved {result.Items.Count} sessions."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting sessions: {ex.Message}");
                var statusCode = ex.Data["StatusCode"] as int? ?? 500;
                return StatusCode(statusCode,
                    ApiResult<Pagination<SessionResponseDto>>.Failure(
                        statusCode.ToString(),
                        ex.Message));
            }
        }

        [HttpGet("uninvoice")]
        public async Task<IActionResult> GetUnivoicedSessionsAsync(
          [FromQuery] string? search,
          [FromQuery] string? sortBy,
          [FromQuery] bool isDescending = false,
          [FromQuery] int page = 1,
          [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _sessionService.GetUninvoicedSessionsAsync(
                    search,
                    sortBy,
                    isDescending,
                    page,
                    pageSize);

                return Ok(ApiResult<Pagination<SessionResponseDto>>.Success(
                    result,
                    "200",
                    $"Retrieved {result.Items.Count} sessions."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting sessions: {ex.Message}");
                var statusCode = ex.Data["StatusCode"] as int? ?? 500;
                return StatusCode(statusCode,
                    ApiResult<Pagination<SessionResponseDto>>.Failure(
                        statusCode.ToString(),
                        ex.Message));
            }
        }
    }
}