
using FinVault.Application.DTOs.UserDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // 1. CRUD AND IDENTIFICATION

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetAllAsync()));

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetById(int userId)
    {
        var result = await _userService.GetByIdAsync(userId);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var result = await _userService.GetByUsernameAsync(username);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpGet("email")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var result = await _userService.GetByEmailAsync(email);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpGet("phone")]
    public async Task<IActionResult> GetByPhoneNumber([FromQuery] string phoneNumber)
    {
        var result = await _userService.GetByPhoneNumberAsync(phoneNumber);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpGet("{userId:int}/details")]
    public async Task<IActionResult> GetUserDetails(int userId)
    {
        var result = await _userService.GetUserDetailsAsync(userId);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var result = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { userId = result.UserId },
            ApiResponse<UserDto>.SuccessResponse(result, "User created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
    {
        var result = await _userService.UpdateAsync(dto);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result, "User updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteUserDto dto)
    {
        var result = await _userService.DeleteAsync(dto);
        return result ? Ok(ApiResponse<bool>.SuccessResponse(true, "User deleted successfully."))
            : NotFound(ApiResponse<bool>.FailureResponse("User could not be deleted.", 404));
    }

    // 2. USER TYPE AND ACCOUNT STATUS

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveUsers()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetActiveUsersAsync()));

    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveUsers()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetInactiveUsersAsync()));

    [HttpGet("type/{userType}")]
    public async Task<IActionResult> GetByUserType(string userType)
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetByUserTypeAsync(userType)));

    [HttpGet("role/{roleId:int}")]
    public async Task<IActionResult> GetByRoleId(int roleId)
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetByRoleIdAsync(roleId)));

    [HttpPatch("{userId:int}/activate")]
    public async Task<IActionResult> Activate(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.ActivateAsync(userId)));

    [HttpPatch("{userId:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.DeactivateAsync(userId)));

    // 3. LOGIN SECURITY AND LOCKOUT

    [HttpGet("security/locked")]
    public async Task<IActionResult> GetCurrentlyLockedUsers([FromQuery] DateTime currentDateTime)
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
            await _userService.GetCurrentlyLockedUsersAsync(currentDateTime)));

    [HttpGet("security/expired-lockout")]
    public async Task<IActionResult> GetUsersWithExpiredLockout([FromQuery] DateTime currentDateTime)
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
            await _userService.GetUsersWithExpiredLockoutAsync(currentDateTime)));

    [HttpGet("security/failed-login-attempts")]
    public async Task<IActionResult> GetUsersWithFailedLoginAttempts([FromQuery] int minimumAttempts)
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
            await _userService.GetUsersWithFailedLoginAttemptsAsync(minimumAttempts)));

    [HttpGet("security/failed-login-attempts/below")]
    public async Task<IActionResult> GetUsersWithFailedLoginAttemptsBelow([FromQuery] int maximumAttempts)
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
            await _userService.GetUsersWithFailedLoginAttemptsBelowAsync(maximumAttempts)));

    [HttpPost("{userId:int}/security/failed-login")]
    public async Task<IActionResult> RecordFailedLoginAttempt(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.RecordFailedLoginAttemptAsync(userId)));

    [HttpPost("{userId:int}/security/reset-failed-login")]
    public async Task<IActionResult> ResetFailedLoginAttempts(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.ResetFailedLoginAttemptsAsync(userId)));

    [HttpPost("{userId:int}/security/lock")]
    public async Task<IActionResult> LockAccount(int userId, [FromQuery] DateTime lockoutEndAt)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.LockAccountAsync(userId, lockoutEndAt)));

    [HttpPost("{userId:int}/security/unlock")]
    public async Task<IActionResult> UnlockAccount(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.UnlockAccountAsync(userId)));

    // 4. EMAIL, PHONE AND MULTI-FACTOR VERIFICATION

    [HttpGet("email-verified")]
    public async Task<IActionResult> GetEmailVerifiedUsers()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetEmailVerifiedUsersAsync()));

    [HttpGet("email-unverified")]
    public async Task<IActionResult> GetEmailUnverifiedUsers()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetEmailUnverifiedUsersAsync()));

    [HttpGet("phone-verified")]
    public async Task<IActionResult> GetPhoneVerifiedUsers()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetPhoneVerifiedUsersAsync()));

    [HttpGet("phone-unverified")]
    public async Task<IActionResult> GetPhoneUnverifiedUsers()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetPhoneUnverifiedUsersAsync()));

    [HttpGet("two-factor/enabled")]
    public async Task<IActionResult> GetTwoFactorEnabledUsers()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetTwoFactorEnabledUsersAsync()));

    [HttpGet("two-factor/disabled")]
    public async Task<IActionResult> GetTwoFactorDisabledUsers()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetTwoFactorDisabledUsersAsync()));

    [HttpPost("{userId:int}/verify-email")]
    public async Task<IActionResult> VerifyEmail(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.VerifyEmailAsync(userId)));

    [HttpPost("{userId:int}/verify-phone")]
    public async Task<IActionResult> VerifyPhoneNumber(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.VerifyPhoneNumberAsync(userId)));

    [HttpPost("{userId:int}/two-factor/enable")]
    public async Task<IActionResult> EnableTwoFactorAuthentication(int userId, [FromBody] string encryptedSecret)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _userService.EnableTwoFactorAuthenticationAsync(userId, encryptedSecret)));

    [HttpPost("{userId:int}/two-factor/disable")]
    public async Task<IActionResult> DisableTwoFactorAuthentication(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.DisableTwoFactorAuthenticationAsync(userId)));

    // 5. CUSTOMER AND EMPLOYEE RELATIONSHIPS

    [HttpGet("with-customer-profiles")]
    public async Task<IActionResult> GetUsersWithCustomerProfiles()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetUsersWithCustomerProfilesAsync()));

    [HttpGet("with-employee-profiles")]
    public async Task<IActionResult> GetUsersWithEmployeeProfiles()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetUsersWithEmployeeProfilesAsync()));

    [HttpGet("{userId:int}/customer")]
    public async Task<IActionResult> GetUserWithCustomer(int userId)
    {
        var result = await _userService.GetUserWithCustomerAsync(userId);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpGet("{userId:int}/employee")]
    public async Task<IActionResult> GetUserWithEmployee(int userId)
    {
        var result = await _userService.GetUserWithEmployeeAsync(userId);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    // 6. PASSWORD AND LOGIN HISTORY

    [HttpPost("{userId:int}/change-password")]
    public async Task<IActionResult> ChangePassword(int userId, [FromQuery] string currentPassword, [FromQuery] string newPassword)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _userService.ChangePasswordAsync(userId, currentPassword, newPassword)));

    [HttpPost("{userId:int}/reset-password")]
    public async Task<IActionResult> ResetPassword(int userId, [FromBody] string newPassword)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.ResetPasswordAsync(userId, newPassword)));

    [HttpPost("{userId:int}/record-login")]
    public async Task<IActionResult> RecordSuccessfulLogin(int userId, [FromQuery] string? ipAddress)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.RecordSuccessfulLoginAsync(userId, ipAddress)));

    [HttpPatch("{userId:int}/password-changed-at")]
    public async Task<IActionResult> UpdatePasswordChangedAt(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.UpdatePasswordChangedAtAsync(userId)));

    [HttpGet("login-history")]
    public async Task<IActionResult> GetUsersByLastLoginRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
            await _userService.GetUsersByLastLoginRangeAsync(startDate, endDate)));

    [HttpGet("never-logged-in")]
    public async Task<IActionResult> GetUsersWhoNeverLoggedIn()
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.GetUsersWhoNeverLoggedInAsync()));

    [HttpGet("password-changed-range")]
    public async Task<IActionResult> GetUsersByPasswordChangedRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
            await _userService.GetUsersByPasswordChangedRangeAsync(startDate, endDate)));

    [HttpGet("created-range")]
    public async Task<IActionResult> GetUsersCreatedWithinRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(
            await _userService.GetUsersCreatedWithinRangeAsync(startDate, endDate)));

    // 7. RELATED RECORDS AND SECURITY INVESTIGATION

    [HttpGet("{userId:int}/refresh-tokens")]
    public async Task<IActionResult> GetUserWithRefreshTokens(int userId)
    {
        var result = await _userService.GetUserWithRefreshTokensAsync(userId);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpGet("{userId:int}/initiated-transactions")]
    public async Task<IActionResult> GetUserWithInitiatedTransactions(int userId)
    {
        var result = await _userService.GetUserWithInitiatedTransactionsAsync(userId);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpGet("{userId:int}/notifications")]
    public async Task<IActionResult> GetUserWithNotifications(int userId)
    {
        var result = await _userService.GetUserWithNotificationsAsync(userId);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    [HttpGet("{userId:int}/audit-logs")]
    public async Task<IActionResult> GetUserWithAuditLogs(int userId)
    {
        var result = await _userService.GetUserWithAuditLogsAsync(userId);
        return result == null ? NotFound(ApiResponse<UserDto>.FailureResponse("User not found.", 404))
            : Ok(ApiResponse<UserDto>.SuccessResponse(result));
    }

    // 8. SEARCH AND REPORTING

    [HttpGet("search")]
    public async Task<IActionResult> SearchUsers([FromQuery] string searchTerm)
        => Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(await _userService.SearchUsersAsync(searchTerm)));

    [HttpGet("count/active-by-role/{roleId:int}")]
    public async Task<IActionResult> CountActiveUsersByRole(int roleId)
        => Ok(ApiResponse<int>.SuccessResponse(await _userService.CountActiveUsersByRoleAsync(roleId)));

    [HttpGet("count/by-type-and-status")]
    public async Task<IActionResult> CountByUserTypeAndStatus([FromQuery] string userType, [FromQuery] bool isActive)
        => Ok(ApiResponse<int>.SuccessResponse(
            await _userService.CountByUserTypeAndStatusAsync(userType, isActive)));

    // 9. VALIDATION

    [HttpGet("exists/username")]
    public async Task<IActionResult> IsUsernameExists([FromQuery] string username)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.IsUsernameExistsAsync(username)));

    [HttpGet("exists/email")]
    public async Task<IActionResult> IsEmailExists([FromQuery] string email)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.IsEmailExistsAsync(email)));

    [HttpGet("exists/phone")]
    public async Task<IActionResult> IsPhoneNumberExists([FromQuery] string phoneNumber)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.IsPhoneNumberExistsAsync(phoneNumber)));

    [HttpGet("{userId:int}/can-delete")]
    public async Task<IActionResult> CanUserBeDeleted(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _userService.CanUserBeDeletedAsync(userId)));
}
