using FinVault.Application.DTOs.AuthDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Domain.Entities;

namespace FinVault.Infrastructure.Identity;

public sealed class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IRefreshTokenGenerator _refreshTokenService;
    private readonly IAuthPolicy _authPolicy;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IPasswordHashService passwordHashService,
        IRefreshTokenGenerator refreshTokenService,
        IAuthPolicy authPolicy)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
        _passwordHashService = passwordHashService;
        _refreshTokenService = refreshTokenService;
        _authPolicy = authPolicy;
    }

    public async Task<AuthResponseDto?> LoginAsync(
        LoginRequestDto request,
        string ipAddress,
        string? userAgent,
        string? deviceName,
        CancellationToken cancellationToken = default)
    {
        var identifier = request.Identifier.Trim();
        var user = await _unitOfWork.User.GetByUsernameAsync(identifier)
                   ?? await _unitOfWork.User.GetByEmailAsync(identifier);

        if (user is null || !user.IsActive)
            return null;

        var now = DateTime.UtcNow;
        if (user.LockoutEndAt.HasValue && user.LockoutEndAt.Value > now)
            return null;

        if (!_passwordHashService.VerifyPassword(user, request.Password))
        {
            user.FailedLoginAttempts++;

            if (user.FailedLoginAttempts >= _authPolicy.MaxFailedAccessAttempts)
            {
                user.LockoutEndAt = now.AddMinutes(_authPolicy.LockoutMinutes);
            }

            _unitOfWork.User.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return null;
        }

        var role = await _unitOfWork.Role.GetRoleDetailsAsync(user.RoleId);
        if (role is null || !role.IsActive)
            return null;

        user.Role = role;
        user.FailedLoginAttempts = 0;
        user.LockoutEndAt = null;
        user.LastLoginAt = now;
        user.LastLoginIp = ipAddress;
        _unitOfWork.User.Update(user);

        var response = await CreateSessionAsync(user, role, ipAddress, userAgent, deviceName, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return response;
    }

    public async Task<AuthResponseDto?> RefreshAsync(
        RefreshTokenRequestDto request,
        string ipAddress,
        string? userAgent,
        string? deviceName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return null;

        var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);
        var currentToken = await _unitOfWork.RefreshToken.GetByTokenHashAsync(tokenHash);
        if (currentToken is null)
            return null;

        var now = DateTime.UtcNow;

        // A revoked or replaced token being presented again is treated as token reuse.
        if (currentToken.RevokedAt.HasValue || currentToken.ReplacedByTokenId.HasValue)
        {
            await RevokeAllUserTokensAsync(currentToken.UserId, now, ipAddress, "Refresh-token reuse detected.");
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return null;
        }

        if (currentToken.ExpiresAt <= now)
            return null;

        var user = await _unitOfWork.User.GetByIdAsync(currentToken.UserId);
        if (user is null || !user.IsActive ||
            (user.LockoutEndAt.HasValue && user.LockoutEndAt.Value > now))
            return null;

        var role = await _unitOfWork.Role.GetRoleDetailsAsync(user.RoleId);
        if (role is null || !role.IsActive)
            return null;

        user.Role = role;

        var response = await CreateSessionAsync(user, role, ipAddress, userAgent, deviceName, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        currentToken.RevokedAt = now;
        currentToken.RevocationReason = "Rotated";
        currentToken.RevokedByIp = ipAddress;
        currentToken.ReplacedByTokenId = await FindNewTokenIdAsync(response.RefreshToken);
        _unitOfWork.RefreshToken.Update(currentToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return response;
    }

    public async Task<bool> LogoutAsync(
        LogoutRequestDto request,
        int userId,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return false;

        var token = await _unitOfWork.RefreshToken.GetByTokenHashAsync(
            _refreshTokenService.HashToken(request.RefreshToken));

        if (token is null || token.UserId != userId || token.RevokedAt.HasValue)
            return false;

        token.RevokedAt = DateTime.UtcNow;
        token.RevocationReason = "User logout";
        token.RevokedByIp = ipAddress;
        _unitOfWork.RefreshToken.Update(token);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ChangePasswordAsync(
        ChangePasswordRequestDto request,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.User.GetByIdAsync(userId);
        if (user is null || !_passwordHashService.VerifyPassword(user, request.CurrentPassword))
            return false;

        if (string.Equals(request.CurrentPassword, request.NewPassword, StringComparison.Ordinal))
            return false;

        user.PasswordHash = _passwordHashService.HashPassword(user, request.NewPassword);
        user.PasswordChangedAt = DateTime.UtcNow;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedBy = userId;

        _unitOfWork.User.Update(user);
        await RevokeAllUserTokensAsync(userId, DateTime.UtcNow, null, "Password changed.");
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<AuthResponseDto> CreateSessionAsync(
        User user,
        Role role,
        string ipAddress,
        string? userAgent,
        string? deviceName,
        CancellationToken cancellationToken)
    {
        var accessToken = _jwtTokenService.CreateAccessToken(user, role);
        var rawRefreshToken = _refreshTokenService.GenerateToken();
        var refreshToken = new RefreshToken
        {
            TokenHash = _refreshTokenService.HashToken(rawRefreshToken),
            JwtId = accessToken.JwtId,
            ExpiresAt = _refreshTokenService.GetExpiryUtc(),
            CreatedByIp = ipAddress,
            UserAgent = userAgent,
            DeviceName = deviceName,
            UserId = user.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshToken.AddAsync(refreshToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken.AccessToken,
            RefreshToken = rawRefreshToken,
            AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
            RefreshTokenExpiresAtUtc = refreshToken.ExpiresAt,
            UserId = user.UserId,
            Username = user.Username,
            Role = role.Name
        };
    }

    private async Task<int?> FindNewTokenIdAsync(string rawRefreshToken)
    {
        var token = await _unitOfWork.RefreshToken.GetByTokenHashAsync(
            _refreshTokenService.HashToken(rawRefreshToken));
        return token?.RefreshTokenId;
    }

    private async Task RevokeAllUserTokensAsync(int userId, DateTime now, string? ipAddress, string reason)
    {
        var tokens = await _unitOfWork.RefreshToken.GetByUserIdAsync(userId);
        foreach (var token in tokens.Where(x => !x.RevokedAt.HasValue && x.ExpiresAt > now))
        {
            token.RevokedAt = now;
            token.RevocationReason = reason;
            token.RevokedByIp = ipAddress;
            _unitOfWork.RefreshToken.Update(token);
        }
    }
}

