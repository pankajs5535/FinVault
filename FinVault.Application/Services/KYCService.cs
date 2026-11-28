using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FinVault.Application.DTOs.KYCDetailsDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace FinVault.Application.Services;

public sealed class KYCService : IKYCService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;
    private readonly IDataProtector _panProtector;
    private readonly IConfiguration _configuration;

    public KYCService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUser,
        IDataProtectionProvider dataProtectionProvider,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _panProtector = (dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider)))
            .CreateProtector("FinVault.KYC.PAN.v1");
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    // =========================================================
    // 1. KYC IDENTIFICATION AND CRUD
    // =========================================================

    public async Task<IEnumerable<KYCDetailsDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.KYCDetails.GetAllAsync();
        return MapMany(entities);
    }

    public async Task<KYCDetailsDto?> GetByIdAsync(int kycDetailsId)
    {
        if (kycDetailsId <= 0)
            return null;

        var entity = await _unitOfWork.KYCDetails.GetByIdAsync(kycDetailsId);
        return entity is null ? null : Map(entity);
    }

    public async Task<KYCDetailsDto?> GetKYCDetailsAsync(int kycDetailsId)
    {
        if (kycDetailsId <= 0)
            return null;

        var entity = await _unitOfWork.KYCDetails.GetKYCDetailsAsync(kycDetailsId);
        return entity is null ? null : Map(entity);
    }

    public async Task<KYCDetailsDto?> GetCurrentKYCByCustomerIdAsync(int customerId)
    {
        if (customerId <= 0)
            return null;

        var entity = await _unitOfWork.KYCDetails
            .GetCurrentKYCByCustomerIdAsync(customerId);

        return entity is null ? null : Map(entity);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetKYCHistoryByCustomerIdAsync(int customerId)
    {
        if (customerId <= 0)
            return Enumerable.Empty<KYCDetailsDto>();

        var entities = await _unitOfWork.KYCDetails
            .GetKYCHistoryByCustomerIdAsync(customerId);

        return MapMany(entities);
    }

    public async Task<KYCDetailsDto> CreateAsync(CreateKYCDetailsDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateCreate(dto);

        var customer = await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId);

        if (customer is null)
            throw new InvalidOperationException(
                $"Customer with ID {dto.CustomerId} does not exist.");

        var entity = _mapper.Map<KYCDetails>(dto);

        // Verification must be performed through the authorized verification workflow.
        entity.VerificationStatus = "Pending";
        entity.VerifiedAt = null;
        entity.VerifiedByEmployeeId = null;
        entity.RejectionReason = null;

        // Do not accept verification claims directly from the create request.
        entity.IsPanVerified = false;
        entity.PanVerifiedAt = null;
        entity.IsAadhaarVerified = false;
        entity.AadhaarVerifiedAt = null;

        entity.CreatedAt = DateTime.UtcNow;
        entity.CreatedBy = _currentUser.UserId;
        entity.UpdatedAt = null;
        entity.UpdatedBy = null;

        ApplyPan(entity, dto.Pan);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (entity.IsCurrent)
                await RetireCurrentRecordAsync(entity.CustomerId, null);

            await _unitOfWork.KYCDetails.AddAsync(entity);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return Map(entity);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<KYCDetailsDto?> UpdateAsync(UpdateKYCDetailsDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.KYCDetailsId <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.KYCDetailsId));

        ValidateStatusValues(
            dto.KycLevel,
            dto.VerificationStatus,
            dto.CkycStatus,
            dto.AadhaarVerificationMethod);

        var entity = await _unitOfWork.KYCDetails
            .GetByIdAsync(dto.KYCDetailsId);

        if (entity is null)
            return null;

        var customer = await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId);

        if (customer is null)
            throw new InvalidOperationException(
                $"Customer with ID {dto.CustomerId} does not exist.");

        if (dto.VerificationStatus == "Verified")
            throw new InvalidOperationException(
                "Use VerifyKYCAsync to verify a KYC record.");

        // Preserve protected and verification-controlled values.
        var existingPanEncrypted = entity.PanEncrypted;
        var existingPanHash = entity.PanHash;
        var existingPanMasked = entity.PanMasked;
        var existingPanVerified = entity.IsPanVerified;
        var existingPanVerifiedAt = entity.PanVerifiedAt;

        var existingAadhaarVerified = entity.IsAadhaarVerified;
        var existingAadhaarVerifiedAt = entity.AadhaarVerifiedAt;
        var existingAadhaarVaultReference = entity.AadhaarVaultReference;

        var existingVerifiedAt = entity.VerifiedAt;
        var existingVerifiedByEmployeeId = entity.VerifiedByEmployeeId;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (dto.IsCurrent)
                await RetireCurrentRecordAsync(
                    dto.CustomerId,
                    entity.KYCDetailsId);

            _mapper.Map(dto, entity);

            entity.PanEncrypted = existingPanEncrypted;
            entity.PanHash = existingPanHash;
            entity.PanMasked = existingPanMasked;
            entity.IsPanVerified = existingPanVerified;
            entity.PanVerifiedAt = existingPanVerifiedAt;

            entity.IsAadhaarVerified = existingAadhaarVerified;
            entity.AadhaarVerifiedAt = existingAadhaarVerifiedAt;
            entity.AadhaarVaultReference = existingAadhaarVaultReference;

            entity.VerifiedAt = existingVerifiedAt;
            entity.VerifiedByEmployeeId = existingVerifiedByEmployeeId;

            if (!string.IsNullOrWhiteSpace(dto.Pan))
                ApplyPan(entity, dto.Pan);

            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.UserId;

            _unitOfWork.KYCDetails.Update(entity);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return Map(entity);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public Task<bool> DeleteAsync(DeleteKYCDetailsDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        // KYC history is a compliance record.
        // Physical deletion is intentionally not supported.
        return Task.FromResult(false);
    }

    public Task<bool> HasCurrentKYCRecordAsync(int customerId)
    {
        if (customerId <= 0)
            return Task.FromResult(false);

        return _unitOfWork.KYCDetails
            .HasCurrentKYCRecordAsync(customerId);
    }

    // =========================================================
    // 2. KYC LEVEL AND VERIFICATION STATUS
    // =========================================================

    public async Task<IEnumerable<KYCDetailsDto>> GetByKycLevelAsync(string kycLevel)
    {
        RequireValue(kycLevel, nameof(kycLevel));

        var entities = await _unitOfWork.KYCDetails
            .GetByKycLevelAsync(kycLevel);

        return MapMany(entities);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetByVerificationStatusAsync(
        string verificationStatus)
    {
        RequireValue(verificationStatus, nameof(verificationStatus));

        var entities = await _unitOfWork.KYCDetails
            .GetByVerificationStatusAsync(verificationStatus);

        return MapMany(entities);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetPendingKYCRecordsAsync()
    {
        var entities = await _unitOfWork.KYCDetails
            .GetPendingKYCRecordsAsync();

        return MapMany(entities);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetInProgressKYCRecordsAsync()
    {
        var entities = await _unitOfWork.KYCDetails
            .GetInProgressKYCRecordsAsync();

        return MapMany(entities);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetVerifiedKYCRecordsAsync()
    {
        var entities = await _unitOfWork.KYCDetails
            .GetVerifiedKYCRecordsAsync();

        return MapMany(entities);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetRejectedKYCRecordsAsync()
    {
        var entities = await _unitOfWork.KYCDetails
            .GetRejectedKYCRecordsAsync();

        return MapMany(entities);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetExpiredKYCRecordsAsync()
    {
        var entities = await _unitOfWork.KYCDetails
            .GetExpiredKYCRecordsAsync();

        return MapMany(entities);
    }

    // =========================================================
    // 3. PAN VERIFICATION AND DUPLICATE DETECTION
    // =========================================================

    public async Task<KYCDetailsDto?> GetCurrentByPanAsync(string pan)
    {
        var panHash = CreatePanHash(pan);

        var entity = await _unitOfWork.KYCDetails
            .GetCurrentByPanHashAsync(panHash);

        return entity is null ? null : Map(entity);
    }

    public Task<bool> ExistsCurrentByPanAsync(string pan)
    {
        var panHash = CreatePanHash(pan);

        return _unitOfWork.KYCDetails
            .ExistsCurrentByPanHashAsync(panHash);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetPanVerifiedRecordsAsync()
    {
        var entities = await _unitOfWork.KYCDetails
            .GetPanVerifiedRecordsAsync();

        return MapMany(entities);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetPanPendingVerificationRecordsAsync()
    {
        var entities = await _unitOfWork.KYCDetails
            .GetPanPendingVerificationRecordsAsync();

        return MapMany(entities);
    }

    public Task<bool> VerifyPanAsync(int kycDetailsId)
    {
        if (kycDetailsId <= 0)
            throw new ArgumentOutOfRangeException(nameof(kycDetailsId));

        // PAN verification requires an approved external PAN verification provider.
        // The current project interfaces do not define that provider.
        throw new NotSupportedException(
            "Integrate an approved PAN verification provider before enabling PAN verification.");
    }

    // =========================================================
    // 4. AADHAAR AND CKYC MANAGEMENT
    // =========================================================

    public async Task<IEnumerable<KYCDetailsDto>> GetAadhaarVerifiedRecordsAsync()
    {
        var entities = await _unitOfWork.KYCDetails
            .GetAadhaarVerifiedRecordsAsync();

        return MapMany(entities);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetByAadhaarVerificationMethodAsync(
        string verificationMethod)
    {
        RequireValue(verificationMethod, nameof(verificationMethod));

        var entities = await _unitOfWork.KYCDetails
            .GetByAadhaarVerificationMethodAsync(verificationMethod);

        return MapMany(entities);
    }

    public async Task<KYCDetailsDto?> GetByCkycNumberAsync(string ckycNumber)
    {
        RequireValue(ckycNumber, nameof(ckycNumber));

        var entity = await _unitOfWork.KYCDetails
            .GetByCkycNumberAsync(ckycNumber.Trim());

        return entity is null ? null : Map(entity);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetByCkycStatusAsync(string ckycStatus)
    {
        RequireValue(ckycStatus, nameof(ckycStatus));

        var entities = await _unitOfWork.KYCDetails
            .GetByCkycStatusAsync(ckycStatus);

        return MapMany(entities);
    }

    public async Task<bool> UpdateCkycStatusAsync(
        int kycDetailsId,
        string ckycStatus)
    {
        if (kycDetailsId <= 0)
            return false;

        var allowedStatuses = new[]
        {
            "NotInitiated",
            "Pending",
            "Uploaded",
            "Verified",
            "Rejected"
        };

        if (!allowedStatuses.Contains(ckycStatus))
            throw new ArgumentException("Invalid CKYC status.", nameof(ckycStatus));

        if (ckycStatus == "Verified")
            EnsureVerifier();

        var entity = await _unitOfWork.KYCDetails
            .GetByIdAsync(kycDetailsId);

        if (entity is null)
            return false;

        entity.CkycStatus = ckycStatus;
        entity.CkycVerifiedAt = ckycStatus == "Verified"
            ? DateTime.UtcNow
            : null;

        Touch(entity);

        _unitOfWork.KYCDetails.Update(entity);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // =========================================================
    // 5. RE-KYC AND VERIFICATION DEADLINES
    // =========================================================

    public async Task<IEnumerable<KYCDetailsDto>> GetOverdueReKYCRecordsAsync(
        DateTime currentDate)
    {
        var entities = await _unitOfWork.KYCDetails
            .GetOverdueReKYCRecordsAsync(currentDate);

        return MapMany(entities);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetReKYCRecordsDueWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var entities = await _unitOfWork.KYCDetails
            .GetReKYCRecordsDueWithinRangeAsync(startDate, endDate);

        return MapMany(entities);
    }

    public async Task<bool> UpdateReKycDueDateAsync(
        int kycDetailsId,
        DateTime nextReKycDueDate)
    {
        if (kycDetailsId <= 0)
            return false;

        if (nextReKycDueDate.Date < DateTime.UtcNow.Date)
            throw new ArgumentOutOfRangeException(
                nameof(nextReKycDueDate),
                "The re-KYC due date cannot be in the past.");

        var entity = await _unitOfWork.KYCDetails
            .GetByIdAsync(kycDetailsId);

        if (entity is null)
            return false;

        entity.NextReKycDueDate = nextReKycDueDate;

        Touch(entity);

        _unitOfWork.KYCDetails.Update(entity);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // =========================================================
    // 6. EMPLOYEE VERIFICATION AND DOCUMENTS
    // =========================================================

    public async Task<IEnumerable<KYCDetailsDto>> GetByVerifiedByEmployeeIdAsync(
        int employeeId)
    {
        if (employeeId <= 0)
            return Enumerable.Empty<KYCDetailsDto>();

        var entities = await _unitOfWork.KYCDetails
            .GetByVerifiedByEmployeeIdAsync(employeeId);

        return MapMany(entities);
    }

    public async Task<IEnumerable<KYCDetailsDto>> GetKYCRecordsWithDocumentsAsync(
        int customerId)
    {
        if (customerId <= 0)
            return Enumerable.Empty<KYCDetailsDto>();

        var entities = await _unitOfWork.KYCDetails
            .GetKYCRecordsWithDocumentsAsync(customerId);

        return MapMany(entities);
    }

    public async Task<bool> VerifyKYCAsync(
        int kycDetailsId,
        int employeeId)
    {
        EnsureVerifier();

        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        var entity = await _unitOfWork.KYCDetails
            .GetByIdAsync(kycDetailsId);

        if (entity is null || !entity.IsCurrent)
            return false;

        if (entity.VerificationStatus == "Rejected" ||
            entity.VerificationStatus == "Expired")
        {
            throw new InvalidOperationException(
                "Rejected or expired KYC records cannot be verified.");
        }

        entity.VerificationStatus = "Verified";
        entity.VerifiedAt = DateTime.UtcNow;
        entity.VerifiedByEmployeeId = employeeId;
        entity.RejectionReason = null;

        Touch(entity);

        _unitOfWork.KYCDetails.Update(entity);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RejectKYCAsync(
        int kycDetailsId,
        int employeeId,
        string rejectionReason)
    {
        EnsureVerifier();

        if (employeeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(employeeId));

        RequireValue(rejectionReason, nameof(rejectionReason));

        if (rejectionReason.Trim().Length > 500)
            throw new ArgumentException(
                "Rejection reason cannot exceed 500 characters.",
                nameof(rejectionReason));

        var entity = await _unitOfWork.KYCDetails
            .GetByIdAsync(kycDetailsId);

        if (entity is null || !entity.IsCurrent)
            return false;

        entity.VerificationStatus = "Rejected";
        entity.VerifiedAt = null;
        entity.VerifiedByEmployeeId = employeeId;
        entity.RejectionReason = rejectionReason.Trim();

        Touch(entity);

        _unitOfWork.KYCDetails.Update(entity);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // =========================================================
    // 7. DATE, SEARCH AND REPORTING
    // =========================================================

    public async Task<IEnumerable<KYCDetailsDto>> GetVerifiedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var entities = await _unitOfWork.KYCDetails
            .GetVerifiedWithinRangeAsync(startDate, endDate);

        return MapMany(entities);
    }

    public Task<int> CountCurrentRecordsByStatusAsync(
        string verificationStatus)
    {
        RequireValue(verificationStatus, nameof(verificationStatus));

        return _unitOfWork.KYCDetails
            .CountCurrentRecordsByStatusAsync(verificationStatus);
    }

    // =========================================================
    // PRIVATE HELPER METHODS
    // =========================================================

    private async Task RetireCurrentRecordAsync(
        int customerId,
        int? exceptKycDetailsId)
    {
        var currentRecords = await _unitOfWork.KYCDetails.FindAsync(x =>
            x.CustomerId == customerId &&
            x.IsCurrent &&
            (!exceptKycDetailsId.HasValue ||
             x.KYCDetailsId != exceptKycDetailsId.Value));

        foreach (var record in currentRecords)
        {
            record.IsCurrent = false;
            Touch(record);

            _unitOfWork.KYCDetails.Update(record);
        }
    }

    private void ApplyPan(KYCDetails entity, string? pan)
    {
        if (string.IsNullOrWhiteSpace(pan))
            return;

        var normalizedPan = pan.Trim().ToUpperInvariant();

        if (!System.Text.RegularExpressions.Regex.IsMatch(
                normalizedPan,
                "^[A-Z]{5}[0-9]{4}[A-Z]$"))
        {
            throw new ArgumentException(
                "PAN must be in the format ABCDE1234F.",
                nameof(pan));
        }

        entity.PanEncrypted = _panProtector.Protect(normalizedPan);
        entity.PanHash = CreatePanHash(normalizedPan);
        entity.PanMasked = $"{normalizedPan[..5]}****{normalizedPan[^1]}";
        entity.IsPanVerified = false;
        entity.PanVerifiedAt = null;
    }

    private string CreatePanHash(string pan)
    {
        if (string.IsNullOrWhiteSpace(pan))
            throw new ArgumentException("PAN is required.", nameof(pan));

        var normalizedPan = pan.Trim().ToUpperInvariant();

        if (!System.Text.RegularExpressions.Regex.IsMatch(
                normalizedPan,
                "^[A-Z]{5}[0-9]{4}[A-Z]$"))
        {
            throw new ArgumentException(
                "PAN must be in the format ABCDE1234F.",
                nameof(pan));
        }

        var configuredKey = _configuration["Security:PanHashKey"];

        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            throw new InvalidOperationException(
                "Configure Security:PanHashKey with a strong secret before using PAN lookup.");
        }

        using var hmac = new HMACSHA256(
            Encoding.UTF8.GetBytes(configuredKey));

        return Convert.ToHexString(
            hmac.ComputeHash(Encoding.UTF8.GetBytes(normalizedPan)));
    }

    private bool HasPermission()
    {
        return _currentUser.IsAuthenticated &&
               _currentUser.Permissions is not null &&
               _currentUser.Permissions.Any(permission =>
                   string.Equals(
                       permission,
                       "KYC.Verify",
                       StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(
                       permission,
                       "KYC.Manage",
                       StringComparison.OrdinalIgnoreCase));
    }

    private void EnsureVerifier()
    {
        if (!HasPermission())
            throw new UnauthorizedAccessException(
                "You are not authorized to verify or reject KYC records.");
    }

    private void Touch(KYCDetails entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = _currentUser.UserId;
    }

    private static void ValidateCreate(CreateKYCDetailsDto dto)
    {
        ValidateStatusValues(
            dto.KycLevel,
            dto.VerificationStatus,
            dto.CkycStatus,
            dto.AadhaarVerificationMethod);
    }

    private static void ValidateStatusValues(
        string kycLevel,
        string verificationStatus,
        string ckycStatus,
        string? aadhaarMethod)
    {
        if (!new[] { "Minimum", "Full", "Video", "EKyc" }.Contains(kycLevel))
            throw new ArgumentException("Invalid KYC level.", nameof(kycLevel));

        if (!new[] { "Pending", "InProgress", "Verified", "Rejected", "Expired" }
            .Contains(verificationStatus))
        {
            throw new ArgumentException(
                "Invalid KYC verification status.",
                nameof(verificationStatus));
        }

        if (!new[] { "NotInitiated", "Pending", "Uploaded", "Verified", "Rejected" }
            .Contains(ckycStatus))
        {
            throw new ArgumentException("Invalid CKYC status.", nameof(ckycStatus));
        }

        if (aadhaarMethod is not null &&
            !new[] { "OtpEkyc", "BiometricEkyc", "OfflineXml", "VirtualId" }
                .Contains(aadhaarMethod))
        {
            throw new ArgumentException(
                "Invalid Aadhaar verification method.",
                nameof(aadhaarMethod));
        }
    }

    private static void RequireValue(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A value is required.", parameterName);
    }

    private static void ValidateDateRange(
        DateTime startDate,
        DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException(
                "Start date must be earlier than or equal to end date.");
    }

    private KYCDetailsDto Map(KYCDetails entity)
        => _mapper.Map<KYCDetailsDto>(entity);

    private IEnumerable<KYCDetailsDto> MapMany(IEnumerable<KYCDetails> entities)
        => _mapper.Map<IEnumerable<KYCDetailsDto>>(entities);
}