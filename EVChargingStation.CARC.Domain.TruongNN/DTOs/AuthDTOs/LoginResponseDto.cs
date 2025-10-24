namespace EVChargingStation.CARC.Domain.TruongNN.DTOs.AuthDTOs
{
    public class LoginResponseDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
