using FinVault.Application.DTOs.RefreshTokenDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        // 1. TOKEN IDENTIFICATION
        Task<IEnumerable<RefreshTokenDto>> GetAllAsync();
        Task<RefreshTokenDto?> GetByIdAsync(int refreshTokenId);
        Task<RefreshTokenDto?> GetByTokenHashAsync(string tokenHash);
        Task<IEnumerable<RefreshTokenDto>> GetByJwtIdAsync(string jwtId);
        Task<bool> ExistsByTokenHashAsync(string tokenHash);

        // 2. TOKEN CREATION AND ROTATION
        Task<RefreshTokenDto> CreateAsync(CreateRefreshTokenDto dto, string tokenHash);
        Task<RefreshTokenDto?> RotateAsync(
            int currentRefreshTokenId,
            CreateRefreshTokenDto replacementDto,
            string replacementTokenHash);
        Task<RefreshTokenDto?> GetReplacementTokenAsync(int refreshTokenId);
        Task<IEnumerable<RefreshTokenDto>> GetReplacedTokensAsync(int refreshTokenId);
        Task<IEnumerable<RefreshTokenDto>> GetTokensWithReplacementAsync();
        Task<IEnumerable<RefreshTokenDto>> GetTokensWithoutReplacementAsync();

        // 3. USER SESSION MANAGEMENT
        Task<IEnumerable<RefreshTokenDto>> GetByUserIdAsync(int userId);
        Task<IEnumerable<RefreshTokenDto>> GetUserTokensWithReplacementHistoryAsync(int userId);
        Task<IEnumerable<RefreshTokenDto>> GetActiveTokensByUserIdAsync(int userId, DateTime currentDateTime);
        Task<int> CountActiveTokensByUserIdAsync(int userId, DateTime currentDateTime);
        Task<bool> RevokeAsync(int refreshTokenId, string revocationReason, string revokedByIp);
        Task<bool> RevokeAllByUserIdAsync(int userId, string revocationReason, string revokedByIp);

        // 4. TOKEN EXPIRY
        Task<IEnumerable<RefreshTokenDto>> GetExpiredTokensAsync(DateTime currentDateTime);
        Task<IEnumerable<RefreshTokenDto>> GetUnexpiredTokensAsync(DateTime currentDateTime);
        Task<IEnumerable<RefreshTokenDto>> GetTokensExpiringWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> IsTokenExpiredAsync(int refreshTokenId, DateTime currentDateTime);

        // 5. REVOCATION HISTORY
        Task<IEnumerable<RefreshTokenDto>> GetRevokedTokensAsync();
        Task<IEnumerable<RefreshTokenDto>> GetNonRevokedTokensAsync();
        Task<IEnumerable<RefreshTokenDto>> GetRevokedTokensByUserIdAsync(int userId);
        Task<IEnumerable<RefreshTokenDto>> GetRevokedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 6. CLIENT AND DEVICE INFORMATION
        Task<IEnumerable<RefreshTokenDto>> GetByCreatedByIpAsync(string ipAddress);
        Task<IEnumerable<RefreshTokenDto>> GetByRevokedByIpAsync(string ipAddress);
        Task<IEnumerable<RefreshTokenDto>> GetByDeviceNameAsync(string deviceName);
        Task<IEnumerable<RefreshTokenDto>> GetCreatedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 7. VALIDATION
        Task<bool> CanTokenBeRefreshedAsync(string tokenHash, DateTime currentDateTime);
        Task<bool> CanTokenBeRevokedAsync(int refreshTokenId);
    }
}


//Banking security rules:
//-Store only the SHA - 256 token hash, never the raw refresh token.
//- Refresh tokens must be single-use during rotation.
//- Revoke the old token when issuing its replacement.
//- Detect reuse of an already-replaced token and revoke the affected token family/session.
//- Never expose TokenHash through RefreshTokenDto.
//- Do not provide ordinary update or delete operations; preserve token security and audit history.
//- Perform rotation atomically using a database transaction.