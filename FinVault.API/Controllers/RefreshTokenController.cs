
using FinVault.Application.DTOs.RefreshTokenDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SystemAdministrator")]
public class RefreshTokenController : ControllerBase
{
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshTokenController(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    // 1. TOKEN IDENTIFICATION

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetAllAsync()));

    [HttpGet("{refreshTokenId:int}")]
    public async Task<IActionResult> GetById(int refreshTokenId)
    {
        var result = await _refreshTokenService.GetByIdAsync(refreshTokenId);
        return result == null
            ? NotFound(ApiResponse<RefreshTokenDto>.FailureResponse("Refresh token not found.", 404))
            : Ok(ApiResponse<RefreshTokenDto>.SuccessResponse(result));
    }

    [HttpGet("hash/{tokenHash}")]
    public async Task<IActionResult> GetByTokenHash(string tokenHash)
    {
        try
        {
            var result = await _refreshTokenService.GetByTokenHashAsync(tokenHash);
            return result == null
                ? NotFound(ApiResponse<RefreshTokenDto>.FailureResponse("Refresh token not found.", 404))
                : Ok(ApiResponse<RefreshTokenDto>.SuccessResponse(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<RefreshTokenDto>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("jwt/{jwtId}")]
    public async Task<IActionResult> GetByJwtId(string jwtId)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetByJwtIdAsync(jwtId)));

    [HttpGet("exists/hash")]
    public async Task<IActionResult> ExistsByTokenHash([FromQuery] string tokenHash)
    {
        try
        {
            return Ok(ApiResponse<bool>.SuccessResponse(
                await _refreshTokenService.ExistsByTokenHashAsync(tokenHash)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<bool>.FailureResponse(ex.Message, 400));
        }
    }

    // 2. TOKEN CREATION AND ROTATION

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRefreshTokenDto dto,
        [FromQuery] string tokenHash)
    {
        try
        {
            var result = await _refreshTokenService.CreateAsync(dto, tokenHash);

            return CreatedAtAction(
                nameof(GetById),
                new { refreshTokenId = result.RefreshTokenId },
                ApiResponse<RefreshTokenDto>.SuccessResponse(
                    result, "Refresh token created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<RefreshTokenDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<RefreshTokenDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPost("{currentRefreshTokenId:int}/rotate")]
    public async Task<IActionResult> Rotate(
        int currentRefreshTokenId,
        [FromBody] CreateRefreshTokenDto replacementDto,
        [FromQuery] string replacementTokenHash)
    {
        try
        {
            var result = await _refreshTokenService.RotateAsync(
                currentRefreshTokenId, replacementDto, replacementTokenHash);

            return result == null
                ? NotFound(ApiResponse<RefreshTokenDto>.FailureResponse(
                    "Current refresh token not found or cannot be rotated.", 404))
                : Ok(ApiResponse<RefreshTokenDto>.SuccessResponse(
                    result, "Refresh token rotated successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<RefreshTokenDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<RefreshTokenDto>.FailureResponse(ex.Message, 409));
        }
    }

    // 3. TOKEN REPLACEMENT HISTORY

    [HttpGet("{refreshTokenId:int}/replacement")]
    public async Task<IActionResult> GetReplacementToken(int refreshTokenId)
    {
        var result = await _refreshTokenService.GetReplacementTokenAsync(refreshTokenId);
        return result == null
            ? NotFound(ApiResponse<RefreshTokenDto>.FailureResponse("Replacement token not found.", 404))
            : Ok(ApiResponse<RefreshTokenDto>.SuccessResponse(result));
    }

    [HttpGet("{refreshTokenId:int}/replaced-tokens")]
    public async Task<IActionResult> GetReplacedTokens(int refreshTokenId)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetReplacedTokensAsync(refreshTokenId)));

    [HttpGet("with-replacement")]
    public async Task<IActionResult> GetTokensWithReplacement()
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetTokensWithReplacementAsync()));

    [HttpGet("without-replacement")]
    public async Task<IActionResult> GetTokensWithoutReplacement()
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetTokensWithoutReplacementAsync()));

    // 4. USER TOKEN HISTORY

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetByUserIdAsync(userId)));

    [HttpGet("user/{userId:int}/replacement-history")]
    public async Task<IActionResult> GetUserTokensWithReplacementHistory(int userId)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetUserTokensWithReplacementHistoryAsync(userId)));

    [HttpGet("user/{userId:int}/active")]
    public async Task<IActionResult> GetActiveTokensByUserId(
        int userId,
        [FromQuery] DateTime currentDateTime)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetActiveTokensByUserIdAsync(userId, currentDateTime)));

    [HttpGet("user/{userId:int}/active/count")]
    public async Task<IActionResult> CountActiveTokensByUserId(
        int userId,
        [FromQuery] DateTime currentDateTime)
        => Ok(ApiResponse<int>.SuccessResponse(
            await _refreshTokenService.CountActiveTokensByUserIdAsync(userId, currentDateTime)));

    // 5. TOKEN REVOCATION

    [HttpPost("{refreshTokenId:int}/revoke")]
    public async Task<IActionResult> Revoke(
        int refreshTokenId,
        [FromQuery] string revocationReason,
        [FromQuery] string revokedByIp)
    {
        try
        {
            var result = await _refreshTokenService.RevokeAsync(
                refreshTokenId, revocationReason, revokedByIp);

            return result
                ? Ok(ApiResponse<bool>.SuccessResponse(true, "Refresh token revoked successfully."))
                : Conflict(ApiResponse<bool>.FailureResponse(
                    "Refresh token could not be revoked.", 409));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<bool>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpPost("user/{userId:int}/revoke-all")]
    public async Task<IActionResult> RevokeAllByUserId(
        int userId,
        [FromQuery] string revocationReason,
        [FromQuery] string revokedByIp)
    {
        try
        {
            var result = await _refreshTokenService.RevokeAllByUserIdAsync(
                userId, revocationReason, revokedByIp);

            return Ok(ApiResponse<bool>.SuccessResponse(
                result, "User refresh tokens revocation processed."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<bool>.FailureResponse(ex.Message, 400));
        }
    }

    // 6. EXPIRY MANAGEMENT

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredTokens([FromQuery] DateTime currentDateTime)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetExpiredTokensAsync(currentDateTime)));

    [HttpGet("unexpired")]
    public async Task<IActionResult> GetUnexpiredTokens([FromQuery] DateTime currentDateTime)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetUnexpiredTokensAsync(currentDateTime)));

    [HttpGet("expiring/date-range")]
    public async Task<IActionResult> GetTokensExpiringWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetTokensExpiringWithinRangeAsync(startDate, endDate)));

    [HttpGet("{refreshTokenId:int}/expired")]
    public async Task<IActionResult> IsTokenExpired(
        int refreshTokenId,
        [FromQuery] DateTime currentDateTime)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _refreshTokenService.IsTokenExpiredAsync(refreshTokenId, currentDateTime)));

    // 7. REVOCATION HISTORY

    [HttpGet("revoked")]
    public async Task<IActionResult> GetRevokedTokens()
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetRevokedTokensAsync()));

    [HttpGet("non-revoked")]
    public async Task<IActionResult> GetNonRevokedTokens()
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetNonRevokedTokensAsync()));

    [HttpGet("user/{userId:int}/revoked")]
    public async Task<IActionResult> GetRevokedTokensByUserId(int userId)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetRevokedTokensByUserIdAsync(userId)));

    [HttpGet("revoked/date-range")]
    public async Task<IActionResult> GetRevokedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetRevokedWithinRangeAsync(startDate, endDate)));

    // 8. CLIENT AND DEVICE INFORMATION

    [HttpGet("created-ip/{ipAddress}")]
    public async Task<IActionResult> GetByCreatedByIp(string ipAddress)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetByCreatedByIpAsync(ipAddress)));

    [HttpGet("revoked-ip/{ipAddress}")]
    public async Task<IActionResult> GetByRevokedByIp(string ipAddress)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetByRevokedByIpAsync(ipAddress)));

    [HttpGet("device/{deviceName}")]
    public async Task<IActionResult> GetByDeviceName(string deviceName)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetByDeviceNameAsync(deviceName)));

    [HttpGet("created/date-range")]
    public async Task<IActionResult> GetCreatedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
        => Ok(ApiResponse<IEnumerable<RefreshTokenDto>>.SuccessResponse(
            await _refreshTokenService.GetCreatedWithinRangeAsync(startDate, endDate)));

    // 9. TOKEN VALIDATION

    [HttpGet("can-refresh")]
    public async Task<IActionResult> CanTokenBeRefreshed(
        [FromQuery] string tokenHash,
        [FromQuery] DateTime currentDateTime)
    {
        try
        {
            return Ok(ApiResponse<bool>.SuccessResponse(
                await _refreshTokenService.CanTokenBeRefreshedAsync(tokenHash, currentDateTime)));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<bool>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("{refreshTokenId:int}/can-revoke")]
    public async Task<IActionResult> CanTokenBeRevoked(int refreshTokenId)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _refreshTokenService.CanTokenBeRevokedAsync(refreshTokenId)));
}
