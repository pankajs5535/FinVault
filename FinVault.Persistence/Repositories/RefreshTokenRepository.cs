
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. TOKEN IDENTIFICATION
        // =========================================================

        // Retrieves a refresh token using its unique SHA-256 hash.
        // Useful for validating a presented refresh token without storing or querying its raw value.
        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
        }

        // Retrieves refresh tokens associated with an access token JWT ID.
        // Useful for investigating refresh-token records linked to a particular access-token identity.
        public async Task<IEnumerable<RefreshToken>> GetByJwtIdAsync(string jwtId)
        {
            return await _dbSet
                .Where(x => x.JwtId == jwtId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Checks whether a token hash already exists.
        // Useful for preventing duplicate refresh-token records.
        public async Task<bool> ExistsByTokenHashAsync(string tokenHash)
        {
            return await _dbSet.AnyAsync(x => x.TokenHash == tokenHash);
        }

        // =========================================================
        // 2. USER TOKEN MANAGEMENT
        // =========================================================

        // Retrieves all refresh tokens belonging to a user.
        // Useful for reviewing the user's registered sessions and devices.
        public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves all refresh tokens for a user with their replacement-token details.
        // Useful for investigating refresh-token rotation history across user sessions.
        public async Task<IEnumerable<RefreshToken>> GetUserTokensWithReplacementHistoryAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.ReplacedByToken)
                .Include(x => x.ReplacedTokens)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves a user's non-revoked and unexpired refresh tokens.
        // Useful for identifying sessions that may still be eligible for refresh.
        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(int userId, DateTime currentDateTime)
        {
            return await _dbSet
                .Where(x => x.UserId == userId &&
                            x.RevokedAt == null &&
                            x.ExpiresAt > currentDateTime)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Counts non-revoked and unexpired refresh tokens for a user.
        // Useful for monitoring the number of active sessions associated with an account.
        public async Task<int> CountActiveTokensByUserIdAsync(int userId, DateTime currentDateTime)
        {
            return await _dbSet.CountAsync(x =>
                x.UserId == userId &&
                x.RevokedAt == null &&
                x.ExpiresAt > currentDateTime);
        }

        // =========================================================
        // 3. TOKEN EXPIRY
        // =========================================================

        // Retrieves refresh tokens that have expired.
        // Useful for identifying records that are no longer eligible for token refresh.
        public async Task<IEnumerable<RefreshToken>> GetExpiredTokensAsync(DateTime currentDateTime)
        {
            return await _dbSet
                .Where(x => x.ExpiresAt <= currentDateTime)
                .OrderBy(x => x.ExpiresAt)
                .ToListAsync();
        }

        // Retrieves unexpired refresh tokens that have not been revoked.
        // Useful for identifying tokens that remain eligible for refresh based on stored state.
        public async Task<IEnumerable<RefreshToken>> GetUnexpiredTokensAsync(DateTime currentDateTime)
        {
            return await _dbSet
                .Where(x => x.ExpiresAt > currentDateTime && x.RevokedAt == null)
                .OrderBy(x => x.ExpiresAt)
                .ToListAsync();
        }

        // Retrieves tokens expiring within a supplied date range.
        // Useful for monitoring upcoming session expiry.
        public async Task<IEnumerable<RefreshToken>> GetTokensExpiringWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.ExpiresAt >= startDate && x.ExpiresAt <= endDate)
                .OrderBy(x => x.ExpiresAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. REVOCATION
        // =========================================================

        // Retrieves refresh tokens that have been revoked.
        // Useful for reviewing sessions invalidated by users or security processes.
        public async Task<IEnumerable<RefreshToken>> GetRevokedTokensAsync()
        {
            return await _dbSet
                .Where(x => x.RevokedAt.HasValue)
                .OrderByDescending(x => x.RevokedAt)
                .ToListAsync();
        }

        // Retrieves refresh tokens that have not been revoked.
        // Useful for reviewing tokens that have no recorded revocation event.
        public async Task<IEnumerable<RefreshToken>> GetNonRevokedTokensAsync()
        {
            return await _dbSet
                .Where(x => x.RevokedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves revoked tokens belonging to a particular user.
        // Useful for investigating a user's session termination history.
        public async Task<IEnumerable<RefreshToken>> GetRevokedTokensByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId && x.RevokedAt.HasValue)
                .OrderByDescending(x => x.RevokedAt)
                .ToListAsync();
        }

        // Retrieves tokens revoked within a supplied date range.
        // Useful for security reporting and reviewing session revocation activity.
        public async Task<IEnumerable<RefreshToken>> GetRevokedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.RevokedAt.HasValue &&
                            x.RevokedAt.Value >= startDate &&
                            x.RevokedAt.Value <= endDate)
                .OrderByDescending(x => x.RevokedAt)
                .ToListAsync();
        }

        // =========================================================
        // 5. TOKEN ROTATION
        // =========================================================

        // Retrieves the token that replaced a particular refresh token.
        // Useful for tracing the next token issued during refresh-token rotation.
        public async Task<RefreshToken?> GetReplacementTokenAsync(int refreshTokenId)
        {
            return await _dbSet
                .Where(x => x.RefreshTokenId == refreshTokenId)
                .Select(x => x.ReplacedByToken)
                .FirstOrDefaultAsync();
        }

        // Retrieves tokens that replaced a particular refresh token.
        // Useful for reviewing the token rotation chain.
        public async Task<IEnumerable<RefreshToken>> GetReplacedTokensAsync(int refreshTokenId)
        {
            return await _dbSet
                .Where(x => x.ReplacedByTokenId == refreshTokenId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves refresh tokens that have been replaced by another token.
        // Useful for identifying previous tokens in a rotation sequence.
        public async Task<IEnumerable<RefreshToken>> GetTokensWithReplacementAsync()
        {
            return await _dbSet
                .Where(x => x.ReplacedByTokenId.HasValue)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves refresh tokens that have not been replaced by another token.
        // Useful for identifying the latest recorded token in each rotation chain.
        public async Task<IEnumerable<RefreshToken>> GetTokensWithoutReplacementAsync()
        {
            return await _dbSet
                .Where(x => x.ReplacedByTokenId == null)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 6. CLIENT AND DEVICE INFORMATION
        // =========================================================

        // Retrieves refresh tokens created from a particular IP address.
        // Useful for reviewing sessions originating from a specific client network address.
        public async Task<IEnumerable<RefreshToken>> GetByCreatedByIpAsync(string ipAddress)
        {
            return await _dbSet
                .Where(x => x.CreatedByIp == ipAddress)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves refresh tokens revoked from a particular IP address.
        // Useful for investigating where session revocation requests originated.
        public async Task<IEnumerable<RefreshToken>> GetByRevokedByIpAsync(string ipAddress)
        {
            return await _dbSet
                .Where(x => x.RevokedByIp == ipAddress)
                .OrderByDescending(x => x.RevokedAt)
                .ToListAsync();
        }

        // Retrieves refresh tokens associated with a particular device name.
        // Useful for reviewing sessions registered against a named device.
        public async Task<IEnumerable<RefreshToken>> GetByDeviceNameAsync(string deviceName)
        {
            return await _dbSet
                .Where(x => x.DeviceName == deviceName)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves refresh tokens created within a supplied date range.
        // Useful for reviewing session creation activity.
        public async Task<IEnumerable<RefreshToken>> GetCreatedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
    }
}
