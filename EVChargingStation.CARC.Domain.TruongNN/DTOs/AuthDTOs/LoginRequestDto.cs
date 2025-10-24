using System.ComponentModel;

namespace EVChargingStation.CARC.Domain.TruongNN.DTOs.AuthDTOs
{
    public class LoginRequestDto
    {
        [DefaultValue("Admin@gmail.com")]
        public required string? Email { get; set; }

        [DefaultValue("Admin@123")]
        public required string? Password { get; set; }
    }
}
