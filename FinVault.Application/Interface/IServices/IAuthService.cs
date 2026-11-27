using FinVault.Application.DTOs.AuthDtos;

namespace FinVault.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(
        LoginRequestDto request,
        string ipAddress,
        string? userAgent,
        string? deviceName,
        CancellationToken cancellationToken = default);

    Task<AuthResponseDto?> RefreshAsync(
        RefreshTokenRequestDto request,
        string ipAddress,
        string? userAgent,
        string? deviceName,
        CancellationToken cancellationToken = default);

    Task<bool> LogoutAsync(
        LogoutRequestDto request,
        int userId,
        string ipAddress,
        CancellationToken cancellationToken = default);

    Task<bool> ChangePasswordAsync(
        ChangePasswordRequestDto request,
        int userId,
        CancellationToken cancellationToken = default);
}

