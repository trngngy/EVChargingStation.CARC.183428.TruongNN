using EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;

namespace EVChargingStation.CARC.Infrastructure.TruongNN.Commons;

public class CurrentTime : ICurrentTime
{
    public DateTime GetCurrentTime()
    {
        return DateTime.UtcNow; // Đảm bảo trả về thời gian UTC
    }
}