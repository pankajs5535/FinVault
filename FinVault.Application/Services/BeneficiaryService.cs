using AutoMapper;
using FinVault.Application.DTOs.BeneficiaryDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class BeneficiaryService : IBeneficiaryService
{
    private static readonly string[] ValidBeneficiaryTypes =
    {
        "IntraBank",
        "InterBank",
        "UPI"
    };

    private static readonly string[] ValidStatuses =
    {
        "Pending",
        "Active",
        "Blocked",
        "Removed"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BeneficiaryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<BeneficiaryDto>> GetAllAsync()
    {
        var beneficiaries = await _unitOfWork.Beneficiary.GetAllAsync();
        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<BeneficiaryDto?> GetByIdAsync(int beneficiaryId)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));

        var beneficiary = await _unitOfWork.Beneficiary.GetByIdAsync(beneficiaryId);
        return _mapper.Map<BeneficiaryDto?>(beneficiary);
    }

    public async Task<BeneficiaryDto?> GetByIdAndCustomerIdAsync(
        int beneficiaryId,
        int customerId)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));
        ValidateId(customerId, nameof(customerId));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByIdAndCustomerIdAsync(beneficiaryId, customerId);

        return _mapper.Map<BeneficiaryDto?>(beneficiary);
    }

    public async Task<BeneficiaryDto?> GetBeneficiaryDetailsAsync(int beneficiaryId)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetBeneficiaryDetailsAsync(beneficiaryId);

        return _mapper.Map<BeneficiaryDto?>(beneficiary);
    }

    public async Task<BeneficiaryDto> CreateAsync(CreateBeneficiaryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var beneficiaryType = NormalizeBeneficiaryType(dto.BeneficiaryType);

        ValidateBeneficiaryDetails(
            beneficiaryType,
            dto.AccountNumber,
            dto.IfscCode,
            dto.UpiVpa,
            dto.PerTransactionLimit);

        var customer = await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId);

        if (customer == null)
            throw new InvalidOperationException("Customer does not exist.");

        if (customer.Status != "Active")
            throw new InvalidOperationException(
                "Only active customers can add beneficiaries.");

        if (dto.InternalAccountId.HasValue)
        {
            var internalAccount = await _unitOfWork.Account
                .GetByIdAsync(dto.InternalAccountId.Value);

            if (internalAccount == null)
                throw new InvalidOperationException(
                    "The selected internal account does not exist.");

            if (internalAccount.Status != "Active")
                throw new InvalidOperationException(
                    "The selected internal account is not active.");
        }

        await ValidateDuplicateBeneficiaryAsync(
            dto.CustomerId,
            beneficiaryType,
            dto.AccountNumber,
            dto.IfscCode,
            dto.UpiVpa,
            null);

        var beneficiary = _mapper.Map<Beneficiary>(dto);

        // New beneficiaries must pass through the cooling period.
        beneficiary.BeneficiaryType = beneficiaryType;
        beneficiary.Status = "Pending";
        beneficiary.ActivatedAt = null;
        beneficiary.CoolingPeriodEndsAt = dto.CoolingPeriodEndsAt;
        beneficiary.CreatedAt = DateTime.UtcNow;
        beneficiary.UpdatedAt = null;

        await _unitOfWork.Beneficiary.AddAsync(beneficiary);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BeneficiaryDto>(beneficiary);
    }

    public async Task<BeneficiaryDto?> UpdateAsync(UpdateBeneficiaryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.BeneficiaryId, nameof(dto.BeneficiaryId));
        ValidateId(dto.CustomerId, nameof(dto.CustomerId));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByIdAndCustomerIdAsync(dto.BeneficiaryId, dto.CustomerId);

        if (beneficiary == null)
            return null;

        if (dto.RowVersion == null ||
            dto.RowVersion.Length == 0 ||
            beneficiary.RowVersion == null ||
            !beneficiary.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                "Beneficiary data has been modified by another operation. Reload the beneficiary and try again.");
        }

        if (beneficiary.Status == "Removed")
            throw new InvalidOperationException(
                "A removed beneficiary cannot be updated.");

        var beneficiaryType = NormalizeBeneficiaryType(dto.BeneficiaryType);

        ValidateBeneficiaryDetails(
            beneficiaryType,
            dto.AccountNumber,
            dto.IfscCode,
            dto.UpiVpa,
            dto.PerTransactionLimit);

        await ValidateDuplicateBeneficiaryAsync(
            dto.CustomerId,
            beneficiaryType,
            dto.AccountNumber,
            dto.IfscCode,
            dto.UpiVpa,
            dto.BeneficiaryId);

        if (dto.InternalAccountId.HasValue)
        {
            var internalAccount = await _unitOfWork.Account
                .GetByIdAsync(dto.InternalAccountId.Value);

            if (internalAccount == null)
                throw new InvalidOperationException(
                    "The selected internal account does not exist.");

            if (internalAccount.Status != "Active")
                throw new InvalidOperationException(
                    "The selected internal account is not active.");
        }

        var originalStatus = beneficiary.Status;
        var originalActivatedAt = beneficiary.ActivatedAt;
        var originalCoolingPeriodEndsAt = beneficiary.CoolingPeriodEndsAt;
        var originalCustomerId = beneficiary.CustomerId;
        var originalCreatedAt = beneficiary.CreatedAt;
        var originalCreatedBy = beneficiary.CreatedBy;
        var originalRowVersion = beneficiary.RowVersion;

        _mapper.Map(dto, beneficiary);

        // Ownership, lifecycle status, cooling period and concurrency data
        // are controlled by the service, not by ordinary update DTO values.
        beneficiary.CustomerId = originalCustomerId;
        beneficiary.Status = originalStatus;
        beneficiary.ActivatedAt = originalActivatedAt;
        beneficiary.CoolingPeriodEndsAt = originalCoolingPeriodEndsAt;
        beneficiary.CreatedAt = originalCreatedAt;
        beneficiary.CreatedBy = originalCreatedBy;
        beneficiary.RowVersion = originalRowVersion;
        beneficiary.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BeneficiaryDto>(beneficiary);
    }

    public async Task<bool> DeleteAsync(DeleteBeneficiaryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.BeneficiaryId, nameof(dto.BeneficiaryId));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByIdAsync(dto.BeneficiaryId);

        if (beneficiary == null)
            return false;

        if (beneficiary.Status == "Removed")
            return true;

        // Soft removal preserves records referenced by payment requests.
        beneficiary.Status = "Removed";
        beneficiary.IsFavorite = false;
        beneficiary.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. CUSTOMER BENEFICIARY MANAGEMENT

    public async Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetBeneficiariesByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<IEnumerable<BeneficiaryDto>> GetActiveBeneficiariesByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetActiveBeneficiariesByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<IEnumerable<BeneficiaryDto>> GetPendingBeneficiariesByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetPendingBeneficiariesByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<IEnumerable<BeneficiaryDto>> GetBlockedBeneficiariesByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetBlockedBeneficiariesByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<IEnumerable<BeneficiaryDto>> GetRemovedBeneficiariesByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetRemovedBeneficiariesByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    // 3. BENEFICIARY TYPE AND PAYMENT IDENTIFICATION

    public async Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesByTypeAsync(
        int customerId,
        string beneficiaryType)
    {
        ValidateId(customerId, nameof(customerId));

        var normalizedType = NormalizeBeneficiaryType(beneficiaryType);

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetBeneficiariesByTypeAsync(customerId, normalizedType);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<BeneficiaryDto?> GetByAccountAndIfscAsync(
        int customerId,
        string accountNumber,
        string ifscCode)
    {
        ValidateId(customerId, nameof(customerId));
        ValidateRequiredString(accountNumber, nameof(accountNumber));
        ValidateRequiredString(ifscCode, nameof(ifscCode));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByAccountAndIfscAsync(
                customerId,
                accountNumber.Trim(),
                ifscCode.Trim().ToUpperInvariant());

        return _mapper.Map<BeneficiaryDto?>(beneficiary);
    }

    public async Task<BeneficiaryDto?> GetByUpiVpaAsync(
        int customerId,
        string upiVpa)
    {
        ValidateId(customerId, nameof(customerId));
        ValidateRequiredString(upiVpa, nameof(upiVpa));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByUpiVpaAsync(customerId, upiVpa.Trim());

        return _mapper.Map<BeneficiaryDto?>(beneficiary);
    }

    // 4. FAVORITES AND COOLING PERIOD

    public async Task<IEnumerable<BeneficiaryDto>> GetFavoriteBeneficiariesAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetFavoriteBeneficiariesAsync(customerId);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesReadyForActivationAsync(
        DateTime currentDateTime)
    {
        ValidateDate(currentDateTime, nameof(currentDateTime));

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetBeneficiariesReadyForActivationAsync(currentDateTime);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesInCoolingPeriodAsync(
        DateTime currentDateTime)
    {
        ValidateDate(currentDateTime, nameof(currentDateTime));

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetBeneficiariesInCoolingPeriodAsync(currentDateTime);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<bool> SetFavoriteAsync(int beneficiaryId, bool isFavorite)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByIdAsync(beneficiaryId);

        if (beneficiary == null)
            return false;

        if (beneficiary.Status == "Removed")
            throw new InvalidOperationException(
                "A removed beneficiary cannot be marked as favorite.");

        beneficiary.IsFavorite = isFavorite;
        beneficiary.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 5. INTERNAL ACCOUNT AND TRANSACTION LIMITS

    public async Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesByInternalAccountIdAsync(
        int internalAccountId)
    {
        ValidateId(internalAccountId, nameof(internalAccountId));

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetBeneficiariesByInternalAccountIdAsync(internalAccountId);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<IEnumerable<BeneficiaryDto>> GetActiveBeneficiariesWithinTransactionLimitAsync(
        int customerId,
        decimal amount)
    {
        ValidateId(customerId, nameof(customerId));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Transaction amount must be greater than zero.");

        var beneficiaries = await _unitOfWork.Beneficiary
            .GetActiveBeneficiariesWithinTransactionLimitAsync(
                customerId,
                amount);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<bool> UpdatePerTransactionLimitAsync(
        int beneficiaryId,
        decimal? limit)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));

        if (limit.HasValue && limit.Value < 0)
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Transaction limit cannot be negative.");

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByIdAsync(beneficiaryId);

        if (beneficiary == null)
            return false;

        if (beneficiary.Status == "Removed")
            throw new InvalidOperationException(
                "A removed beneficiary's transaction limit cannot be updated.");

        beneficiary.PerTransactionLimit = limit;
        beneficiary.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 6. STATUS AND ACTIVATION MANAGEMENT

    public async Task<bool> ActivateAsync(int beneficiaryId)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByIdAsync(beneficiaryId);

        if (beneficiary == null)
            return false;

        if (beneficiary.Status == "Active")
            return true;

        if (beneficiary.Status != "Pending")
            throw new InvalidOperationException(
                "Only a pending beneficiary can be activated.");

        if (!beneficiary.CoolingPeriodEndsAt.HasValue)
            throw new InvalidOperationException(
                "The beneficiary cooling-period end date has not been configured.");

        if (beneficiary.CoolingPeriodEndsAt.Value > DateTime.UtcNow)
            throw new InvalidOperationException(
                "The beneficiary is still within its cooling period.");

        var customer = await _unitOfWork.Customer
            .GetByIdAsync(beneficiary.CustomerId);

        if (customer == null || customer.Status != "Active")
            throw new InvalidOperationException(
                "The beneficiary's customer must be active before activation.");

        await ValidateDuplicateBeneficiaryAsync(
            beneficiary.CustomerId,
            beneficiary.BeneficiaryType,
            beneficiary.AccountNumber,
            beneficiary.IfscCode,
            beneficiary.UpiVpa,
            beneficiary.BeneficiaryId);

        beneficiary.Status = "Active";
        beneficiary.ActivatedAt = DateTime.UtcNow;
        beneficiary.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> BlockAsync(int beneficiaryId)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByIdAsync(beneficiaryId);

        if (beneficiary == null)
            return false;

        if (beneficiary.Status == "Removed")
            throw new InvalidOperationException(
                "A removed beneficiary cannot be blocked.");

        if (beneficiary.Status == "Blocked")
            return true;

        beneficiary.Status = "Blocked";
        beneficiary.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveAsync(int beneficiaryId)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByIdAsync(beneficiaryId);

        if (beneficiary == null)
            return false;

        if (beneficiary.Status == "Removed")
            return true;

        beneficiary.Status = "Removed";
        beneficiary.IsFavorite = false;
        beneficiary.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateCoolingPeriodAsync(
        int beneficiaryId,
        DateTime coolingPeriodEndsAt)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));
        ValidateDate(coolingPeriodEndsAt, nameof(coolingPeriodEndsAt));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetByIdAsync(beneficiaryId);

        if (beneficiary == null)
            return false;

        if (beneficiary.Status != "Pending")
            throw new InvalidOperationException(
                "The cooling period can only be changed for pending beneficiaries.");

        if (coolingPeriodEndsAt <= DateTime.UtcNow)
            throw new ArgumentException(
                "Cooling period end must be in the future.",
                nameof(coolingPeriodEndsAt));

        beneficiary.CoolingPeriodEndsAt = coolingPeriodEndsAt;
        beneficiary.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 7. SEARCH AND VALIDATION

    public async Task<IEnumerable<BeneficiaryDto>> SearchAsync(
        int customerId,
        string searchTerm,
        int pageNumber,
        int pageSize)
    {
        ValidateId(customerId, nameof(customerId));
        ValidateRequiredString(searchTerm, nameof(searchTerm));

        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                "Page number must be greater than zero.");

        if (pageSize <= 0 || pageSize > 200)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be between 1 and 200.");

        var beneficiaries = await _unitOfWork.Beneficiary
            .SearchBeneficiariesAsync(
                customerId,
                searchTerm.Trim(),
                pageNumber,
                pageSize);

        return _mapper.Map<IEnumerable<BeneficiaryDto>>(beneficiaries);
    }

    public async Task<bool> ExistsForCustomerAsync(
        int beneficiaryId,
        int customerId)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));
        ValidateId(customerId, nameof(customerId));

        return await _unitOfWork.Beneficiary
            .ExistsForCustomerAsync(beneficiaryId, customerId);
    }

    public async Task<bool> HasActiveBankBeneficiaryAsync(
        int customerId,
        string accountNumber,
        string ifscCode)
    {
        ValidateId(customerId, nameof(customerId));
        ValidateRequiredString(accountNumber, nameof(accountNumber));
        ValidateRequiredString(ifscCode, nameof(ifscCode));

        return await _unitOfWork.Beneficiary
            .HasActiveBankBeneficiaryAsync(
                customerId,
                accountNumber.Trim(),
                ifscCode.Trim().ToUpperInvariant());
    }

    public async Task<bool> HasActiveUpiBeneficiaryAsync(
        int customerId,
        string upiVpa)
    {
        ValidateId(customerId, nameof(customerId));
        ValidateRequiredString(upiVpa, nameof(upiVpa));

        return await _unitOfWork.Beneficiary
            .HasActiveUpiBeneficiaryAsync(customerId, upiVpa.Trim());
    }

    public async Task<bool> CanBeneficiaryBeDeletedAsync(int beneficiaryId)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));

        var beneficiary = await _unitOfWork.Beneficiary
            .GetBeneficiaryDetailsAsync(beneficiaryId);

        if (beneficiary == null)
            return false;

        // PaymentRequests are loaded by GetBeneficiaryDetailsAsync
        // according to the repository contract's detail-loading purpose.
        return !beneficiary.PaymentRequests.Any();
    }

    // PRIVATE VALIDATION METHODS

    private async Task ValidateDuplicateBeneficiaryAsync(
        int customerId,
        string beneficiaryType,
        string? accountNumber,
        string? ifscCode,
        string? upiVpa,
        int? excludeBeneficiaryId)
    {
        if (beneficiaryType == "UPI")
        {
            var duplicate = await _unitOfWork.Beneficiary.ExistsAsync(
                x => x.CustomerId == customerId &&
                     x.BeneficiaryType == "UPI" &&
                     x.UpiVpa == upiVpa &&
                     x.Status != "Removed" &&
                     x.BeneficiaryId != excludeBeneficiaryId);

            if (duplicate)
                throw new InvalidOperationException(
                    "This UPI VPA is already registered for the customer.");

            return;
        }

        var bankDuplicate = await _unitOfWork.Beneficiary.ExistsAsync(
            x => x.CustomerId == customerId &&
                 x.BeneficiaryType != "UPI" &&
                 x.AccountNumber == accountNumber &&
                 x.IfscCode == ifscCode &&
                 x.Status != "Removed" &&
                 x.BeneficiaryId != excludeBeneficiaryId);

        if (bankDuplicate)
            throw new InvalidOperationException(
                "This bank account number and IFSC combination is already registered for the customer.");
    }

    private static void ValidateBeneficiaryDetails(
        string beneficiaryType,
        string? accountNumber,
        string? ifscCode,
        string? upiVpa,
        decimal? perTransactionLimit)
    {
        if (perTransactionLimit.HasValue && perTransactionLimit.Value < 0)
            throw new ArgumentOutOfRangeException(
                nameof(perTransactionLimit),
                "Per-transaction limit cannot be negative.");

        if (beneficiaryType == "UPI")
        {
            ValidateRequiredString(upiVpa, nameof(upiVpa));

            if (!string.IsNullOrWhiteSpace(accountNumber) ||
                !string.IsNullOrWhiteSpace(ifscCode))
            {
                throw new ArgumentException(
                    "UPI beneficiaries must use a UPI VPA instead of bank account and IFSC details.");
            }

            return;
        }

        ValidateRequiredString(accountNumber, nameof(accountNumber));
        ValidateRequiredString(ifscCode, nameof(ifscCode));

        if (!System.Text.RegularExpressions.Regex.IsMatch(
                ifscCode.Trim().ToUpperInvariant(),
                "^[A-Z]{4}0[A-Z0-9]{6}$"))
        {
            throw new ArgumentException("Invalid IFSC code.", nameof(ifscCode));
        }

        if (!string.IsNullOrWhiteSpace(upiVpa))
            throw new ArgumentException(
                "Bank beneficiaries must not contain a UPI VPA.",
                nameof(upiVpa));
    }

    private static string NormalizeBeneficiaryType(string beneficiaryType)
    {
        if (string.IsNullOrWhiteSpace(beneficiaryType))
            throw new ArgumentException(
                "Beneficiary type is required.",
                nameof(beneficiaryType));

        var normalized = ValidBeneficiaryTypes.FirstOrDefault(
            x => string.Equals(
                x,
                beneficiaryType.Trim(),
                StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid beneficiary type. Allowed values: {string.Join(", ", ValidBeneficiaryTypes)}.",
            nameof(beneficiaryType));
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "ID must be greater than zero.");
    }

    private static void ValidateRequiredString(
        string? value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
    }

    private static void ValidateDate(DateTime date, string parameterName)
    {
        if (date == default)
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
    }
}