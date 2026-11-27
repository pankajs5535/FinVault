using AutoMapper;
using FinVault.Application.DTOs.AccountTypeDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class AccountTypeService : IAccountTypeService
{
    private static readonly string[] ValidCategories =
    {
        "Savings", "Current", "FixedDeposit",
        "RecurringDeposit", "Internal"
    };

    private static readonly string[] ValidInterestPayoutFrequencies =
    {
        "Monthly", "Quarterly", "HalfYearly", "Yearly", "AtMaturity"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AccountTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<AccountTypeDto>> GetAllAsync()
    {
        var accountTypes = await _unitOfWork.AccountType.GetAllAsync();
        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    public async Task<AccountTypeDto?> GetByIdAsync(int accountTypeId)
    {
        ValidateId(accountTypeId, nameof(accountTypeId));

        var accountType = await _unitOfWork.AccountType.GetByIdAsync(accountTypeId);
        return _mapper.Map<AccountTypeDto?>(accountType);
    }

    public async Task<AccountTypeDto?> GetByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var accountType = await _unitOfWork.AccountType.GetByCodeAsync(NormalizeCode(code));
        return _mapper.Map<AccountTypeDto?>(accountType);
    }

    public async Task<IEnumerable<AccountTypeDto>> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account type name is required.", nameof(name));

        var accountTypes = await _unitOfWork.AccountType.GetByNameAsync(name.Trim());
        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    public async Task<AccountTypeDto?> GetAccountTypeDetailsAsync(int accountTypeId)
    {
        ValidateId(accountTypeId, nameof(accountTypeId));

        var accountType = await _unitOfWork.AccountType.GetAccountTypeDetailsAsync(accountTypeId);
        return _mapper.Map<AccountTypeDto?>(accountType);
    }

    public async Task<AccountTypeDto> CreateAsync(CreateAccountTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var code = NormalizeCode(dto.Code);
        var name = NormalizeName(dto.Name);
        var category = NormalizeCategory(dto.Category);

        ValidateBusinessRules(
            dto.MinimumBalance,
            dto.MinimumAverageBalance,
            dto.DailyTransactionLimit,
            dto.IsInterestBearing,
            dto.InterestPayoutFrequency,
            dto.MinTenureMonths,
            dto.MaxTenureMonths);

        if (await IsCodeExistsAsync(code))
            throw new InvalidOperationException($"Account type code '{code}' already exists.");

        if (await IsNameExistsAsync(name))
            throw new InvalidOperationException($"Account type name '{name}' already exists.");

        var accountType = _mapper.Map<AccountType>(dto);
        accountType.Code = code;
        accountType.Name = name;
        accountType.Category = category;
        accountType.InterestPayoutFrequency =
            NormalizeInterestPayoutFrequency(dto.InterestPayoutFrequency);
        accountType.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.AccountType.AddAsync(accountType);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AccountTypeDto>(accountType);
    }

    public async Task<AccountTypeDto?> UpdateAsync(UpdateAccountTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.AccountTypeId, nameof(dto.AccountTypeId));

        var accountType = await _unitOfWork.AccountType.GetByIdAsync(dto.AccountTypeId);
        if (accountType == null)
            return null;

        var code = NormalizeCode(dto.Code);
        var name = NormalizeName(dto.Name);
        var category = NormalizeCategory(dto.Category);

        ValidateBusinessRules(
            dto.MinimumBalance,
            dto.MinimumAverageBalance,
            dto.DailyTransactionLimit,
            dto.IsInterestBearing,
            dto.InterestPayoutFrequency,
            dto.MinTenureMonths,
            dto.MaxTenureMonths);

        var existingByCode = await _unitOfWork.AccountType.GetByCodeAsync(code);
        if (existingByCode != null && existingByCode.AccountTypeId != dto.AccountTypeId)
            throw new InvalidOperationException($"Account type code '{code}' already exists.");

        var existingByName = await _unitOfWork.AccountType.GetByNameAsync(name);
        if (existingByName.Any(x =>
                x.AccountTypeId != dto.AccountTypeId &&
                string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Account type name '{name}' already exists.");
        }

        if (!dto.IsActive && accountType.IsActive &&
            await IsAccountTypeInUseAsync(dto.AccountTypeId))
        {
            throw new InvalidOperationException(
                "This account type cannot be deactivated while accounts, interest rates, or fees reference it.");
        }

        _mapper.Map(dto, accountType);

        accountType.Code = code;
        accountType.Name = name;
        accountType.Category = category;
        accountType.InterestPayoutFrequency =
            NormalizeInterestPayoutFrequency(dto.InterestPayoutFrequency);
        accountType.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.AccountType.Update(accountType);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AccountTypeDto>(accountType);
    }

    public async Task<bool> DeleteAsync(DeleteAccountTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.AccountTypeId, nameof(dto.AccountTypeId));

        var accountType = await _unitOfWork.AccountType.GetByIdAsync(dto.AccountTypeId);
        if (accountType == null || !await CanAccountTypeBeDeletedAsync(dto.AccountTypeId))
            return false;

        _unitOfWork.AccountType.Delete(accountType);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. CATEGORY AND STATUS

    public async Task<IEnumerable<AccountTypeDto>> GetByCategoryAsync(string category)
    {
        category = NormalizeCategory(category);

        var accountTypes = await _unitOfWork.AccountType.GetByCategoryAsync(category);
        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    public async Task<IEnumerable<AccountTypeDto>> GetActiveAccountTypesAsync()
    {
        var accountTypes = await _unitOfWork.AccountType.GetActiveAccountTypesAsync();
        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    public async Task<IEnumerable<AccountTypeDto>> GetInactiveAccountTypesAsync()
    {
        var accountTypes = await _unitOfWork.AccountType.GetInactiveAccountTypesAsync();
        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    public async Task<bool> ActivateAsync(int accountTypeId)
    {
        ValidateId(accountTypeId, nameof(accountTypeId));

        var accountType = await _unitOfWork.AccountType.GetByIdAsync(accountTypeId);
        if (accountType == null)
            return false;

        if (accountType.IsActive)
            return true;

        accountType.IsActive = true;
        accountType.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.AccountType.Update(accountType);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(int accountTypeId)
    {
        ValidateId(accountTypeId, nameof(accountTypeId));

        var accountType = await _unitOfWork.AccountType.GetByIdAsync(accountTypeId);
        if (accountType == null)
            return false;

        if (!accountType.IsActive)
            return true;

        if (await IsAccountTypeInUseAsync(accountTypeId))
            throw new InvalidOperationException(
                "This account type cannot be deactivated while accounts, interest rates, or fees reference it.");

        accountType.IsActive = false;
        accountType.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.AccountType.Update(accountType);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 3. ACCOUNT PRODUCT FEATURES

    public async Task<IEnumerable<AccountTypeDto>> GetInterestBearingAccountTypesAsync()
    {
        var accountTypes = await _unitOfWork.AccountType.GetInterestBearingAccountTypesAsync();
        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    public async Task<IEnumerable<AccountTypeDto>> GetOverdraftAccountTypesAsync()
    {
        var accountTypes = await _unitOfWork.AccountType.GetOverdraftAccountTypesAsync();
        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    public async Task<IEnumerable<AccountTypeDto>> GetAccountTypesByTenureRangeAsync(
        int minTenureMonths,
        int maxTenureMonths)
    {
        if (minTenureMonths < 0)
            throw new ArgumentOutOfRangeException(nameof(minTenureMonths));

        if (maxTenureMonths < minTenureMonths)
            throw new ArgumentOutOfRangeException(
                nameof(maxTenureMonths),
                "Maximum tenure must be greater than or equal to minimum tenure.");

        var accountTypes = await _unitOfWork.AccountType
            .GetAccountTypesByTenureRangeAsync(minTenureMonths, maxTenureMonths);

        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    // 4. BUSINESS RELATIONSHIPS

    public async Task<IEnumerable<AccountTypeDto>> GetAccountTypesWithAccountsAsync()
    {
        var accountTypes = await _unitOfWork.AccountType.GetAccountTypesWithAccountsAsync();
        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    public async Task<IEnumerable<AccountTypeDto>> GetAccountTypesWithInterestRatesAsync()
    {
        var accountTypes = await _unitOfWork.AccountType.GetAccountTypesWithInterestRatesAsync();
        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    public async Task<IEnumerable<AccountTypeDto>> GetAccountTypesWithFeesAsync()
    {
        var accountTypes = await _unitOfWork.AccountType.GetAccountTypesWithFeesAsync();
        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    // 5. SEARCH AND ADMINISTRATION

    public async Task<IEnumerable<AccountTypeDto>> GetRecentlyCreatedAccountTypesAsync(int count)
    {
        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count));

        var accountTypes = await _unitOfWork.AccountType
            .GetRecentlyCreatedAccountTypesAsync(count);

        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    public async Task<IEnumerable<AccountTypeDto>> SearchAsync(
        string? searchTerm,
        string? category,
        bool? isActive,
        bool? isInterestBearing,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var normalizedCategory = string.IsNullOrWhiteSpace(category)
            ? null
            : NormalizeCategory(category);

        var accountTypes = await _unitOfWork.AccountType.SearchAccountTypesAsync(
            searchTerm?.Trim(),
            normalizedCategory,
            isActive,
            isInterestBearing,
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<AccountTypeDto>>(accountTypes);
    }

    // 6. VALIDATION AND USAGE

    public async Task<bool> IsCodeExistsAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        return await _unitOfWork.AccountType.GetByCodeAsync(NormalizeCode(code)) != null;
    }

    public async Task<bool> IsNameExistsAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var accountTypes = await _unitOfWork.AccountType.GetByNameAsync(name.Trim());

        return accountTypes.Any(x =>
            string.Equals(x.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> IsAccountTypeInUseAsync(int accountTypeId)
    {
        ValidateId(accountTypeId, nameof(accountTypeId));

        return await _unitOfWork.AccountType.IsAccountTypeInUseAsync(accountTypeId);
    }

    public async Task<bool> CanAccountTypeBeDeletedAsync(int accountTypeId)
    {
        ValidateId(accountTypeId, nameof(accountTypeId));

        var accountType = await _unitOfWork.AccountType.GetByIdAsync(accountTypeId);
        if (accountType == null)
            return false;

        return !await IsAccountTypeInUseAsync(accountTypeId);
    }

    // PRIVATE BUSINESS-RULE HELPERS

    private static void ValidateBusinessRules(
        decimal minimumBalance,
        decimal minimumAverageBalance,
        decimal? dailyTransactionLimit,
        bool isInterestBearing,
        string? interestPayoutFrequency,
        int? minTenureMonths,
        int? maxTenureMonths)
    {
        if (minimumBalance < 0)
            throw new ArgumentOutOfRangeException(
                nameof(minimumBalance),
                "Minimum balance cannot be negative.");

        if (minimumAverageBalance < 0)
            throw new ArgumentOutOfRangeException(
                nameof(minimumAverageBalance),
                "Minimum average balance cannot be negative.");

        if (dailyTransactionLimit < 0)
            throw new ArgumentOutOfRangeException(
                nameof(dailyTransactionLimit),
                "Daily transaction limit cannot be negative.");

        if (minTenureMonths < 0)
            throw new ArgumentOutOfRangeException(
                nameof(minTenureMonths),
                "Minimum tenure cannot be negative.");

        if (maxTenureMonths < 0)
            throw new ArgumentOutOfRangeException(
                nameof(maxTenureMonths),
                "Maximum tenure cannot be negative.");

        if (minTenureMonths.HasValue &&
            maxTenureMonths.HasValue &&
            minTenureMonths.Value > maxTenureMonths.Value)
        {
            throw new InvalidOperationException(
                "Minimum tenure cannot exceed maximum tenure.");
        }

        if (isInterestBearing)
        {
            if (string.IsNullOrWhiteSpace(interestPayoutFrequency))
                throw new InvalidOperationException(
                    "Interest payout frequency is required for an interest-bearing account type.");

            NormalizeInterestPayoutFrequency(interestPayoutFrequency);
        }
        else if (!string.IsNullOrWhiteSpace(interestPayoutFrequency))
        {
            throw new InvalidOperationException(
                "Interest payout frequency must not be configured for a non-interest-bearing account type.");
        }
    }

    private static string NormalizeCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Account type category is required.", nameof(category));

        var normalized = ValidCategories.FirstOrDefault(
            x => string.Equals(x, category.Trim(), StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid account type category. Allowed values: {string.Join(", ", ValidCategories)}.",
            nameof(category));
    }

    private static string? NormalizeInterestPayoutFrequency(string? frequency)
    {
        if (string.IsNullOrWhiteSpace(frequency))
            return null;

        var normalized = ValidInterestPayoutFrequencies.FirstOrDefault(
            x => string.Equals(x, frequency.Trim(), StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid interest payout frequency. Allowed values: {string.Join(", ", ValidInterestPayoutFrequencies)}.",
            nameof(frequency));
    }

    private static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Account type code is required.", nameof(code));

        return code.Trim().ToUpperInvariant();
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account type name is required.", nameof(name));

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