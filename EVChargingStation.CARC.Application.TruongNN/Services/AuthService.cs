using EVChargingStation.CARC.Application.TruongNN.Interfaces;
using EVChargingStation.CARC.Application.TruongNN.Interfaces.Commons;
using EVChargingStation.CARC.Application.TruongNN.Utils;
using EVChargingStation.CARC.Domain.TruongNN.DTOs.AuthDTOs;
using EVChargingStation.CARC.Domain.TruongNN.Entities;
using EVChargingStation.CARC.Domain.TruongNN.Enums;
using EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace EVChargingStationCARC.Application.TruongNN.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggerService _loggerService;

    public AuthService(IUnitOfWork unitOfWork, ILoggerService loggerService)
    {
        _unitOfWork = unitOfWork;
        _loggerService = loggerService;
    }

    /// <summary>
    ///     Login a user and return JWT access and refresh token.
    /// </summary>
    /// <param name="loginDto"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto, IConfiguration configuration)
    {
        _loggerService.Info($"Login attempt for email: {loginDto.Email}");

        //Get user from DB
        var user = await GetUserByEmailAsync(loginDto.Email!);
        if (user == null)
            throw ErrorHelper.NotFound("Account does not exist.");

        if(!new PasswordHasher().VerifyPassword(loginDto.Password!, user.PasswordHash))
            throw ErrorHelper.Unauthorized("Password is incorrect.");

        if (user.Status == UserStatus.Banned)
            throw ErrorHelper.Forbidden("Account is inactive. Please contact support.");

        if (user.Status != UserStatus.Active)
            throw ErrorHelper.Unauthorized("Account is not active. Please verify your email.");

        _loggerService.Success($"User {loginDto.Email} authenticated successfully.");

        var accessToken = JwtUtils.GenerateJwtToken(
            user.TruongNNID,
            user.Email,
            user.Role.ToString(),
            configuration,
            TimeSpan.FromMinutes(30)
            );

        var refreshToken = Guid.NewGuid().ToString();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _loggerService.Info($"Tokens generated for user {loginDto.Email}.");

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    /// <summary>
    ///     Logout a user by removing their refresh token from the database.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<bool> LogoutAsync(Guid userId)
    {
        _loggerService.Info($"Logout attempt for user ID: {userId}");

        var user = await GetUserById(userId);

        if (user == null)
            throw ErrorHelper.NotFound("User not found.");

        if (user.IsDeleted || user.Status == UserStatus.Banned || user.Status == UserStatus.Deleted)
            throw ErrorHelper.Forbidden("Account has been disabled or banned.");

        if (string.IsNullOrEmpty(user.RefreshToken))
            throw ErrorHelper.BadRequest("User previously logged out.");

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _loggerService.Info($"Logout successful for user ID: {userId}.");
        return true;
    }

    public async Task<LoginResponseDto?> RefreshTokenAsync(TokenRefreshRequestDto refreshTokenDto, IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenDto.RefreshToken))
            throw ErrorHelper.BadRequest("Missing tokens");

        var user = await GetUserByRefreshToken(refreshTokenDto.RefreshToken);

        if (user == null)
            throw ErrorHelper.NotFound("Account does not exist.");

        if (string.IsNullOrEmpty(user.RefreshToken))
            throw ErrorHelper.BadRequest("User previously logged out.");

        // Kiểm tra Refresh Token có còn hiệu lực hay không
        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw ErrorHelper.Conflict("Refresh token has expired.");

        var roleName = user.Role.ToString();

        // Tạo mới access và refresh token
        var newAccessToken = JwtUtils.GenerateJwtToken(
            user.TruongNNID,
            user.Email,
            roleName,
            configuration,
            TimeSpan.FromHours(1)
        );

        var newRefreshToken = Guid.NewGuid().ToString();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    private async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    private async Task<User?> GetUserById(Guid id)
    {
        return await _unitOfWork.Users.GetByIdAsync(id);
    }

    private async Task<User?> GetUserByRefreshToken(string refreshToken)
    {
        return await _unitOfWork.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}