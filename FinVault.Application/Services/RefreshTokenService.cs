
using System.Net;
using AutoMapper;
using FinVault.Application.DTOs.RefreshTokenDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RefreshTokenService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. TOKEN IDENTIFICATION

    public async Task<IEnumerable<RefreshTokenDto>> GetAllAsync()
    {
        var tokens = await _unitOfWork.RefreshToken.GetAllAsync();
        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<RefreshTokenDto?> GetByIdAsync(int refreshTokenId)
    {
        if (refreshTokenId <= 0)
            return null;

        var token = await _unitOfWork.RefreshToken.GetByIdAsync(refreshTokenId);
        return token == null ? null : _mapper.Map<RefreshTokenDto>(token);
    }

    public async Task<RefreshTokenDto?> GetByTokenHashAsync(string tokenHash)
    {
        ValidateTokenHash(tokenHash);

        var token = await _unitOfWork.RefreshToken.GetByTokenHashAsync(tokenHash);
        return token == null ? null : _mapper.Map<RefreshTokenDto>(token);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetByJwtIdAsync(string jwtId)
    {
        ValidateRequiredText(jwtId, nameof(jwtId), 64);

        var tokens = await _unitOfWork.RefreshToken.GetByJwtIdAsync(jwtId.Trim());
        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public Task<bool> ExistsByTokenHashAsync(string tokenHash)
    {
        ValidateTokenHash(tokenHash);
        return _unitOfWork.RefreshToken.ExistsByTokenHashAsync(tokenHash);
    }

    // 2. TOKEN CREATION AND ROTATION

    public async Task<RefreshTokenDto> CreateAsync(
        CreateRefreshTokenDto dto,
        string tokenHash)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateTokenHash(tokenHash);
        ValidateCreateDto(dto);

        if (await _unitOfWork.RefreshToken.ExistsByTokenHashAsync(tokenHash))
        {
            throw new InvalidOperationException(
                "A refresh token with this hash already exists.");
        }

        if (await _unitOfWork.User.GetByIdAsync(dto.UserId) == null)
            throw new InvalidOperationException("The specified user does not exist.");

        var token = BuildToken(dto, tokenHash);

        await _unitOfWork.RefreshToken.AddAsync(token);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RefreshTokenDto>(token);
    }

    public async Task<RefreshTokenDto?> RotateAsync(
        int currentRefreshTokenId,
        CreateRefreshTokenDto replacementDto,
        string replacementTokenHash)
    {
        ArgumentNullException.ThrowIfNull(replacementDto);

        if (currentRefreshTokenId <= 0)
            throw new ArgumentOutOfRangeException(nameof(currentRefreshTokenId));

        ValidateTokenHash(replacementTokenHash);
        ValidateCreateDto(replacementDto);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var currentToken = await _unitOfWork.RefreshToken
                .GetByIdAsync(currentRefreshTokenId);

            if (currentToken == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return null;
            }

            // Reuse of a token that has already been replaced.
            if (currentToken.ReplacedByTokenId.HasValue)
            {
                await RevokeTokenFamilyAsync(
                    currentToken,
                    "Refresh token reuse detected.",
                    replacementDto.CreatedByIp);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                throw new InvalidOperationException(
                    "Refresh token reuse detected. The affected token family has been revoked.");
            }

            if (currentToken.RevokedAt.HasValue ||
                currentToken.ExpiresAt <= DateTime.UtcNow)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return null;
            }

            if (currentToken.UserId != replacementDto.UserId)
            {
                throw new InvalidOperationException(
                    "The replacement token must belong to the same user.");
            }

            if (await _unitOfWork.User.GetByIdAsync(replacementDto.UserId) == null)
                throw new InvalidOperationException("The specified user does not exist.");

            if (await _unitOfWork.RefreshToken
                .ExistsByTokenHashAsync(replacementTokenHash))
            {
                throw new InvalidOperationException(
                    "The replacement token hash already exists.");
            }

            var replacementToken = BuildToken(
                replacementDto,
                replacementTokenHash);

            await _unitOfWork.RefreshToken.AddAsync(replacementToken);
            await _unitOfWork.SaveChangesAsync();

            currentToken.RevokedAt = DateTime.UtcNow;
            currentToken.RevocationReason = "Token rotated.";
            currentToken.RevokedByIp = replacementDto.CreatedByIp.Trim();
            currentToken.ReplacedByTokenId = replacementToken.RefreshTokenId;

            _unitOfWork.RefreshToken.Update(currentToken);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<RefreshTokenDto>(replacementToken);
        }
        catch (InvalidOperationException exception)
            when (exception.Message.StartsWith(
                "Refresh token reuse detected.",
                StringComparison.Ordinal))
        {
            throw;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<RefreshTokenDto?> GetReplacementTokenAsync(int refreshTokenId)
    {
        if (refreshTokenId <= 0)
            return null;

        var token = await _unitOfWork.RefreshToken
            .GetReplacementTokenAsync(refreshTokenId);

        return token == null ? null : _mapper.Map<RefreshTokenDto>(token);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetReplacedTokensAsync(
        int refreshTokenId)
    {
        if (refreshTokenId <= 0)
            return Enumerable.Empty<RefreshTokenDto>();

        var tokens = await _unitOfWork.RefreshToken
            .GetReplacedTokensAsync(refreshTokenId);

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetTokensWithReplacementAsync()
    {
        var tokens = await _unitOfWork.RefreshToken.GetTokensWithReplacementAsync();
        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetTokensWithoutReplacementAsync()
    {
        var tokens = await _unitOfWork.RefreshToken.GetTokensWithoutReplacementAsync();
        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    // 3. USER SESSION MANAGEMENT

    public async Task<IEnumerable<RefreshTokenDto>> GetByUserIdAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<RefreshTokenDto>();

        var tokens = await _unitOfWork.RefreshToken.GetByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>>
        GetUserTokensWithReplacementHistoryAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<RefreshTokenDto>();

        var tokens = await _unitOfWork.RefreshToken
            .GetUserTokensWithReplacementHistoryAsync(userId);

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetActiveTokensByUserIdAsync(
        int userId,
        DateTime currentDateTime)
    {
        if (userId <= 0)
            return Enumerable.Empty<RefreshTokenDto>();

        ValidateDate(currentDateTime, nameof(currentDateTime));

        var tokens = await _unitOfWork.RefreshToken
            .GetActiveTokensByUserIdAsync(userId, currentDateTime);

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public Task<int> CountActiveTokensByUserIdAsync(
        int userId,
        DateTime currentDateTime)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId));

        ValidateDate(currentDateTime, nameof(currentDateTime));

        return _unitOfWork.RefreshToken
            .CountActiveTokensByUserIdAsync(userId, currentDateTime);
    }

    public async Task<bool> RevokeAsync(
        int refreshTokenId,
        string revocationReason,
        string revokedByIp)
    {
        if (refreshTokenId <= 0)
            return false;

        ValidateRequiredText(revocationReason, nameof(revocationReason), 250);
        ValidateIpAddress(revokedByIp, nameof(revokedByIp));

        var token = await _unitOfWork.RefreshToken.GetByIdAsync(refreshTokenId);

        if (token == null || token.RevokedAt.HasValue)
            return false;

        token.RevokedAt = DateTime.UtcNow;
        token.RevocationReason = revocationReason.Trim();
        token.RevokedByIp = revokedByIp.Trim();

        _unitOfWork.RefreshToken.Update(token);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RevokeAllByUserIdAsync(
        int userId,
        string revocationReason,
        string revokedByIp)
    {
        if (userId <= 0)
            return false;

        ValidateRequiredText(revocationReason, nameof(revocationReason), 250);
        ValidateIpAddress(revokedByIp, nameof(revokedByIp));

        var tokens = await _unitOfWork.RefreshToken.GetByUserIdAsync(userId);
        var now = DateTime.UtcNow;
        var changed = false;

        foreach (var token in tokens.Where(x => !x.RevokedAt.HasValue))
        {
            token.RevokedAt = now;
            token.RevocationReason = revocationReason.Trim();
            token.RevokedByIp = revokedByIp.Trim();

            _unitOfWork.RefreshToken.Update(token);
            changed = true;
        }

        if (changed)
            await _unitOfWork.SaveChangesAsync();

        return changed;
    }

    // 4. TOKEN EXPIRY

    public async Task<IEnumerable<RefreshTokenDto>> GetExpiredTokensAsync(
        DateTime currentDateTime)
    {
        ValidateDate(currentDateTime, nameof(currentDateTime));

        var tokens = await _unitOfWork.RefreshToken
            .GetExpiredTokensAsync(currentDateTime);

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetUnexpiredTokensAsync(
        DateTime currentDateTime)
    {
        ValidateDate(currentDateTime, nameof(currentDateTime));

        var tokens = await _unitOfWork.RefreshToken
            .GetUnexpiredTokensAsync(currentDateTime);

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetTokensExpiringWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var tokens = await _unitOfWork.RefreshToken
            .GetTokensExpiringWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<bool> IsTokenExpiredAsync(
        int refreshTokenId,
        DateTime currentDateTime)
    {
        if (refreshTokenId <= 0)
            return false;

        ValidateDate(currentDateTime, nameof(currentDateTime));

        var token = await _unitOfWork.RefreshToken.GetByIdAsync(refreshTokenId);

        return token != null && token.ExpiresAt <= currentDateTime;
    }

    // 5. REVOCATION HISTORY

    public async Task<IEnumerable<RefreshTokenDto>> GetRevokedTokensAsync()
    {
        var tokens = await _unitOfWork.RefreshToken.GetRevokedTokensAsync();
        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetNonRevokedTokensAsync()
    {
        var tokens = await _unitOfWork.RefreshToken.GetNonRevokedTokensAsync();
        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetRevokedTokensByUserIdAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<RefreshTokenDto>();

        var tokens = await _unitOfWork.RefreshToken
            .GetRevokedTokensByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetRevokedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var tokens = await _unitOfWork.RefreshToken
            .GetRevokedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    // 6. CLIENT AND DEVICE INFORMATION

    public async Task<IEnumerable<RefreshTokenDto>> GetByCreatedByIpAsync(
        string ipAddress)
    {
        ValidateIpAddress(ipAddress, nameof(ipAddress));

        var tokens = await _unitOfWork.RefreshToken
            .GetByCreatedByIpAsync(ipAddress.Trim());

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetByRevokedByIpAsync(
        string ipAddress)
    {
        ValidateIpAddress(ipAddress, nameof(ipAddress));

        var tokens = await _unitOfWork.RefreshToken
            .GetByRevokedByIpAsync(ipAddress.Trim());

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetByDeviceNameAsync(
        string deviceName)
    {
        ValidateRequiredText(deviceName, nameof(deviceName), 100);

        var tokens = await _unitOfWork.RefreshToken
            .GetByDeviceNameAsync(deviceName.Trim());

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    public async Task<IEnumerable<RefreshTokenDto>> GetCreatedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var tokens = await _unitOfWork.RefreshToken
            .GetCreatedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<RefreshTokenDto>>(tokens);
    }

    // 7. VALIDATION

    public async Task<bool> CanTokenBeRefreshedAsync(
        string tokenHash,
        DateTime currentDateTime)
    {
        ValidateTokenHash(tokenHash);
        ValidateDate(currentDateTime, nameof(currentDateTime));

        var token = await _unitOfWork.RefreshToken
            .GetByTokenHashAsync(tokenHash);

        return token != null &&
               !token.RevokedAt.HasValue &&
               !token.ReplacedByTokenId.HasValue &&
               token.ExpiresAt > currentDateTime;
    }

    public async Task<bool> CanTokenBeRevokedAsync(int refreshTokenId)
    {
        if (refreshTokenId <= 0)
            return false;

        var token = await _unitOfWork.RefreshToken.GetByIdAsync(refreshTokenId);

        return token != null && !token.RevokedAt.HasValue;
    }

    // PRIVATE METHODS

    private RefreshToken BuildToken(CreateRefreshTokenDto dto, string tokenHash)
    {
        var token = _mapper.Map<RefreshToken>(dto);

        token.TokenHash = tokenHash;
        token.JwtId = dto.JwtId.Trim();
        token.CreatedByIp = dto.CreatedByIp.Trim();
        token.UserAgent = NormalizeOptionalText(dto.UserAgent);
        token.DeviceName = NormalizeOptionalText(dto.DeviceName);
        token.CreatedAt = DateTime.UtcNow;
        token.RevokedAt = null;
        token.RevocationReason = null;
        token.RevokedByIp = null;
        token.ReplacedByTokenId = null;

        return token;
    }

    private async Task RevokeTokenFamilyAsync(
        RefreshToken startingToken,
        string reason,
        string revokedByIp)
    {
        var userTokens = (await _unitOfWork.RefreshToken
            .GetByUserIdAsync(startingToken.UserId)).ToList();

        var familyIds = new HashSet<int>
        {
            startingToken.RefreshTokenId
        };

        // Follow both predecessor and replacement links to find this session's family.
        bool changed;

        do
        {
            changed = false;

            foreach (var token in userTokens)
            {
                if (familyIds.Contains(token.RefreshTokenId) ||
                    (token.ReplacedByTokenId.HasValue &&
                     familyIds.Contains(token.ReplacedByTokenId.Value)))
                {
                    if (familyIds.Add(token.RefreshTokenId))
                        changed = true;

                    if (token.ReplacedByTokenId.HasValue &&
                        familyIds.Add(token.ReplacedByTokenId.Value))
                    {
                        changed = true;
                    }
                }
            }
        }
        while (changed);

        var now = DateTime.UtcNow;

        foreach (var token in userTokens.Where(x => familyIds.Contains(x.RefreshTokenId)))
        {
            if (token.RevokedAt.HasValue)
                continue;

            token.RevokedAt = now;
            token.RevocationReason = reason;
            token.RevokedByIp = revokedByIp.Trim();

            _unitOfWork.RefreshToken.Update(token);
        }
    }

    private static void ValidateCreateDto(CreateRefreshTokenDto dto)
    {
        ValidateRequiredText(dto.JwtId, nameof(dto.JwtId), 64);
        ValidateIpAddress(dto.CreatedByIp, nameof(dto.CreatedByIp));
        ValidateOptionalText(dto.UserAgent, nameof(dto.UserAgent), 512);
        ValidateOptionalText(dto.DeviceName, nameof(dto.DeviceName), 100);

        if (dto.UserId <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.UserId));

        if (dto.ExpiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Refresh token expiry must be in the future.");
    }

    private static void ValidateTokenHash(string? tokenHash)
    {
        if (string.IsNullOrWhiteSpace(tokenHash) || tokenHash.Length != 64)
            throw new ArgumentException(
                "A SHA-256 token hash in 64-character hexadecimal format is required.",
                nameof(tokenHash));

        try
        {
            _ = Convert.FromHexString(tokenHash);
        }
        catch (FormatException)
        {
            throw new ArgumentException(
                "Token hash must contain only hexadecimal characters.",
                nameof(tokenHash));
        }
    }

    private static void ValidateIpAddress(string? ipAddress, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(ipAddress) ||
            ipAddress.Trim().Length > 45 ||
            !IPAddress.TryParse(ipAddress.Trim(), out _))
        {
            throw new ArgumentException("A valid IP address is required.", parameterName);
        }
    }

    private static void ValidateRequiredText(
        string? value,
        string parameterName,
        int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", parameterName);

        if (value.Trim().Length > maximumLength)
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength} characters.",
                parameterName);
    }

    private static void ValidateOptionalText(
        string? value,
        string parameterName,
        int maximumLength)
    {
        if (value?.Trim().Length > maximumLength)
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength} characters.",
                parameterName);
    }

    private static void ValidateDate(DateTime date, string parameterName)
    {
        if (date == default)
            throw new ArgumentException("A valid date and time is required.", parameterName);
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        ValidateDate(startDate, nameof(startDate));
        ValidateDate(endDate, nameof(endDate));

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
