namespace EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;

public interface IClaimsService
{
    public Guid GetCurrentUserId { get; }

    public string? IpAddress { get; }
}