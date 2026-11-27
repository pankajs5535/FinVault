using AutoMapper;
using FinVault.Application.DTOs.TransactionTypeDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class TransactionTypeService : ITransactionTypeService
{
    private static readonly string[] ValidCategories =
    {
        "Deposit", "Withdrawal", "Transfer", "Payment", "Fee",
        "Interest", "Refund", "Reversal", "Loan", "Card"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransactionTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<TransactionTypeDto>> GetAllAsync()
    {
        var types = await _unitOfWork.TransactionType.GetAllAsync();
        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<TransactionTypeDto?> GetByIdAsync(int transactionTypeId)
    {
        ValidateId(transactionTypeId, nameof(transactionTypeId));

        var type = await _unitOfWork.TransactionType.GetByIdAsync(transactionTypeId);
        return _mapper.Map<TransactionTypeDto?>(type);
    }

    public async Task<TransactionTypeDto?> GetByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var type = await _unitOfWork.TransactionType.GetByCodeAsync(NormalizeCode(code));
        return _mapper.Map<TransactionTypeDto?>(type);
    }

    public async Task<IEnumerable<TransactionTypeDto>> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Transaction type name is required.", nameof(name));

        var types = await _unitOfWork.TransactionType.GetByNameAsync(name.Trim());
        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<TransactionTypeDto?> GetTransactionTypeDetailsAsync(int transactionTypeId)
    {
        ValidateId(transactionTypeId, nameof(transactionTypeId));

        var type = await _unitOfWork.TransactionType
            .GetTransactionTypeDetailsAsync(transactionTypeId);

        return _mapper.Map<TransactionTypeDto?>(type);
    }

    public async Task<TransactionTypeDto> CreateAsync(CreateTransactionTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var code = NormalizeCode(dto.Code);
        var name = NormalizeName(dto.Name);
        var category = NormalizeCategory(dto.Category);

        ValidateBusinessRules(
            dto.RequiresApproval,
            dto.ApprovalThreshold,
            dto.DailyLimit);

        if (await ExistsByCodeAsync(code))
            throw new InvalidOperationException(
                $"Transaction type code '{code}' already exists.");

        if (await IsNameAlreadyUsedAsync(name))
            throw new InvalidOperationException(
                $"Transaction type name '{name}' already exists.");

        var type = _mapper.Map<TransactionType>(dto);

        type.Code = code;
        type.Name = name;
        type.Category = category;
        type.Description = dto.Description?.Trim();
        type.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.TransactionType.AddAsync(type);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransactionTypeDto>(type);
    }

    public async Task<TransactionTypeDto?> UpdateAsync(UpdateTransactionTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.TransactionTypeId, nameof(dto.TransactionTypeId));

        var type = await _unitOfWork.TransactionType
            .GetByIdAsync(dto.TransactionTypeId);

        if (type == null)
            return null;

        var code = NormalizeCode(dto.Code);
        var name = NormalizeName(dto.Name);
        var category = NormalizeCategory(dto.Category);

        ValidateBusinessRules(
            dto.RequiresApproval,
            dto.ApprovalThreshold,
            dto.DailyLimit);

        var existingCode = await _unitOfWork.TransactionType.GetByCodeAsync(code);

        if (existingCode != null &&
            existingCode.TransactionTypeId != dto.TransactionTypeId)
        {
            throw new InvalidOperationException(
                $"Transaction type code '{code}' already exists.");
        }

        if (await IsNameAlreadyUsedAsync(name, dto.TransactionTypeId))
            throw new InvalidOperationException(
                $"Transaction type name '{name}' already exists.");

        if (!dto.IsActive && type.IsActive &&
            await IsTransactionTypeInUseAsync(dto.TransactionTypeId))
        {
            throw new InvalidOperationException(
                "A transaction type referenced by existing transactions or fees cannot be deactivated.");
        }

        _mapper.Map(dto, type);

        type.Code = code;
        type.Name = name;
        type.Category = category;
        type.Description = dto.Description?.Trim();
        type.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.TransactionType.Update(type);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransactionTypeDto>(type);
    }

    public async Task<bool> DeleteAsync(DeleteTransactionTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.TransactionTypeId, nameof(dto.TransactionTypeId));

        var type = await _unitOfWork.TransactionType
            .GetByIdAsync(dto.TransactionTypeId);

        if (type == null ||
            await IsTransactionTypeInUseAsync(dto.TransactionTypeId))
            return false;

        _unitOfWork.TransactionType.Delete(type);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. CATEGORY AND STATUS

    public async Task<IEnumerable<TransactionTypeDto>> GetByCategoryAsync(string category)
    {
        category = NormalizeCategory(category);

        var types = await _unitOfWork.TransactionType.GetByCategoryAsync(category);
        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<IEnumerable<TransactionTypeDto>> GetActiveTransactionTypesAsync()
    {
        var types = await _unitOfWork.TransactionType.GetActiveTransactionTypesAsync();
        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<IEnumerable<TransactionTypeDto>> GetInactiveTransactionTypesAsync()
    {
        var types = await _unitOfWork.TransactionType.GetInactiveTransactionTypesAsync();
        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<bool> ActivateAsync(int transactionTypeId)
    {
        ValidateId(transactionTypeId, nameof(transactionTypeId));

        var type = await _unitOfWork.TransactionType.GetByIdAsync(transactionTypeId);

        if (type == null)
            return false;

        if (type.IsActive)
            return true;

        type.IsActive = true;
        type.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.TransactionType.Update(type);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(int transactionTypeId)
    {
        ValidateId(transactionTypeId, nameof(transactionTypeId));

        var type = await _unitOfWork.TransactionType.GetByIdAsync(transactionTypeId);

        if (type == null)
            return false;

        if (!type.IsActive)
            return true;

        if (await IsTransactionTypeInUseAsync(transactionTypeId))
            throw new InvalidOperationException(
                "A transaction type referenced by existing transactions or fees cannot be deactivated.");

        type.IsActive = false;
        type.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.TransactionType.Update(type);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 3. APPROVAL AND REVERSAL RULES

    public async Task<IEnumerable<TransactionTypeDto>> GetApprovalRequiredTransactionTypesAsync()
    {
        var types = await _unitOfWork.TransactionType
            .GetApprovalRequiredTransactionTypesAsync();

        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<IEnumerable<TransactionTypeDto>> GetReversibleTransactionTypesAsync()
    {
        var types = await _unitOfWork.TransactionType
            .GetReversibleTransactionTypesAsync();

        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<IEnumerable<TransactionTypeDto>> GetNonReversibleTransactionTypesAsync()
    {
        var types = await _unitOfWork.TransactionType
            .GetNonReversibleTransactionTypesAsync();

        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<IEnumerable<TransactionTypeDto>> GetTransactionTypesWithApprovalThresholdAsync()
    {
        var types = await _unitOfWork.TransactionType
            .GetTransactionTypesWithApprovalThresholdAsync();

        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<IEnumerable<TransactionTypeDto>> GetTransactionTypesWithDailyLimitAsync()
    {
        var types = await _unitOfWork.TransactionType
            .GetTransactionTypesWithDailyLimitAsync();

        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    // 4. BUSINESS RELATIONSHIPS

    public async Task<IEnumerable<TransactionTypeDto>> GetTransactionTypesWithTransactionsAsync()
    {
        var types = await _unitOfWork.TransactionType
            .GetTransactionTypesWithTransactionsAsync();

        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<IEnumerable<TransactionTypeDto>> GetTransactionTypesWithFeesAsync()
    {
        var types = await _unitOfWork.TransactionType
            .GetTransactionTypesWithFeesAsync();

        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    // 5. SEARCH AND VALIDATION

    public async Task<IEnumerable<TransactionTypeDto>> GetRecentlyCreatedTransactionTypesAsync(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        var types = await _unitOfWork.TransactionType
            .GetRecentlyCreatedTransactionTypesAsync(count);

        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<IEnumerable<TransactionTypeDto>> SearchTransactionTypesAsync(
        string? searchTerm,
        string? category,
        bool? isActive,
        bool? requiresApproval,
        bool? isReversible,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var normalizedCategory = string.IsNullOrWhiteSpace(category)
            ? null
            : NormalizeCategory(category);

        var types = await _unitOfWork.TransactionType.SearchTransactionTypesAsync(
            searchTerm?.Trim(),
            normalizedCategory,
            isActive,
            requiresApproval,
            isReversible,
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<TransactionTypeDto>>(types);
    }

    public async Task<bool> IsTransactionTypeInUseAsync(int transactionTypeId)
    {
        ValidateId(transactionTypeId, nameof(transactionTypeId));

        return await _unitOfWork.TransactionType
            .IsTransactionTypeInUseAsync(transactionTypeId);
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        return await _unitOfWork.TransactionType
            .GetByCodeAsync(NormalizeCode(code)) != null;
    }

    // PRIVATE VALIDATION HELPERS

    private async Task<bool> IsNameAlreadyUsedAsync(
        string name,
        int? excludeTransactionTypeId = null)
    {
        var types = await _unitOfWork.TransactionType.GetByNameAsync(name);

        return types.Any(x =>
            x.TransactionTypeId != excludeTransactionTypeId &&
            string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    private static void ValidateBusinessRules(
        bool requiresApproval,
        decimal? approvalThreshold,
        decimal? dailyLimit)
    {
        if (approvalThreshold < 0)
            throw new ArgumentOutOfRangeException(
                nameof(approvalThreshold),
                "Approval threshold cannot be negative.");

        if (dailyLimit < 0)
            throw new ArgumentOutOfRangeException(
                nameof(dailyLimit),
                "Daily limit cannot be negative.");

        if (approvalThreshold.HasValue && !requiresApproval)
            throw new InvalidOperationException(
                "An approval threshold requires approval to be enabled.");
    }

    private static string NormalizeCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException(
                "Transaction category is required.",
                nameof(category));

        var normalized = ValidCategories.FirstOrDefault(x =>
            string.Equals(x, category.Trim(), StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid transaction category. Allowed values: {string.Join(", ", ValidCategories)}.",
            nameof(category));
    }

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Transaction type code is required.",
                nameof(code));

        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Transaction type name is required.",
                nameof(name));

        return name.Trim();
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "ID must be greater than zero.");
    }
}