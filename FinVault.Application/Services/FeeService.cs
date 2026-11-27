
using AutoMapper;
using FinVault.Application.DTOs.FeeDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class FeeService : IFeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private static readonly string[] ValidCalculationMethods =
    {
        "Flat", "Percentage", "Slab"
    };

    private static readonly string[] ValidFrequencies =
    {
        "PerTransaction", "Monthly", "Quarterly", "Yearly", "OneTime"
    };

    public FeeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. FEE IDENTIFICATION

    public async Task<IEnumerable<FeeDto>> GetAllAsync()
    {
        var fees = await _unitOfWork.Fee.GetAllAsync();
        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    public async Task<FeeDto?> GetByIdAsync(int feeId)
    {
        if (feeId <= 0)
            return null;

        var fee = await _unitOfWork.Fee.GetByIdAsync(feeId);
        return fee == null ? null : _mapper.Map<FeeDto>(fee);
    }

    public async Task<FeeDto?> GetByCodeAsync(string code)
    {
        ValidateRequiredText(code, nameof(code), 30);

        var fee = await _unitOfWork.Fee.GetByCodeAsync(code.Trim());
        return fee == null ? null : _mapper.Map<FeeDto>(fee);
    }

    public async Task<FeeDto?> GetFeeDetailsAsync(int feeId)
    {
        if (feeId <= 0)
            return null;

        var fee = await _unitOfWork.Fee.GetFeeDetailsAsync(feeId);
        return fee == null ? null : _mapper.Map<FeeDto>(fee);
    }

    public async Task<IEnumerable<FeeDto>> GetByNameAsync(string name)
    {
        ValidateRequiredText(name, nameof(name));

        var fees = await _unitOfWork.Fee.GetByNameAsync(name.Trim());
        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    public Task<bool> ExistsByCodeAsync(string code)
    {
        ValidateRequiredText(code, nameof(code), 30);
        return _unitOfWork.Fee.ExistsByCodeAsync(code.Trim());
    }

    // 2. FEE CRUD

    public async Task<FeeDto> CreateAsync(CreateFeeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateFee(
            dto.Code,
            dto.Name,
            dto.CalculationMethod,
            dto.Frequency,
            dto.FlatAmount,
            dto.Percentage,
            dto.MinimumAmount,
            dto.MaximumAmount,
            dto.IsGstApplicable,
            dto.GstRate,
            dto.EffectiveFrom,
            dto.EffectiveTo);

        var code = dto.Code.Trim();

        if (await _unitOfWork.Fee.ExistsByCodeAsync(code))
        {
            throw new InvalidOperationException(
                "A fee with this code already exists.");
        }

        await ValidateAssociationsAsync(
            dto.TransactionTypeId,
            dto.PaymentMethodId,
            dto.AccountTypeId);

        var fee = _mapper.Map<Fee>(dto);

        fee.Code = code;
        fee.Name = dto.Name.Trim();
        fee.CalculationMethod = dto.CalculationMethod.Trim();
        fee.Frequency = dto.Frequency.Trim();
        fee.GstRate = dto.IsGstApplicable ? dto.GstRate : 0;
        fee.CreatedAt = DateTime.UtcNow;
        fee.UpdatedAt = null;
        fee.CreatedBy = null;
        fee.UpdatedBy = null;

        await _unitOfWork.Fee.AddAsync(fee);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FeeDto>(fee);
    }

    public async Task<FeeDto?> UpdateAsync(UpdateFeeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.FeeId <= 0)
            return null;

        ValidateFee(
            dto.Code,
            dto.Name,
            dto.CalculationMethod,
            dto.Frequency,
            dto.FlatAmount,
            dto.Percentage,
            dto.MinimumAmount,
            dto.MaximumAmount,
            dto.IsGstApplicable,
            dto.GstRate,
            dto.EffectiveFrom,
            dto.EffectiveTo);

        var fee = await _unitOfWork.Fee.GetByIdAsync(dto.FeeId);

        if (fee == null)
            return null;

        var code = dto.Code.Trim();

        var existingFee = await _unitOfWork.Fee.GetByCodeAsync(code);

        if (existingFee != null && existingFee.FeeId != dto.FeeId)
        {
            throw new InvalidOperationException(
                "Another fee already uses this code.");
        }

        await ValidateAssociationsAsync(
            dto.TransactionTypeId,
            dto.PaymentMethodId,
            dto.AccountTypeId);

        var hasChargedTransactions = fee.ChargedTransactions?.Any() == true;

        if (hasChargedTransactions &&
            (fee.Code != code ||
             fee.CalculationMethod != dto.CalculationMethod ||
             fee.Frequency != dto.Frequency))
        {
            throw new InvalidOperationException(
                "The fee code, calculation method, and frequency cannot be changed " +
                "because this fee is already referenced by transactions.");
        }

        _mapper.Map(dto, fee);

        fee.Code = code;
        fee.Name = dto.Name.Trim();
        fee.CalculationMethod = dto.CalculationMethod.Trim();
        fee.Frequency = dto.Frequency.Trim();
        fee.GstRate = dto.IsGstApplicable ? dto.GstRate : 0;
        fee.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Fee.Update(fee);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FeeDto>(fee);
    }

    public async Task<bool> DeleteAsync(DeleteFeeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.FeeId <= 0)
            return false;

        var fee = await _unitOfWork.Fee.GetFeeDetailsAsync(dto.FeeId);

        if (fee == null)
            return false;

        if (fee.ChargedTransactions?.Any() == true)
        {
            throw new InvalidOperationException(
                "This fee cannot be deleted because it is already referenced by transactions.");
        }

        _unitOfWork.Fee.Delete(fee);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 3. FEE CLASSIFICATION

    public async Task<IEnumerable<FeeDto>> GetByCalculationMethodAsync(
        string calculationMethod)
    {
        ValidateAllowedValue(
            calculationMethod,
            ValidCalculationMethods,
            nameof(calculationMethod));

        var fees = await _unitOfWork.Fee
            .GetByCalculationMethodAsync(calculationMethod.Trim());

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    public async Task<IEnumerable<FeeDto>> GetByFrequencyAsync(string frequency)
    {
        ValidateAllowedValue(
            frequency,
            ValidFrequencies,
            nameof(frequency));

        var fees = await _unitOfWork.Fee
            .GetByFrequencyAsync(frequency.Trim());

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    // 4. BANKING ENTITY ASSOCIATIONS

    public async Task<IEnumerable<FeeDto>> GetByTransactionTypeIdAsync(
        int transactionTypeId)
    {
        if (transactionTypeId <= 0)
            return Enumerable.Empty<FeeDto>();

        var fees = await _unitOfWork.Fee
            .GetByTransactionTypeIdAsync(transactionTypeId);

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    public async Task<IEnumerable<FeeDto>> GetByPaymentMethodIdAsync(
        int paymentMethodId)
    {
        if (paymentMethodId <= 0)
            return Enumerable.Empty<FeeDto>();

        var fees = await _unitOfWork.Fee
            .GetByPaymentMethodIdAsync(paymentMethodId);

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    public async Task<IEnumerable<FeeDto>> GetByAccountTypeIdAsync(int accountTypeId)
    {
        if (accountTypeId <= 0)
            return Enumerable.Empty<FeeDto>();

        var fees = await _unitOfWork.Fee
            .GetByAccountTypeIdAsync(accountTypeId);

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    // 5. EFFECTIVE DATE MANAGEMENT

    public async Task<IEnumerable<FeeDto>> GetEffectiveFeesAsync(
        DateTime effectiveDate)
    {
        if (effectiveDate == default)
            throw new ArgumentException("Effective date is required.");

        var fees = await _unitOfWork.Fee
            .GetEffectiveFeesAsync(effectiveDate);

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    public async Task<IEnumerable<FeeDto>> GetUpcomingFeesAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var fees = await _unitOfWork.Fee
            .GetUpcomingFeesAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    public async Task<IEnumerable<FeeDto>> GetActiveFeesAsync()
    {
        var fees = await _unitOfWork.Fee.GetActiveFeesAsync();
        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    public async Task<IEnumerable<FeeDto>> GetInactiveFeesAsync()
    {
        var fees = await _unitOfWork.Fee.GetInactiveFeesAsync();
        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    // 6. TAX AND AMOUNT MANAGEMENT

    public async Task<IEnumerable<FeeDto>> GetGstApplicableFeesAsync()
    {
        var fees = await _unitOfWork.Fee.GetGstApplicableFeesAsync();
        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    public async Task<IEnumerable<FeeDto>> GetFeesWithinAmountRangeAsync(
        decimal minimumAmount,
        decimal maximumAmount)
    {
        if (minimumAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumAmount));

        if (maximumAmount < minimumAmount)
            throw new ArgumentException(
                "Maximum amount cannot be less than minimum amount.");

        var fees = await _unitOfWork.Fee
            .GetFeesWithinAmountRangeAsync(minimumAmount, maximumAmount);

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    // 7. TRANSACTION AND USAGE TRACKING

    public async Task<IEnumerable<FeeDto>> GetFeesWithChargedTransactionsAsync()
    {
        var fees = await _unitOfWork.Fee
            .GetFeesWithChargedTransactionsAsync();

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    // 8. AUDIT AND SEARCH

    public async Task<IEnumerable<FeeDto>> GetFeesCreatedByUserAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<FeeDto>();

        var fees = await _unitOfWork.Fee
            .GetFeesCreatedByUserAsync(userId);

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    public async Task<IEnumerable<FeeDto>> SearchFeesAsync(string searchTerm)
    {
        ValidateRequiredText(searchTerm, nameof(searchTerm));

        var fees = await _unitOfWork.Fee
            .SearchFeesAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<FeeDto>>(fees);
    }

    // VALIDATION

    private async Task ValidateAssociationsAsync(
        int? transactionTypeId,
        int? paymentMethodId,
        int? accountTypeId)
    {
        if (transactionTypeId.HasValue &&
            await _unitOfWork.TransactionType
                .GetByIdAsync(transactionTypeId.Value) == null)
        {
            throw new InvalidOperationException(
                "The specified transaction type does not exist.");
        }

        if (paymentMethodId.HasValue &&
            await _unitOfWork.PaymentMethod
                .GetByIdAsync(paymentMethodId.Value) == null)
        {
            throw new InvalidOperationException(
                "The specified payment method does not exist.");
        }

        if (accountTypeId.HasValue &&
            await _unitOfWork.AccountType
                .GetByIdAsync(accountTypeId.Value) == null)
        {
            throw new InvalidOperationException(
                "The specified account type does not exist.");
        }
    }

    private static void ValidateFee(
        string? code,
        string? name,
        string? calculationMethod,
        string? frequency,
        decimal? flatAmount,
        decimal? percentage,
        decimal? minimumAmount,
        decimal? maximumAmount,
        bool isGstApplicable,
        decimal gstRate,
        DateTime effectiveFrom,
        DateTime? effectiveTo)
    {
        ValidateRequiredText(code, nameof(code), 30);
        ValidateRequiredText(name, nameof(name), 100);

        ValidateAllowedValue(
            calculationMethod,
            ValidCalculationMethods,
            nameof(calculationMethod));

        ValidateAllowedValue(
            frequency,
            ValidFrequencies,
            nameof(frequency));

        if (flatAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(flatAmount));

        if (percentage < 0)
            throw new ArgumentOutOfRangeException(nameof(percentage));

        if (minimumAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumAmount));

        if (maximumAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(maximumAmount));

        if (minimumAmount.HasValue &&
            maximumAmount.HasValue &&
            maximumAmount.Value < minimumAmount.Value)
        {
            throw new ArgumentException(
                "Maximum amount cannot be less than minimum amount.");
        }

        if (calculationMethod == "Flat" && !flatAmount.HasValue)
        {
            throw new ArgumentException(
                "FlatAmount is required for Flat calculation.");
        }

        if (calculationMethod == "Percentage" && !percentage.HasValue)
        {
            throw new ArgumentException(
                "Percentage is required for Percentage calculation.");
        }

        if (calculationMethod == "Slab" &&
            (!minimumAmount.HasValue || !maximumAmount.HasValue))
        {
            throw new ArgumentException(
                "MinimumAmount and MaximumAmount are required for Slab calculation.");
        }

        if (gstRate < 0 || gstRate > 100)
            throw new ArgumentOutOfRangeException(
                nameof(gstRate),
                "GST rate must be between 0 and 100.");

        if (!isGstApplicable && gstRate != 0)
        {
            // GST is not applied when the fee is not GST applicable.
        }

        if (effectiveFrom == default)
            throw new ArgumentException("EffectiveFrom is required.");

        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
        {
            throw new ArgumentException(
                "EffectiveTo cannot be earlier than EffectiveFrom.");
        }
    }

    private static void ValidateAllowedValue(
        string? value,
        IEnumerable<string> allowedValues,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !allowedValues.Contains(value.Trim(), StringComparer.Ordinal))
        {
            throw new ArgumentException(
                $"Invalid value for {parameterName}.",
                parameterName);
        }
    }

    private static void ValidateRequiredText(
        string? value,
        string parameterName,
        int? maximumLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", parameterName);

        if (maximumLength.HasValue &&
            value.Trim().Length > maximumLength.Value)
        {
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength.Value} characters.",
                parameterName);
        }
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException("Start date and end date are required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }
}
