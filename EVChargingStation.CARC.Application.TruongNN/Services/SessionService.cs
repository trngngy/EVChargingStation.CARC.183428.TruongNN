using EVChargingStation.CARC.Application.TruongNN.Interfaces;
using EVChargingStation.CARC.Application.TruongNN.Interfaces.Commons;
using EVChargingStation.CARC.Domain.TruongNN.DTOs.SessionDTOs;
using EVChargingStation.CARC.Domain.TruongNN.Enums;
using EVChargingStation.CARC.Infrastructure.TruongNN.Commons;
using EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVChargingStation.CARC.Application.TruongNN.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerService _logger;

        public SessionService(IUnitOfWork unitOfWork, ILoggerService logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Pagination<SessionResponseDto>> GetAllSessionsAsync(
            string? search,
            string? sortBy,
            bool isDescending,
            int page,
            int pageSize)
        {
            try
            {
                var sessionsQuery = _unitOfWork.Sessions
                    .GetQueryable()
                    .Include(s => s.User)
                    .Where(s => !s.IsDeleted);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchLower = search.ToLower();
                    sessionsQuery = sessionsQuery.Where(s =>
                        s.User.Email.ToLower().Contains(searchLower) ||
                        (s.User.FirstName + " " + s.User.LastName).ToLower().Contains(searchLower));
                }

                var sortKey = (sortBy ?? string.Empty).ToLower();

                sessionsQuery = sortKey switch
                {
                    "starttime" => isDescending
                        ? sessionsQuery.OrderByDescending(s => s.StartTime)
                        : sessionsQuery.OrderBy(s => s.StartTime),
                    "endtime" => isDescending
                        ? sessionsQuery.OrderByDescending(s => s.EndTime)
                        : sessionsQuery.OrderBy(s => s.EndTime),
                    "energykwh" => isDescending
                        ? sessionsQuery.OrderByDescending(s => s.EnergyKwh)
                        : sessionsQuery.OrderBy(s => s.EnergyKwh),
                    "cost" => isDescending
                        ? sessionsQuery.OrderByDescending(s => s.Cost)
                        : sessionsQuery.OrderBy(s => s.Cost),
                    "status" => isDescending
                        ? sessionsQuery.OrderByDescending(s => s.Status)
                        : sessionsQuery.OrderBy(s => s.Status),
                    _ => isDescending
                        ? sessionsQuery.OrderByDescending(s => s.CreatedAt)
                        : sessionsQuery.OrderBy(s => s.CreatedAt)
                };

                var total = await sessionsQuery.CountAsync();

                var paged = await sessionsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = paged.Select(s => new SessionResponseDto
                {
                    Id = s.TruongNNID,
                    UserId = s.UserId,
                    UserEmail = s.User?.Email ?? "N/A",
                    UserFullName = s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : "N/A",
                    ConnectorId = s.ConnectorId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Status = s.Status,
                    StatusDisplay = GetStatusDisplay(s.Status),
                    EnergyKwh = s.EnergyKwh,
                    Cost = s.Cost,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }).ToList();

                return new Pagination<SessionResponseDto>(dtos, total, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting all sessions: {ex.Message}");
                throw new Exception("An unexpected error occurred while fetching sessions.");
            }
        }

        private string GetStatusDisplay(SessionStatus status)
        {
            return status switch
            {
                SessionStatus.Stopped => "Stopped",
                SessionStatus.Failed => "Failed",
                _ => "Unknown"
            };
        }
    }
}
