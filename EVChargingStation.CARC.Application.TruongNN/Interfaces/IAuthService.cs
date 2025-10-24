using EVChargingStation.CARC.Domain.TruongNN.DTOs.AuthDTOs;
using Microsoft.Extensions.Configuration;

namespace EVChargingStation.CARC.Application.TruongNN.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto, IConfiguration configuration);
    Task<bool> LogoutAsync(Guid userId);
    Task<LoginResponseDto?> RefreshTokenAsync(TokenRefreshRequestDto refreshTokenDto, IConfiguration configuration);

}