using EVChargingStation.CARC.Domain.TruongNN.DTOs.SessionDTOs;
using EVChargingStation.CARC.Infrastructure.TruongNN.Commons;

namespace EVChargingStation.CARC.Application.TruongNN.Interfaces
{
    public interface ISessionService
    {
        Task<Pagination<SessionResponseDto>> GetAllSessionsAsync(
            string? search,
            string? sortBy,
            bool isDescending,
            int page,
            int pageSize);

        Task<Pagination<SessionResponseDto>> GetUninvoicedSessionsAsync(
            string? search,
            string? sortBy,
            bool isDescending,
            int page,
            int pageSize);
    }
}
