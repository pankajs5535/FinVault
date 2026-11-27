
using FinVault.Application.DTOs.RefreshTokenDtos;

namespace FinVault.Application.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<IEnumerable<RefreshTokenDto>> GetAllAsync();
    Task<RefreshTokenDto?> GetByIdAsync(int refreshTokenId);
    Task<RefreshTokenDto?> GetByTokenHashAsync(string tokenHash);
    Task<IEnumerable<RefreshTokenDto>> GetByJwtIdAsync(string jwtId);
    Task<bool> ExistsByTokenHashAsync(string tokenHash);

    Task<RefreshTokenDto> CreateAsync(CreateRefreshTokenDto dto, string tokenHash);
    Task<RefreshTokenDto?> RotateAsync(int currentRefreshTokenId, CreateRefreshTokenDto replacementDto, string replacementTokenHash);

    Task<RefreshTokenDto?> GetReplacementTokenAsync(int refreshTokenId);
    Task<IEnumerable<RefreshTokenDto>> GetReplacedTokensAsync(int refreshTokenId);
    Task<IEnumerable<RefreshTokenDto>> GetTokensWithReplacementAsync();
    Task<IEnumerable<RefreshTokenDto>> GetTokensWithoutReplacementAsync();

    Task<IEnumerable<RefreshTokenDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<RefreshTokenDto>> GetUserTokensWithReplacementHistoryAsync(int userId);
    Task<IEnumerable<RefreshTokenDto>> GetActiveTokensByUserIdAsync(int userId, DateTime currentDateTime);
    Task<int> CountActiveTokensByUserIdAsync(int userId, DateTime currentDateTime);

    Task<bool> RevokeAsync(int refreshTokenId, string revocationReason, string revokedByIp);
    Task<bool> RevokeAllByUserIdAsync(int userId, string revocationReason, string revokedByIp);

    Task<IEnumerable<RefreshTokenDto>> GetExpiredTokensAsync(DateTime currentDateTime);
    Task<IEnumerable<RefreshTokenDto>> GetUnexpiredTokensAsync(DateTime currentDateTime);
    Task<IEnumerable<RefreshTokenDto>> GetTokensExpiringWithinRangeAsync(DateTime startDate, DateTime endDate);
    Task<bool> IsTokenExpiredAsync(int refreshTokenId, DateTime currentDateTime);

    Task<IEnumerable<RefreshTokenDto>> GetRevokedTokensAsync();
    Task<IEnumerable<RefreshTokenDto>> GetNonRevokedTokensAsync();
    Task<IEnumerable<RefreshTokenDto>> GetRevokedTokensByUserIdAsync(int userId);
    Task<IEnumerable<RefreshTokenDto>> GetRevokedWithinRangeAsync(DateTime startDate, DateTime endDate);

    Task<IEnumerable<RefreshTokenDto>> GetByCreatedByIpAsync(string ipAddress);
    Task<IEnumerable<RefreshTokenDto>> GetByRevokedByIpAsync(string ipAddress);
    Task<IEnumerable<RefreshTokenDto>> GetByDeviceNameAsync(string deviceName);
    Task<IEnumerable<RefreshTokenDto>> GetCreatedWithinRangeAsync(DateTime startDate, DateTime endDate);

    Task<bool> CanTokenBeRefreshedAsync(string tokenHash, DateTime currentDateTime);
    Task<bool> CanTokenBeRevokedAsync(int refreshTokenId);
}
