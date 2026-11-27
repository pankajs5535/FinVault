using AutoMapper;
using FinVault.Application.DTOs.PaymentMethodDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private static readonly string[] ValidRails =
    {
        "UPI", "NEFT", "RTGS", "IMPS", "NACH", "Cheque", "Card", "Internal"
    };

    private static readonly string[] ValidSettlementTypes =
    {
        "RealTime", "DeferredNet", "Batch"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentMethodService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<PaymentMethodDto>> GetAllAsync()
    {
        var methods = await _unitOfWork.PaymentMethod.GetAllAsync();
        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<PaymentMethodDto?> GetByIdAsync(int paymentMethodId)
    {
        ValidateId(paymentMethodId, nameof(paymentMethodId));

        var method = await _unitOfWork.PaymentMethod.GetByIdAsync(paymentMethodId);
        return _mapper.Map<PaymentMethodDto?>(method);
    }

    public async Task<PaymentMethodDto?> GetByCodeAsync(string code)
    {
        ValidateRequiredString(code, nameof(code));

        var method = await _unitOfWork.PaymentMethod.GetByCodeAsync(
            code.Trim().ToUpperInvariant());

        return _mapper.Map<PaymentMethodDto?>(method);
    }

    public async Task<PaymentMethodDto?> GetByNameAsync(string name)
    {
        ValidateRequiredString(name, nameof(name));

        var method = await _unitOfWork.PaymentMethod.GetByNameAsync(name.Trim());

        return _mapper.Map<PaymentMethodDto?>(method);
    }

    public async Task<PaymentMethodDto?> GetPaymentMethodDetailsAsync(
        int paymentMethodId)
    {
        ValidateId(paymentMethodId, nameof(paymentMethodId));

        var method = await _unitOfWork.PaymentMethod
            .GetPaymentMethodDetailsAsync(paymentMethodId);

        return _mapper.Map<PaymentMethodDto?>(method);
    }

    public async Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateMethodDetails(
            dto.Code,
            dto.Name,
            dto.Rail,
            dto.Description,
            dto.MinAmount,
            dto.MaxAmount,
            dto.DailyLimit,
            dto.SettlementType);

        var normalizedCode = dto.Code.Trim().ToUpperInvariant();

        if (await _unitOfWork.PaymentMethod.ExistsByCodeAsync(normalizedCode))
            throw new InvalidOperationException(
                $"Payment method code '{normalizedCode}' already exists.");

        var existingName = await _unitOfWork.PaymentMethod.GetByNameAsync(
            dto.Name.Trim());

        if (existingName != null)
            throw new InvalidOperationException(
                $"Payment method name '{dto.Name.Trim()}' already exists.");

        var method = _mapper.Map<PaymentMethod>(dto);

        method.Code = normalizedCode;
        method.Name = dto.Name.Trim();
        method.Rail = NormalizeValue(dto.Rail, ValidRails, nameof(dto.Rail));
        method.SettlementType = NormalizeValue(
            dto.SettlementType,
            ValidSettlementTypes,
            nameof(dto.SettlementType));
        method.Description = string.IsNullOrWhiteSpace(dto.Description)
            ? null
            : dto.Description.Trim();
        method.CreatedAt = DateTime.UtcNow;
        method.UpdatedAt = null;

        await _unitOfWork.PaymentMethod.AddAsync(method);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PaymentMethodDto>(method);
    }

    public async Task<PaymentMethodDto?> UpdateAsync(UpdatePaymentMethodDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.PaymentMethodId, nameof(dto.PaymentMethodId));

        var method = await _unitOfWork.PaymentMethod
            .GetByIdAsync(dto.PaymentMethodId);

        if (method == null)
            return null;

        ValidateMethodDetails(
            dto.Code,
            dto.Name,
            dto.Rail,
            dto.Description,
            dto.MinAmount,
            dto.MaxAmount,
            dto.DailyLimit,
            dto.SettlementType);

        var normalizedCode = dto.Code.Trim().ToUpperInvariant();

        var existingCode = await _unitOfWork.PaymentMethod
            .GetByCodeAsync(normalizedCode);

        if (existingCode != null &&
            existingCode.PaymentMethodId != dto.PaymentMethodId)
        {
            throw new InvalidOperationException(
                $"Payment method code '{normalizedCode}' already exists.");
        }

        var existingName = await _unitOfWork.PaymentMethod
            .GetByNameAsync(dto.Name.Trim());

        if (existingName != null &&
            existingName.PaymentMethodId != dto.PaymentMethodId)
        {
            throw new InvalidOperationException(
                $"Payment method name '{dto.Name.Trim()}' already exists.");
        }

        // Preserve identity and audit properties.
        var originalId = method.PaymentMethodId;
        var originalCreatedAt = method.CreatedAt;
        var originalCreatedBy = method.CreatedBy;

        // A referenced method cannot be physically removed or disabled
        // by an ordinary update if doing so would break active payment operations.
        var isInUse = await _unitOfWork.PaymentMethod
            .IsPaymentMethodInUseAsync(dto.PaymentMethodId);

        if (!dto.IsActive && isInUse)
        {
            throw new InvalidOperationException(
                "This payment method is referenced by payment requests, fees or reconciliation batches. Review outstanding operations before deactivating it.");
        }

        _mapper.Map(dto, method);

        method.PaymentMethodId = originalId;
        method.CreatedAt = originalCreatedAt;
        method.CreatedBy = originalCreatedBy;
        method.Code = normalizedCode;
        method.Name = dto.Name.Trim();
        method.Rail = NormalizeValue(dto.Rail, ValidRails, nameof(dto.Rail));
        method.SettlementType = NormalizeValue(
            dto.SettlementType,
            ValidSettlementTypes,
            nameof(dto.SettlementType));
        method.Description = string.IsNullOrWhiteSpace(dto.Description)
            ? null
            : dto.Description.Trim();
        method.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.PaymentMethod.Update(method);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PaymentMethodDto>(method);
    }

    public async Task<bool> DeleteAsync(DeletePaymentMethodDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.PaymentMethodId, nameof(dto.PaymentMethodId));

        var method = await _unitOfWork.PaymentMethod
            .GetByIdAsync(dto.PaymentMethodId);

        if (method == null)
            return false;

        var isInUse = await _unitOfWork.PaymentMethod
            .IsPaymentMethodInUseAsync(dto.PaymentMethodId);

        if (isInUse)
        {
            // Referenced payment methods are deactivated rather than deleted.
            if (!method.IsActive)
                return true;

            method.IsActive = false;
            method.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PaymentMethod.Update(method);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        _unitOfWork.PaymentMethod.Delete(method);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        ValidateRequiredString(code, nameof(code));

        return await _unitOfWork.PaymentMethod
            .ExistsByCodeAsync(code.Trim().ToUpperInvariant());
    }

    // 2. RAIL AND SETTLEMENT CLASSIFICATION

    public async Task<IEnumerable<PaymentMethodDto>> GetByRailAsync(string rail)
    {
        var normalizedRail = NormalizeValue(rail, ValidRails, nameof(rail));

        var methods = await _unitOfWork.PaymentMethod
            .GetByRailAsync(normalizedRail);

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetBySettlementTypeAsync(
        string settlementType)
    {
        var normalizedType = NormalizeValue(
            settlementType,
            ValidSettlementTypes,
            nameof(settlementType));

        var methods = await _unitOfWork.PaymentMethod
            .GetBySettlementTypeAsync(normalizedType);

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<IEnumerable<PaymentMethodDto>> Get24x7PaymentMethodsAsync()
    {
        var methods = await _unitOfWork.PaymentMethod
            .Get24x7PaymentMethodsAsync();

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    // 3. AVAILABILITY AND STATUS

    public async Task<IEnumerable<PaymentMethodDto>> GetActivePaymentMethodsAsync()
    {
        var methods = await _unitOfWork.PaymentMethod
            .GetActivePaymentMethodsAsync();

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetInactivePaymentMethodsAsync()
    {
        var methods = await _unitOfWork.PaymentMethod
            .GetInactivePaymentMethodsAsync();

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetAvailableMethodsForAmountAsync(
        decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Payment amount must be greater than zero.");

        var methods = await _unitOfWork.PaymentMethod
            .GetAvailableMethodsForAmountAsync(amount);

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    // 4. LIMIT AND SETTLEMENT CONFIGURATION

    public async Task<IEnumerable<PaymentMethodDto>> GetByMinimumAmountRangeAsync(
        decimal minimumAmount,
        decimal maximumAmount)
    {
        if (minimumAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumAmount));

        if (maximumAmount < minimumAmount)
            throw new ArgumentException(
                "Maximum amount cannot be less than minimum amount.");

        var methods = await _unitOfWork.PaymentMethod
            .GetByMinimumAmountRangeAsync(minimumAmount, maximumAmount);

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetWithDailyLimitAboveAsync(
        decimal minimumDailyLimit)
    {
        if (minimumDailyLimit < 0)
            throw new ArgumentOutOfRangeException(
                nameof(minimumDailyLimit));

        var methods = await _unitOfWork.PaymentMethod
            .GetWithDailyLimitAboveAsync(minimumDailyLimit);

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetWithoutMaximumAmountAsync()
    {
        var methods = await _unitOfWork.PaymentMethod
            .GetWithoutMaximumAmountAsync();

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    // 5. RELATED PAYMENT OPERATIONS

    public async Task<PaymentMethodDto?> GetWithPaymentRequestsAsync(
        int paymentMethodId)
    {
        ValidateId(paymentMethodId, nameof(paymentMethodId));

        var method = await _unitOfWork.PaymentMethod
            .GetWithPaymentRequestsAsync(paymentMethodId);

        return _mapper.Map<PaymentMethodDto?>(method);
    }

    public async Task<PaymentMethodDto?> GetWithFeesAsync(int paymentMethodId)
    {
        ValidateId(paymentMethodId, nameof(paymentMethodId));

        var method = await _unitOfWork.PaymentMethod
            .GetWithFeesAsync(paymentMethodId);

        return _mapper.Map<PaymentMethodDto?>(method);
    }

    public async Task<PaymentMethodDto?> GetWithReconciliationBatchesAsync(
        int paymentMethodId)
    {
        ValidateId(paymentMethodId, nameof(paymentMethodId));

        var method = await _unitOfWork.PaymentMethod
            .GetWithReconciliationBatchesAsync(paymentMethodId);

        return _mapper.Map<PaymentMethodDto?>(method);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetMethodsWithPaymentRequestsAsync()
    {
        var methods = await _unitOfWork.PaymentMethod
            .GetMethodsWithPaymentRequestsAsync();

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetMethodsWithFeesAsync()
    {
        var methods = await _unitOfWork.PaymentMethod
            .GetMethodsWithFeesAsync();

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<IEnumerable<PaymentMethodDto>>
        GetMethodsWithReconciliationBatchesAsync()
    {
        var methods = await _unitOfWork.PaymentMethod
            .GetMethodsWithReconciliationBatchesAsync();

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    // 6. ADMINISTRATION AND SEARCH

    public async Task<IEnumerable<PaymentMethodDto>> GetCreatedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var methods = await _unitOfWork.PaymentMethod
            .GetCreatedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<IEnumerable<PaymentMethodDto>> SearchPaymentMethodsAsync(
        string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm));

        var methods = await _unitOfWork.PaymentMethod
            .SearchPaymentMethodsAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
    }

    public async Task<bool> IsPaymentMethodInUseAsync(int paymentMethodId)
    {
        ValidateId(paymentMethodId, nameof(paymentMethodId));

        return await _unitOfWork.PaymentMethod
            .IsPaymentMethodInUseAsync(paymentMethodId);
    }

    // PRIVATE VALIDATION METHODS

    private static void ValidateMethodDetails(
        string code,
        string name,
        string rail,
        string? description,
        decimal minAmount,
        decimal? maxAmount,
        decimal? dailyLimit,
        string settlementType)
    {
        ValidateRequiredString(code, nameof(code));
        ValidateRequiredString(name, nameof(name));

        if (code.Trim().Length > 20)
            throw new ArgumentException(
                "Payment method code cannot exceed 20 characters.");

        if (name.Trim().Length > 100)
            throw new ArgumentException(
                "Payment method name cannot exceed 100 characters.");

        if (description?.Length > 250)
            throw new ArgumentException(
                "Description cannot exceed 250 characters.");

        NormalizeValue(rail, ValidRails, nameof(rail));
        NormalizeValue(
            settlementType,
            ValidSettlementTypes,
            nameof(settlementType));

        if (minAmount < 0)
            throw new ArgumentOutOfRangeException(
                nameof(minAmount),
                "Minimum amount cannot be negative.");

        if (maxAmount.HasValue && maxAmount.Value < minAmount)
            throw new ArgumentException(
                "Maximum amount cannot be less than minimum amount.");

        if (dailyLimit.HasValue && dailyLimit.Value <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(dailyLimit),
                "Daily limit must be greater than zero when configured.");

        if (maxAmount.HasValue &&
            dailyLimit.HasValue &&
            dailyLimit.Value < maxAmount.Value)
        {
            throw new ArgumentException(
                "Daily limit cannot be lower than the maximum transaction amount.");
        }
    }

    private static string NormalizeValue(
        string value,
        IEnumerable<string> allowedValues,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);

        var normalized = allowedValues.FirstOrDefault(
            x => string.Equals(
                x,
                value.Trim(),
                StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid {parameterName}. Allowed values: {string.Join(", ", allowedValues)}.",
            parameterName);
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

    private static void ValidateDateRange(
        DateTime startDate,
        DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException(
                "Start date and end date are required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot precede start date.");
    }
}