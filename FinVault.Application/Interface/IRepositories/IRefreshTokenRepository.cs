
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        // =========================================================
        // 1. TOKEN IDENTIFICATION
        // =========================================================

        // Retrieves a refresh token using its unique SHA-256 hash.
        // Useful for validating a presented refresh token without storing or querying its raw value.
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);

        // Retrieves refresh tokens associated with an access token JWT ID.
        // Useful for investigating refresh-token records linked to a particular access-token identity.
        Task<IEnumerable<RefreshToken>> GetByJwtIdAsync(string jwtId);

        // Checks whether a token hash already exists.
        // Useful for preventing duplicate refresh-token records.
        Task<bool> ExistsByTokenHashAsync(string tokenHash);

        // =========================================================
        // 2. USER TOKEN MANAGEMENT
        // =========================================================

        // Retrieves all refresh tokens belonging to a user.
        // Useful for reviewing the user's registered sessions and devices.
        Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId);

        // Retrieves all refresh tokens for a user with their replacement-token details.
        // Useful for investigating refresh-token rotation history across user sessions.
        Task<IEnumerable<RefreshToken>> GetUserTokensWithReplacementHistoryAsync(int userId);

        // Retrieves a user's non-revoked and unexpired refresh tokens.
        // Useful for identifying sessions that may still be eligible for refresh.
        Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(int userId, DateTime currentDateTime);

        // Counts non-revoked and unexpired refresh tokens for a user.
        // Useful for monitoring the number of active sessions associated with an account.
        Task<int> CountActiveTokensByUserIdAsync(int userId, DateTime currentDateTime);

        // =========================================================
        // 3. TOKEN EXPIRY
        // =========================================================

        // Retrieves refresh tokens that have expired.
        // Useful for identifying records that are no longer eligible for token refresh.
        Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync(DateTime currentDateTime);

        // Retrieves unexpired refresh tokens that have not been revoked.
        // Useful for identifying tokens that remain eligible for refresh based on stored state.
        Task<IEnumerable<RefreshToken>> GetUnexpiredTokensAsync(DateTime currentDateTime);

        // Retrieves tokens expiring within a supplied date range.
        // Useful for monitoring upcoming session expiry.
        Task<IEnumerable<RefreshToken>> GetTokensExpiringWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 4. REVOCATION
        // =========================================================

        // Retrieves refresh tokens that have been revoked.
        // Useful for reviewing sessions invalidated by users or security processes.
        Task<IEnumerable<RefreshToken>> GetRevokedTokensAsync();

        // Retrieves refresh tokens that have not been revoked.
        // Useful for reviewing tokens that have no recorded revocation event.
        Task<IEnumerable<RefreshToken>> GetNonRevokedTokensAsync();

        // Retrieves revoked tokens belonging to a particular user.
        // Useful for investigating a user's session termination history.
        Task<IEnumerable<RefreshToken>> GetRevokedTokensByUserIdAsync(int userId);

        // Retrieves tokens revoked within a supplied date range.
        // Useful for security reporting and reviewing session revocation activity.
        Task<IEnumerable<RefreshToken>> GetRevokedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 5. TOKEN ROTATION
        // =========================================================

        // Retrieves the token that replaced a particular refresh token.
        // Useful for tracing the next token issued during refresh-token rotation.
        Task<RefreshToken?> GetReplacementTokenAsync(int refreshTokenId);

        // Retrieves tokens that replaced a particular refresh token.
        // Useful for reviewing the token rotation chain.
        Task<IEnumerable<RefreshToken>> GetReplacedTokensAsync(int refreshTokenId);

        // Retrieves refresh tokens that have been replaced by another token.
        // Useful for identifying previous tokens in a rotation sequence.
        Task<IEnumerable<RefreshToken>> GetTokensWithReplacementAsync();

        // Retrieves refresh tokens that have not been replaced by another token.
        // Useful for identifying the latest recorded token in each rotation chain.
        Task<IEnumerable<RefreshToken>> GetTokensWithoutReplacementAsync();

        // =========================================================
        // 6. CLIENT AND DEVICE INFORMATION
        // =========================================================

        // Retrieves refresh tokens created from a particular IP address.
        // Useful for reviewing sessions originating from a specific client network address.
        Task<IEnumerable<RefreshToken>> GetByCreatedByIpAsync(string ipAddress);

        // Retrieves refresh tokens revoked from a particular IP address.
        // Useful for investigating where session revocation requests originated.
        Task<IEnumerable<RefreshToken>> GetByRevokedByIpAsync(string ipAddress);

        // Retrieves refresh tokens associated with a particular device name.
        // Useful for reviewing sessions registered against a named device.
        Task<IEnumerable<RefreshToken>> GetByDeviceNameAsync(string deviceName);

        // Retrieves refresh tokens created within a supplied date range.
        // Useful for reviewing session creation activity.
        Task<IEnumerable<RefreshToken>> GetCreatedWithinRangeAsync(DateTime startDate, DateTime endDate);
    }
}
