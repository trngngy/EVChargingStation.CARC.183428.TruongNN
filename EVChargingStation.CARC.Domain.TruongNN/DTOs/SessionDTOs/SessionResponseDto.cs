using EVChargingStation.CARC.Domain.TruongNN.Enums;

namespace EVChargingStation.CARC.Domain.TruongNN.DTOs.SessionDTOs
{
    public class SessionResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public Guid ConnectorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public SessionStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public decimal? EnergyKwh { get; set; }
        public decimal? Cost { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
