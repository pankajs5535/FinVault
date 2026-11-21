
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. USER IDENTIFICATION
        // =========================================================

        // Retrieves a user using the unique username.
        // Useful for locating the account during sign-in.
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Username == username);
        }

        // Retrieves a user using the unique email address.
        // Useful for email-based sign-in and account recovery.
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        }

        // Retrieves a user using the registered phone number.
        // Useful for phone-based account verification and recovery.
        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
        }

        // Retrieves user details with role and linked customer or employee records.
        // Useful for reviewing the identity and banking profile associated with a login.
        public async Task<User?> GetUserDetailsAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.Role)
                .Include(x => x.Customer)
                .Include(x => x.Employee)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // Checks whether a username already exists.
        // Useful for preventing duplicate login identities.
        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _dbSet.AnyAsync(x => x.Username == username);
        }

        // Checks whether an email address already exists.
        // Useful for preventing duplicate email-based login identities.
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(x => x.Email == email);
        }

        // Checks whether a phone number already exists.
        // Useful for preventing duplicate phone registrations.
        public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbSet.AnyAsync(x => x.PhoneNumber == phoneNumber);
        }

        // =========================================================
        // 2. ACCOUNT STATUS AND USER TYPE
        // =========================================================

        // Retrieves all active users.
        // Useful for identifying accounts currently enabled to access the application.
        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // Retrieves all inactive users.
        // Useful for reviewing accounts disabled by administrators or account lifecycle processes.
        public async Task<IEnumerable<User>> GetInactiveUsersAsync()
        {
            return await _dbSet
                .Where(x => !x.IsActive)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // Retrieves users belonging to a particular user type.
        // Useful for separating customer, employee, administrator and system identities.
        public async Task<IEnumerable<User>> GetByUserTypeAsync(string userType)
        {
            return await _dbSet
                .Where(x => x.UserType == userType)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // Retrieves users assigned to a particular role.
        // Useful for reviewing the users who hold a banking access role.
        public async Task<IEnumerable<User>> GetByRoleIdAsync(int roleId)
        {
            return await _dbSet
                .Where(x => x.RoleId == roleId)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // =========================================================
        // 3. LOGIN SECURITY AND LOCKOUT
        // =========================================================

        // Retrieves users whose lockout period is still active.
        // Useful for monitoring accounts temporarily blocked after failed login attempts.
        public async Task<IEnumerable<User>> GetCurrentlyLockedUsersAsync(DateTime currentDateTime)
        {
            return await _dbSet
                .Where(x => x.LockoutEndAt.HasValue && x.LockoutEndAt.Value > currentDateTime)
                .OrderBy(x => x.LockoutEndAt)
                .ToListAsync();
        }

        // Retrieves users whose lockout period has expired.
        // Useful for identifying accounts eligible for lockout recovery.
        public async Task<IEnumerable<User>> GetUsersWithExpiredLockoutAsync(DateTime currentDateTime)
        {
            return await _dbSet
                .Where(x => x.LockoutEndAt.HasValue && x.LockoutEndAt.Value <= currentDateTime)
                .OrderBy(x => x.LockoutEndAt)
                .ToListAsync();
        }

        // Retrieves users with failed login attempts at or above a threshold.
        // Useful for identifying repeated unsuccessful sign-in activity.
        public async Task<IEnumerable<User>> GetUsersWithFailedLoginAttemptsAsync(int minimumAttempts)
        {
            return await _dbSet
                .Where(x => x.FailedLoginAttempts >= minimumAttempts)
                .OrderByDescending(x => x.FailedLoginAttempts)
                .ToListAsync();
        }

        // Retrieves users whose failed login attempts are at or below a threshold.
        // Useful for reviewing login activity before an account reaches its lockout threshold.
        public async Task<IEnumerable<User>> GetUsersWithFailedLoginAttemptsBelowAsync(int maximumAttempts)
        {
            return await _dbSet
                .Where(x => x.FailedLoginAttempts <= maximumAttempts)
                .OrderByDescending(x => x.FailedLoginAttempts)
                .ToListAsync();
        }

        // =========================================================
        // 4. VERIFICATION AND MULTI-FACTOR AUTHENTICATION
        // =========================================================

        // Retrieves users whose email addresses are verified.
        // Useful for identifying accounts that have completed email verification.
        public async Task<IEnumerable<User>> GetEmailVerifiedUsersAsync()
        {
            return await _dbSet
                .Where(x => x.IsEmailVerified)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // Retrieves users whose email addresses are not verified.
        // Useful for identifying accounts requiring email verification.
        public async Task<IEnumerable<User>> GetEmailUnverifiedUsersAsync()
        {
            return await _dbSet
                .Where(x => !x.IsEmailVerified)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves users whose phone numbers are verified.
        // Useful for identifying accounts that have completed phone verification.
        public async Task<IEnumerable<User>> GetPhoneVerifiedUsersAsync()
        {
            return await _dbSet
                .Where(x => x.IsPhoneVerified)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // Retrieves users whose phone numbers are not verified.
        // Useful for identifying accounts requiring phone verification.
        public async Task<IEnumerable<User>> GetPhoneUnverifiedUsersAsync()
        {
            return await _dbSet
                .Where(x => !x.IsPhoneVerified)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves users with two-factor authentication enabled.
        // Useful for monitoring accounts protected by an additional authentication factor.
        public async Task<IEnumerable<User>> GetTwoFactorEnabledUsersAsync()
        {
            return await _dbSet
                .Where(x => x.IsTwoFactorEnabled)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // Retrieves users without two-factor authentication enabled.
        // Useful for identifying accounts that may require stronger authentication policies.
        public async Task<IEnumerable<User>> GetTwoFactorDisabledUsersAsync()
        {
            return await _dbSet
                .Where(x => !x.IsTwoFactorEnabled)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // =========================================================
        // 5. CUSTOMER AND EMPLOYEE RELATIONSHIPS
        // =========================================================

        // Retrieves users linked to customer profiles.
        // Useful for reviewing customer login identities and their associated profiles.
        public async Task<IEnumerable<User>> GetUsersWithCustomerProfilesAsync()
        {
            return await _dbSet
                .Where(x => x.Customer != null)
                .Include(x => x.Customer)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // Retrieves users linked to employee profiles.
        // Useful for reviewing staff login identities and their employment records.
        public async Task<IEnumerable<User>> GetUsersWithEmployeeProfilesAsync()
        {
            return await _dbSet
                .Where(x => x.Employee != null)
                .Include(x => x.Employee)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // Retrieves a user with the linked customer profile.
        // Useful for resolving a customer's login identity to their banking profile.
        public async Task<User?> GetUserWithCustomerAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // Retrieves a user with the linked employee profile.
        // Useful for resolving an employee's login identity to their staff record.
        public async Task<User?> GetUserWithEmployeeAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // =========================================================
        // 6. LOGIN AND PASSWORD HISTORY
        // =========================================================

        // Retrieves users who logged in within a date range.
        // Useful for reviewing recent account access activity.
        public async Task<IEnumerable<User>> GetUsersByLastLoginRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.LastLoginAt.HasValue &&
                            x.LastLoginAt.Value >= startDate &&
                            x.LastLoginAt.Value <= endDate)
                .OrderByDescending(x => x.LastLoginAt)
                .ToListAsync();
        }

        // Retrieves users who have never logged in.
        // Useful for identifying newly created or unused login accounts.
        public async Task<IEnumerable<User>> GetUsersWhoNeverLoggedInAsync()
        {
            return await _dbSet
                .Where(x => x.LastLoginAt == null)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves users whose passwords were changed within a date range.
        // Useful for reviewing credential maintenance activity.
        public async Task<IEnumerable<User>> GetUsersByPasswordChangedRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.PasswordChangedAt.HasValue &&
                            x.PasswordChangedAt.Value >= startDate &&
                            x.PasswordChangedAt.Value <= endDate)
                .OrderByDescending(x => x.PasswordChangedAt)
                .ToListAsync();
        }

        // Retrieves users created within a date range.
        // Useful for reporting newly registered customer and employee login identities.
        public async Task<IEnumerable<User>> GetUsersCreatedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 7. RELATED RECORDS AND AUDIT
        // =========================================================

        // Retrieves a user with their refresh tokens.
        // Useful for reviewing active and historical token records during security investigations.
        public async Task<User?> GetUserWithRefreshTokensAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // Retrieves a user with their initiated transactions.
        // Useful for investigating which login identity initiated banking transactions.
        public async Task<User?> GetUserWithInitiatedTransactionsAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.InitiatedTransactions)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // Retrieves a user with their notifications.
        // Useful for reviewing communications associated with a login identity.
        public async Task<User?> GetUserWithNotificationsAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // Retrieves a user with their audit logs.
        // Useful for reviewing actions recorded against a particular login identity.
        public async Task<User?> GetUserWithAuditLogsAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.AuditLogs)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // =========================================================
        // 8. SEARCH AND REPORTING
        // =========================================================

        // Searches users by username, email or phone number.
        // Useful for locating login identities during administration and customer support.
        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.Username.Contains(searchTerm) ||
                            x.Email.Contains(searchTerm) ||
                            (x.PhoneNumber != null && x.PhoneNumber.Contains(searchTerm)))
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // Counts active users assigned to a particular role.
        // Useful for reporting the number of enabled accounts under each banking role.
        public async Task<int> CountActiveUsersByRoleAsync(int roleId)
        {
            return await _dbSet.CountAsync(x => x.RoleId == roleId && x.IsActive);
        }

        // Counts users by user type and account status.
        // Useful for reporting active customer, employee, administrator and system identities.
        public async Task<int> CountByUserTypeAndStatusAsync(string userType, bool isActive)
        {
            return await _dbSet.CountAsync(x => x.UserType == userType && x.IsActive == isActive);
        }
    }
}
