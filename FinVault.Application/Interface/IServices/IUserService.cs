using FinVault.Application.DTOs.UserDtos;
using System.Security.Cryptography;

namespace FinVault.Application.Interfaces.Services
{
    public interface IUserService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int userId);
        Task<UserDto?> GetByUsernameAsync(string username);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto?> GetByPhoneNumberAsync(string phoneNumber);
        Task<UserDto?> GetUserDetailsAsync(int userId);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto?> UpdateAsync(UpdateUserDto dto);
        Task<bool> DeleteAsync(DeleteUserDto dto);

        // 2. USER TYPE AND ACCOUNT STATUS
        Task<IEnumerable<UserDto>> GetActiveUsersAsync();
        Task<IEnumerable<UserDto>> GetInactiveUsersAsync();
        Task<IEnumerable<UserDto>> GetByUserTypeAsync(string userType);
        Task<IEnumerable<UserDto>> GetByRoleIdAsync(int roleId);
        Task<bool> ActivateAsync(int userId);
        Task<bool> DeactivateAsync(int userId);

        // 3. LOGIN SECURITY AND LOCKOUT
        Task<IEnumerable<UserDto>> GetCurrentlyLockedUsersAsync(DateTime currentDateTime);
        Task<IEnumerable<UserDto>> GetUsersWithExpiredLockoutAsync(DateTime currentDateTime);
        Task<IEnumerable<UserDto>> GetUsersWithFailedLoginAttemptsAsync(int minimumAttempts);
        Task<IEnumerable<UserDto>> GetUsersWithFailedLoginAttemptsBelowAsync(int maximumAttempts);
        Task<bool> RecordFailedLoginAttemptAsync(int userId);
        Task<bool> ResetFailedLoginAttemptsAsync(int userId);
        Task<bool> LockAccountAsync(int userId, DateTime lockoutEndAt);
        Task<bool> UnlockAccountAsync(int userId);

        // 4. EMAIL, PHONE AND MULTI-FACTOR VERIFICATION
        Task<IEnumerable<UserDto>> GetEmailVerifiedUsersAsync();
        Task<IEnumerable<UserDto>> GetEmailUnverifiedUsersAsync();
        Task<IEnumerable<UserDto>> GetPhoneVerifiedUsersAsync();
        Task<IEnumerable<UserDto>> GetPhoneUnverifiedUsersAsync();
        Task<IEnumerable<UserDto>> GetTwoFactorEnabledUsersAsync();
        Task<IEnumerable<UserDto>> GetTwoFactorDisabledUsersAsync();
        Task<bool> VerifyEmailAsync(int userId);
        Task<bool> VerifyPhoneNumberAsync(int userId);
        Task<bool> EnableTwoFactorAuthenticationAsync(int userId, string encryptedSecret);
        Task<bool> DisableTwoFactorAuthenticationAsync(int userId);

        // 5. CUSTOMER AND EMPLOYEE RELATIONSHIPS
        Task<IEnumerable<UserDto>> GetUsersWithCustomerProfilesAsync();
        Task<IEnumerable<UserDto>> GetUsersWithEmployeeProfilesAsync();
        Task<UserDto?> GetUserWithCustomerAsync(int userId);
        Task<UserDto?> GetUserWithEmployeeAsync(int userId);

        // 6. PASSWORD AND LOGIN HISTORY
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
        Task<bool> RecordSuccessfulLoginAsync(int userId, string? ipAddress);
        Task<bool> UpdatePasswordChangedAtAsync(int userId);
        Task<IEnumerable<UserDto>> GetUsersByLastLoginRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<UserDto>> GetUsersWhoNeverLoggedInAsync();
        Task<IEnumerable<UserDto>> GetUsersByPasswordChangedRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<UserDto>> GetUsersCreatedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 7. RELATED RECORDS AND SECURITY INVESTIGATION
        Task<UserDto?> GetUserWithRefreshTokensAsync(int userId);
        Task<UserDto?> GetUserWithInitiatedTransactionsAsync(int userId);
        Task<UserDto?> GetUserWithNotificationsAsync(int userId);
        Task<UserDto?> GetUserWithAuditLogsAsync(int userId);

        // 8. SEARCH AND REPORTING
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);
        Task<int> CountActiveUsersByRoleAsync(int roleId);
        Task<int> CountByUserTypeAndStatusAsync(string userType, bool isActive);

        // 9. VALIDATION
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber);
        Task<bool> CanUserBeDeletedAsync(int userId);
    }
}


//Banking security rules:
//-Never expose PasswordHash, SecurityStamp, or encrypted 2FA secrets in UserDto.
//- Hash passwords using a secure password - hashing algorithm; never store plaintext passwords.
//- Rotate SecurityStamp after password or role changes to invalidate existing sessions/tokens.
//- Enforce lockout policies and verify the user's identity before password recovery.
//- Prevent deletion of users referenced by financial transactions, audit logs, customers, or employees.