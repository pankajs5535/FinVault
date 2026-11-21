
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // =========================================================
        // 1. USER IDENTIFICATION
        // =========================================================

        // Retrieves a user using the unique username.
        // Useful for locating the account during sign-in.
        Task<User?> GetByUsernameAsync(string username);

        // Retrieves a user using the unique email address.
        // Useful for email-based sign-in and account recovery.
        Task<User?> GetByEmailAsync(string email);

        // Retrieves a user using the registered phone number.
        // Useful for phone-based account verification and recovery.
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);

        // Retrieves user details with role and linked customer or employee records.
        // Useful for reviewing the identity and banking profile associated with a login.
        Task<User?> GetUserDetailsAsync(int userId);

        // Checks whether a username already exists.
        // Useful for preventing duplicate login identities.
        Task<bool> ExistsByUsernameAsync(string username);

        // Checks whether an email address already exists.
        // Useful for preventing duplicate email-based login identities.
        Task<bool> ExistsByEmailAsync(string email);

        // Checks whether a phone number already exists.
        // Useful for preventing duplicate phone registrations.
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber);

        // =========================================================
        // 2. ACCOUNT STATUS AND USER TYPE
        // =========================================================

        // Retrieves all active users.
        // Useful for identifying accounts currently enabled to access the application.
        Task<IEnumerable<User>> GetActiveUsersAsync();

        // Retrieves all inactive users.
        // Useful for reviewing accounts disabled by administrators or account lifecycle processes.
        Task<IEnumerable<User>> GetInactiveUsersAsync();

        // Retrieves users belonging to a particular user type.
        // Useful for separating customer, employee, administrator and system identities.
        Task<IEnumerable<User>> GetByUserTypeAsync(string userType);

        // Retrieves users assigned to a particular role.
        // Useful for reviewing the users who hold a banking access role.
        Task<IEnumerable<User>> GetByRoleIdAsync(int roleId);

        // =========================================================
        // 3. LOGIN SECURITY AND LOCKOUT
        // =========================================================

        // Retrieves users whose lockout period is still active.
        // Useful for monitoring accounts temporarily blocked after failed login attempts.
        Task<IEnumerable<User>> GetCurrentlyLockedUsersAsync(DateTime currentDateTime);

        // Retrieves users whose lockout period has expired.
        // Useful for identifying accounts eligible for lockout recovery.
        Task<IEnumerable<User>> GetUsersWithExpiredLockoutAsync(DateTime currentDateTime);

        // Retrieves users with failed login attempts at or above a threshold.
        // Useful for identifying repeated unsuccessful sign-in activity.
        Task<IEnumerable<User>> GetUsersWithFailedLoginAttemptsAsync(int minimumAttempts);

        // Retrieves users whose failed login attempts are at or below a threshold.
        // Useful for reviewing login activity before an account reaches its lockout threshold.
        Task<IEnumerable<User>> GetUsersWithFailedLoginAttemptsBelowAsync(int maximumAttempts);

        // =========================================================
        // 4. VERIFICATION AND MULTI-FACTOR AUTHENTICATION
        // =========================================================

        // Retrieves users whose email addresses are verified.
        // Useful for identifying accounts that have completed email verification.
        Task<IEnumerable<User>> GetEmailVerifiedUsersAsync();

        // Retrieves users whose email addresses are not verified.
        // Useful for identifying accounts requiring email verification.
        Task<IEnumerable<User>> GetEmailUnverifiedUsersAsync();

        // Retrieves users whose phone numbers are verified.
        // Useful for identifying accounts that have completed phone verification.
        Task<IEnumerable<User>> GetPhoneVerifiedUsersAsync();

        // Retrieves users whose phone numbers are not verified.
        // Useful for identifying accounts requiring phone verification.
        Task<IEnumerable<User>> GetPhoneUnverifiedUsersAsync();

        // Retrieves users with two-factor authentication enabled.
        // Useful for monitoring accounts protected by an additional authentication factor.
        Task<IEnumerable<User>> GetTwoFactorEnabledUsersAsync();

        // Retrieves users without two-factor authentication enabled.
        // Useful for identifying accounts that may require stronger authentication policies.
        Task<IEnumerable<User>> GetTwoFactorDisabledUsersAsync();

        // =========================================================
        // 5. CUSTOMER AND EMPLOYEE RELATIONSHIPS
        // =========================================================

        // Retrieves users linked to customer profiles.
        // Useful for reviewing customer login identities and their associated profiles.
        Task<IEnumerable<User>> GetUsersWithCustomerProfilesAsync();

        // Retrieves users linked to employee profiles.
        // Useful for reviewing staff login identities and their employment records.
        Task<IEnumerable<User>> GetUsersWithEmployeeProfilesAsync();

        // Retrieves a user with the linked customer profile.
        // Useful for resolving a customer's login identity to their banking profile.
        Task<User?> GetUserWithCustomerAsync(int userId);

        // Retrieves a user with the linked employee profile.
        // Useful for resolving an employee's login identity to their staff record.
        Task<User?> GetUserWithEmployeeAsync(int userId);

        // =========================================================
        // 6. LOGIN AND PASSWORD HISTORY
        // =========================================================

        // Retrieves users who logged in within a date range.
        // Useful for reviewing recent account access activity.
        Task<IEnumerable<User>> GetUsersByLastLoginRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves users who have never logged in.
        // Useful for identifying newly created or unused login accounts.
        Task<IEnumerable<User>> GetUsersWhoNeverLoggedInAsync();

        // Retrieves users whose passwords were changed within a date range.
        // Useful for reviewing credential maintenance activity.
        Task<IEnumerable<User>> GetUsersByPasswordChangedRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves users created within a date range.
        // Useful for reporting newly registered customer and employee login identities.
        Task<IEnumerable<User>> GetUsersCreatedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 7. RELATED RECORDS AND AUDIT
        // =========================================================

        // Retrieves a user with their refresh tokens.
        // Useful for reviewing active and historical token records during security investigations.
        Task<User?> GetUserWithRefreshTokensAsync(int userId);

        // Retrieves a user with their initiated transactions.
        // Useful for investigating which login identity initiated banking transactions.
        Task<User?> GetUserWithInitiatedTransactionsAsync(int userId);

        // Retrieves a user with their notifications.
        // Useful for reviewing communications associated with a login identity.
        Task<User?> GetUserWithNotificationsAsync(int userId);

        // Retrieves a user with their audit logs.
        // Useful for reviewing actions recorded against a particular login identity.
        Task<User?> GetUserWithAuditLogsAsync(int userId);

        // =========================================================
        // 8. SEARCH AND REPORTING
        // =========================================================

        // Searches users by username, email or phone number.
        // Useful for locating login identities during administration and customer support.
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);

        // Counts active users assigned to a particular role.
        // Useful for reporting the number of enabled accounts under each banking role.
        Task<int> CountActiveUsersByRoleAsync(int roleId);

        // Counts users by user type and account status.
        // Useful for reporting active customer, employee, administrator and system identities.
        Task<int> CountByUserTypeAndStatusAsync(string userType, bool isActive);
    }
}
