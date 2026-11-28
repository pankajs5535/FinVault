
using FinVault.Application.DTOs.AuthDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(
        IAuthService authService,
        ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(
            request,
            GetIpAddress(),
            Request.Headers["User-Agent"].ToString(),
            GetDeviceName(),
            cancellationToken);

        if (response is null)
            return Unauthorized(ApiResponse<AuthResponseDto>.FailureResponse(
                "Invalid credentials or account unavailable.",
                StatusCodes.Status401Unauthorized,
                traceId: HttpContext.TraceIdentifier));

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
            response,
            "Login successful.",
            StatusCodes.Status200OK));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await _authService.RefreshAsync(
            request,
            GetIpAddress(),
            Request.Headers["User-Agent"].ToString(),
            GetDeviceName(),
            cancellationToken);

        if (response is null)
            return Unauthorized(ApiResponse<AuthResponseDto>.FailureResponse(
                "Refresh token is invalid, expired or revoked.",
                StatusCodes.Status401Unauthorized,
                traceId: HttpContext.TraceIdentifier));

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
            response,
            "Token refreshed successfully.",
            StatusCodes.Status200OK));
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        var result = await _authService.LogoutAsync(
            request,
            _currentUserService.UserId.Value,
            GetIpAddress(),
            cancellationToken);

        if (!result)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Unable to revoke the supplied refresh token.",
                StatusCodes.Status400BadRequest,
                traceId: HttpContext.TraceIdentifier));

        return Ok(ApiResponse<object>.SuccessResponse(
            null,
            "Logout successful.",
            StatusCodes.Status200OK));
    }

    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.UserId.HasValue)
            return Unauthorized();

        var result = await _authService.ChangePasswordAsync(
            request,
            _currentUserService.UserId.Value,
            cancellationToken);

        if (!result)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Password change failed. Verify your current password and new password.",
                StatusCodes.Status400BadRequest,
                traceId: HttpContext.TraceIdentifier));

        return Ok(ApiResponse<object>.SuccessResponse(
            null,
            "Password changed successfully. Existing refresh tokens have been revoked.",
            StatusCodes.Status200OK));
    }

    private string GetIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private string? GetDeviceName()
    {
        var deviceName = Request.Headers["X-Device-Name"].ToString();
        return string.IsNullOrWhiteSpace(deviceName) ? null : deviceName.Trim();
    }
}
