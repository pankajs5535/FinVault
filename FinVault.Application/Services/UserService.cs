
using AutoMapper;
using FinVault.Application.DTOs.UserDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHashService _passwordHashService;

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHashService passwordHashService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHashService = passwordHashService;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<UserDto>> GetAllAsync()
        => MapUsers(await _unitOfWork.User.GetAllAsync());

    public async Task<UserDto?> GetByIdAsync(int userId)
    {
        if (userId <= 0) return null;
        return MapUser(await _unitOfWork.User.GetByIdAsync(userId));
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        ValidateText(username, nameof(username));
        return MapUser(await _unitOfWork.User.GetByUsernameAsync(username.Trim()));
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        ValidateText(email, nameof(email));
        return MapUser(await _unitOfWork.User.GetByEmailAsync(email.Trim()));
    }

    public async Task<UserDto?> GetByPhoneNumberAsync(string phoneNumber)
    {
        ValidateText(phoneNumber, nameof(phoneNumber));
        return MapUser(await _unitOfWork.User.GetByPhoneNumberAsync(phoneNumber.Trim()));
    }

    public async Task<UserDto?> GetUserDetailsAsync(int userId)
    {
        if (userId <= 0) return null;
        return MapUser(await _unitOfWork.User.GetUserDetailsAsync(userId));
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateCreateDto(dto);

        if (await IsUsernameExistsAsync(dto.Username))
            throw new InvalidOperationException("Username already exists.");

        if (await IsEmailExistsAsync(dto.Email))
            throw new InvalidOperationException("Email already exists.");

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) &&
            await IsPhoneNumberExistsAsync(dto.PhoneNumber))
            throw new InvalidOperationException("Phone number already exists.");

        if (await _unitOfWork.Role.GetByIdAsync(dto.RoleId) == null)
            throw new InvalidOperationException("The specified role does not exist.");

        var user = _mapper.Map<User>(dto);

        user.Username = dto.Username.Trim();
        user.Email = dto.Email.Trim();
        user.PhoneNumber = NormalizePhone(dto.PhoneNumber);
        user.PasswordHash = _passwordHashService.HashPassword(user, dto.Password);
        user.PasswordChangedAt = DateTime.UtcNow;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.FailedLoginAttempts = 0;
        user.LockoutEndAt = null;
        user.TwoFactorSecretEncrypted = null;
        user.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.User.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateAsync(UpdateUserDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateUpdateDto(dto);

        var user = await _unitOfWork.User.GetByIdAsync(dto.UserId);
        if (user == null) return null;

        if (dto.RowVersion == null || !dto.RowVersion.SequenceEqual(user.RowVersion))
            throw new InvalidOperationException(
                "The user record has changed. Reload it before updating.");

        var username = dto.Username.Trim();
        var email = dto.Email.Trim();
        var phone = NormalizePhone(dto.PhoneNumber);

        var existingUsername = await _unitOfWork.User.FindAsync(
            u => u.Username == username && u.UserId != dto.UserId);

        if (existingUsername.Any())
            throw new InvalidOperationException("Username already exists.");

        var existingEmail = await _unitOfWork.User.FindAsync(
            u => u.Email == email && u.UserId != dto.UserId);

        if (existingEmail.Any())
            throw new InvalidOperationException("Email already exists.");

        if (phone != null)
        {
            var existingPhone = await _unitOfWork.User.FindAsync(
                u => u.PhoneNumber == phone && u.UserId != dto.UserId);

            if (existingPhone.Any())
                throw new InvalidOperationException("Phone number already exists.");
        }

        if (await _unitOfWork.Role.GetByIdAsync(dto.RoleId) == null)
            throw new InvalidOperationException("The specified role does not exist.");

        bool roleChanged = user.RoleId != dto.RoleId;

        user.Username = username;
        user.Email = email;
        user.PhoneNumber = phone;
        user.UserType = dto.UserType.Trim();
        user.IsActive = dto.IsActive;
        user.IsEmailVerified = dto.IsEmailVerified;
        user.IsPhoneVerified = dto.IsPhoneVerified;
        user.IsTwoFactorEnabled = dto.IsTwoFactorEnabled;
        user.RoleId = dto.RoleId;
        user.UpdatedAt = DateTime.UtcNow;

        if (roleChanged)
            user.SecurityStamp = Guid.NewGuid().ToString("N");

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteAsync(DeleteUserDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.UserId <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.UserId));

        if (!await CanUserBeDeletedAsync(dto.UserId))
            return false;

        var user = await _unitOfWork.User.GetByIdAsync(dto.UserId);
        if (user == null) return false;

        _unitOfWork.User.Delete(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. USER TYPE AND ACCOUNT STATUS

    public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
        => MapUsers(await _unitOfWork.User.GetActiveUsersAsync());

    public async Task<IEnumerable<UserDto>> GetInactiveUsersAsync()
        => MapUsers(await _unitOfWork.User.GetInactiveUsersAsync());

    public async Task<IEnumerable<UserDto>> GetByUserTypeAsync(string userType)
    {
        ValidateText(userType, nameof(userType));
        return MapUsers(await _unitOfWork.User.GetByUserTypeAsync(userType.Trim()));
    }

    public async Task<IEnumerable<UserDto>> GetByRoleIdAsync(int roleId)
    {
        ValidateId(roleId, nameof(roleId));
        return MapUsers(await _unitOfWork.User.GetByRoleIdAsync(roleId));
    }

    public async Task<bool> ActivateAsync(int userId)
        => await SetActiveStatusAsync(userId, true);

    public async Task<bool> DeactivateAsync(int userId)
        => await SetActiveStatusAsync(userId, false);

    // 3. LOGIN SECURITY AND LOCKOUT

    public async Task<IEnumerable<UserDto>> GetCurrentlyLockedUsersAsync(DateTime currentDateTime)
        => MapUsers(await _unitOfWork.User.GetCurrentlyLockedUsersAsync(currentDateTime));

    public async Task<IEnumerable<UserDto>> GetUsersWithExpiredLockoutAsync(DateTime currentDateTime)
        => MapUsers(await _unitOfWork.User.GetUsersWithExpiredLockoutAsync(currentDateTime));

    public async Task<IEnumerable<UserDto>> GetUsersWithFailedLoginAttemptsAsync(int minimumAttempts)
    {
        if (minimumAttempts < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumAttempts));

        return MapUsers(
            await _unitOfWork.User.GetUsersWithFailedLoginAttemptsAsync(minimumAttempts));
    }

    public async Task<IEnumerable<UserDto>> GetUsersWithFailedLoginAttemptsBelowAsync(int maximumAttempts)
    {
        if (maximumAttempts < 0)
            throw new ArgumentOutOfRangeException(nameof(maximumAttempts));

        return MapUsers(
            await _unitOfWork.User.GetUsersWithFailedLoginAttemptsBelowAsync(maximumAttempts));
    }

    public async Task<bool> RecordFailedLoginAttemptAsync(int userId)
    {
        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.FailedLoginAttempts = checked(user.FailedLoginAttempts + 1);
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ResetFailedLoginAttemptsAsync(int userId)
    {
        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.FailedLoginAttempts = 0;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LockAccountAsync(int userId, DateTime lockoutEndAt)
    {
        if (lockoutEndAt <= DateTime.UtcNow)
            throw new ArgumentException("Lockout end time must be in the future.");

        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.LockoutEndAt = lockoutEndAt;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UnlockAccountAsync(int userId)
    {
        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.LockoutEndAt = null;
        user.FailedLoginAttempts = 0;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 4. EMAIL, PHONE AND MULTI-FACTOR VERIFICATION

    public async Task<IEnumerable<UserDto>> GetEmailVerifiedUsersAsync()
        => MapUsers(await _unitOfWork.User.GetEmailVerifiedUsersAsync());

    public async Task<IEnumerable<UserDto>> GetEmailUnverifiedUsersAsync()
        => MapUsers(await _unitOfWork.User.GetEmailUnverifiedUsersAsync());

    public async Task<IEnumerable<UserDto>> GetPhoneVerifiedUsersAsync()
        => MapUsers(await _unitOfWork.User.GetPhoneVerifiedUsersAsync());

    public async Task<IEnumerable<UserDto>> GetPhoneUnverifiedUsersAsync()
        => MapUsers(await _unitOfWork.User.GetPhoneUnverifiedUsersAsync());

    public async Task<IEnumerable<UserDto>> GetTwoFactorEnabledUsersAsync()
        => MapUsers(await _unitOfWork.User.GetTwoFactorEnabledUsersAsync());

    public async Task<IEnumerable<UserDto>> GetTwoFactorDisabledUsersAsync()
        => MapUsers(await _unitOfWork.User.GetTwoFactorDisabledUsersAsync());

    public async Task<bool> VerifyEmailAsync(int userId)
    {
        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.IsEmailVerified = true;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> VerifyPhoneNumberAsync(int userId)
    {
        var user = await FindUserAsync(userId);
        if (user == null) return false;

        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            throw new InvalidOperationException("The user has no registered phone number.");

        user.IsPhoneVerified = true;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EnableTwoFactorAuthenticationAsync(int userId, string encryptedSecret)
    {
        ValidateText(encryptedSecret, nameof(encryptedSecret));

        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.TwoFactorSecretEncrypted = encryptedSecret;
        user.IsTwoFactorEnabled = true;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DisableTwoFactorAuthenticationAsync(int userId)
    {
        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.IsTwoFactorEnabled = false;
        user.TwoFactorSecretEncrypted = null;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 5. CUSTOMER AND EMPLOYEE RELATIONSHIPS

    public async Task<IEnumerable<UserDto>> GetUsersWithCustomerProfilesAsync()
        => MapUsers(await _unitOfWork.User.GetUsersWithCustomerProfilesAsync());

    public async Task<IEnumerable<UserDto>> GetUsersWithEmployeeProfilesAsync()
        => MapUsers(await _unitOfWork.User.GetUsersWithEmployeeProfilesAsync());

    public async Task<UserDto?> GetUserWithCustomerAsync(int userId)
    {
        if (userId <= 0) return null;
        return MapUser(await _unitOfWork.User.GetUserWithCustomerAsync(userId));
    }

    public async Task<UserDto?> GetUserWithEmployeeAsync(int userId)
    {
        if (userId <= 0) return null;
        return MapUser(await _unitOfWork.User.GetUserWithEmployeeAsync(userId));
    }

    // 6. PASSWORD AND LOGIN HISTORY

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        ValidatePassword(newPassword, nameof(newPassword));
        ValidateText(currentPassword, nameof(currentPassword));

        var user = await FindUserAsync(userId);
        if (user == null) return false;

        if (!_passwordHashService.VerifyPassword(user, currentPassword))
            return false;

        user.PasswordHash = _passwordHashService.HashPassword(user, newPassword);
        user.PasswordChangedAt = DateTime.UtcNow;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
    {
        ValidatePassword(newPassword, nameof(newPassword));

        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.PasswordHash = _passwordHashService.HashPassword(user, newPassword);
        user.PasswordChangedAt = DateTime.UtcNow;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.FailedLoginAttempts = 0;
        user.LockoutEndAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RecordSuccessfulLoginAsync(int userId, string? ipAddress)
    {
        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.LastLoginAt = DateTime.UtcNow;
        user.LastLoginIp = string.IsNullOrWhiteSpace(ipAddress) ? null : ipAddress.Trim();
        user.FailedLoginAttempts = 0;
        user.LockoutEndAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdatePasswordChangedAtAsync(int userId)
    {
        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.PasswordChangedAt = DateTime.UtcNow;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<UserDto>> GetUsersByLastLoginRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);
        return MapUsers(await _unitOfWork.User.GetUsersByLastLoginRangeAsync(startDate, endDate));
    }

    public async Task<IEnumerable<UserDto>> GetUsersWhoNeverLoggedInAsync()
        => MapUsers(await _unitOfWork.User.GetUsersWhoNeverLoggedInAsync());

    public async Task<IEnumerable<UserDto>> GetUsersByPasswordChangedRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);
        return MapUsers(await _unitOfWork.User.GetUsersByPasswordChangedRangeAsync(startDate, endDate));
    }

    public async Task<IEnumerable<UserDto>> GetUsersCreatedWithinRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);
        return MapUsers(await _unitOfWork.User.GetUsersCreatedWithinRangeAsync(startDate, endDate));
    }

    // 7. RELATED RECORDS AND SECURITY INVESTIGATION

    public async Task<UserDto?> GetUserWithRefreshTokensAsync(int userId)
    {
        if (userId <= 0) return null;
        return MapUser(await _unitOfWork.User.GetUserWithRefreshTokensAsync(userId));
    }

    public async Task<UserDto?> GetUserWithInitiatedTransactionsAsync(int userId)
    {
        if (userId <= 0) return null;
        return MapUser(await _unitOfWork.User.GetUserWithInitiatedTransactionsAsync(userId));
    }

    public async Task<UserDto?> GetUserWithNotificationsAsync(int userId)
    {
        if (userId <= 0) return null;
        return MapUser(await _unitOfWork.User.GetUserWithNotificationsAsync(userId));
    }

    public async Task<UserDto?> GetUserWithAuditLogsAsync(int userId)
    {
        if (userId <= 0) return null;
        return MapUser(await _unitOfWork.User.GetUserWithAuditLogsAsync(userId));
    }

    // 8. SEARCH AND REPORTING

    public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm)
    {
        ValidateText(searchTerm, nameof(searchTerm));
        return MapUsers(await _unitOfWork.User.SearchUsersAsync(searchTerm.Trim()));
    }

    public Task<int> CountActiveUsersByRoleAsync(int roleId)
    {
        ValidateId(roleId, nameof(roleId));
        return _unitOfWork.User.CountActiveUsersByRoleAsync(roleId);
    }

    public Task<int> CountByUserTypeAndStatusAsync(string userType, bool isActive)
    {
        ValidateText(userType, nameof(userType));
        return _unitOfWork.User.CountByUserTypeAndStatusAsync(userType.Trim(), isActive);
    }

    // 9. VALIDATION

    public Task<bool> IsUsernameExistsAsync(string username)
    {
        ValidateText(username, nameof(username));
        return _unitOfWork.User.ExistsByUsernameAsync(username.Trim());
    }

    public Task<bool> IsEmailExistsAsync(string email)
    {
        ValidateText(email, nameof(email));
        return _unitOfWork.User.ExistsByEmailAsync(email.Trim());
    }

    public Task<bool> IsPhoneNumberExistsAsync(string phoneNumber)
    {
        ValidateText(phoneNumber, nameof(phoneNumber));
        return _unitOfWork.User.ExistsByPhoneNumberAsync(phoneNumber.Trim());
    }

    public async Task<bool> CanUserBeDeletedAsync(int userId)
    {
        if (userId <= 0) return false;

        var user = await _unitOfWork.User.GetByIdAsync(userId);
        if (user == null) return false;

        // Preserve users referenced by financial, customer, employee,
        // authentication, notification, reporting, AI or audit records.
        return
            !await _unitOfWork.Customer.ExistsAsync(x => x.UserId == userId) &&
            !await _unitOfWork.Employee.ExistsAsync(x => x.UserId == userId) &&
            !await _unitOfWork.Transaction.ExistsAsync(x => x.InitiatedByUserId == userId) &&
            !await _unitOfWork.AuditLog.ExistsAsync(x => x.UserId == userId) &&
            !await _unitOfWork.RefreshToken.ExistsAsync(x => x.UserId == userId) &&
            !await _unitOfWork.Notification.ExistsAsync(x => x.UserId == userId) &&
            !await _unitOfWork.ReportConfiguration.ExistsAsync(x => x.OwnerUserId == userId) &&
            !await _unitOfWork.AIConversation.ExistsAsync(x => x.UserId == userId) &&
            !await _unitOfWork.AIUsageLog.ExistsAsync(x => x.UserId == userId);
    }

    // PRIVATE HELPERS

    private async Task<User?> FindUserAsync(int userId)
    {
        ValidateId(userId, nameof(userId));
        return await _unitOfWork.User.GetByIdAsync(userId);
    }

    private async Task<bool> SetActiveStatusAsync(int userId, bool isActive)
    {
        var user = await FindUserAsync(userId);
        if (user == null) return false;

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;

        if (!isActive)
            user.SecurityStamp = Guid.NewGuid().ToString("N");

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private UserDto? MapUser(User? user)
        => user == null ? null : _mapper.Map<UserDto>(user);

    private IEnumerable<UserDto> MapUsers(IEnumerable<User> users)
        => _mapper.Map<IEnumerable<UserDto>>(users);

    private static void ValidateCreateDto(CreateUserDto dto)
    {
        ValidateText(dto.Username, nameof(dto.Username));
        ValidateText(dto.Email, nameof(dto.Email));
        ValidateText(dto.UserType, nameof(dto.UserType));
        ValidatePassword(dto.Password, nameof(dto.Password));
        ValidateId(dto.RoleId, nameof(dto.RoleId));

        if (dto.Username.Trim().Length > 50)
            throw new ArgumentException("Username cannot exceed 50 characters.");

        if (dto.Email.Trim().Length > 256)
            throw new ArgumentException("Email cannot exceed 256 characters.");

        if (!IsValidUserType(dto.UserType))
            throw new ArgumentException("Invalid user type.");
    }

    private static void ValidateUpdateDto(UpdateUserDto dto)
    {
        ValidateId(dto.UserId, nameof(dto.UserId));
        ValidateId(dto.RoleId, nameof(dto.RoleId));
        ValidateText(dto.Username, nameof(dto.Username));
        ValidateText(dto.Email, nameof(dto.Email));
        ValidateText(dto.UserType, nameof(dto.UserType));

        if (!IsValidUserType(dto.UserType))
            throw new ArgumentException("Invalid user type.");

        if (dto.RowVersion == null || dto.RowVersion.Length == 0)
            throw new ArgumentException("A valid RowVersion is required.");
    }

    private static bool IsValidUserType(string userType)
        => new[] { "Customer", "Employee", "Admin", "System" }
            .Contains(userType.Trim(), StringComparer.OrdinalIgnoreCase);

    private static void ValidatePassword(string password, string parameterName)
    {
        ValidateText(password, parameterName);

        if (password.Length < 8 || password.Length > 128)
            throw new ArgumentException("Password must contain between 8 and 128 characters.", parameterName);
    }

    private static void ValidateText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A value is required.", parameterName);
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(parameterName, "ID must be greater than zero.");
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be later than end date.");
    }

    private static string? NormalizePhone(string? phoneNumber)
        => string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
}
