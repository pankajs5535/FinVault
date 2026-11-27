using AutoMapper;
using FinVault.Application.DTOs.AccountHolderDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class AccountHolderService : IAccountHolderService
{
    private static readonly string[] ValidHolderTypes =
    {
        "Primary",
        "Joint",
        "Guardian",
        "AuthorizedSignatory"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AccountHolderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<AccountHolderDto>> GetAllAsync()
    {
        var holders = await _unitOfWork.AccountHolder.GetAllAsync();
        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    public async Task<AccountHolderDto?> GetByIdAsync(int accountHolderId)
    {
        ValidateId(accountHolderId, nameof(accountHolderId));

        var holder = await _unitOfWork.AccountHolder.GetByIdAsync(accountHolderId);
        return _mapper.Map<AccountHolderDto?>(holder);
    }

    public async Task<AccountHolderDto?> GetByAccountAndCustomerAsync(
        int accountId,
        int customerId)
    {
        ValidateId(accountId, nameof(accountId));
        ValidateId(customerId, nameof(customerId));

        var holder = await _unitOfWork.AccountHolder
            .GetByAccountAndCustomerAsync(accountId, customerId);

        return _mapper.Map<AccountHolderDto?>(holder);
    }

    public async Task<AccountHolderDto?> GetAccountHolderDetailsAsync(
        int accountHolderId)
    {
        ValidateId(accountHolderId, nameof(accountHolderId));

        var holder = await _unitOfWork.AccountHolder
            .GetAccountHolderDetailsAsync(accountHolderId);

        return _mapper.Map<AccountHolderDto?>(holder);
    }

    public async Task<AccountHolderDto> CreateAsync(CreateAccountHolderDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var holderType = NormalizeHolderType(dto.HolderType);

        ValidateEffectivePeriod(dto.EffectiveFrom, dto.EffectiveTo);

        var effectiveFrom = dto.EffectiveFrom == default
            ? DateTime.UtcNow.Date
            : dto.EffectiveFrom.Date;

        var account = await _unitOfWork.Account.GetByIdAsync(dto.AccountId);

        if (account == null)
            throw new InvalidOperationException(
                "The selected account does not exist.");

        if (account.Status is "Closed" or "Frozen" or "Inactive")
            throw new InvalidOperationException(
                "An account in Closed, Frozen or Inactive status cannot receive a new account holder.");

        var customer = await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId);

        if (customer == null)
            throw new InvalidOperationException(
                "The selected customer does not exist.");

        if (customer.Status != "Active")
            throw new InvalidOperationException(
                "Only active customers can be assigned to an account.");

        var duplicateExists = await _unitOfWork.AccountHolder.ExistsAsync(
            x => x.AccountId == dto.AccountId &&
                 x.CustomerId == dto.CustomerId &&
                 x.IsActive);

        if (duplicateExists)
            throw new InvalidOperationException(
                "This customer is already an active holder of the selected account.");

        await ValidateHolderRulesAsync(
            dto.AccountId,
            customer,
            holderType,
            dto.CanOperate,
            dto.IsActive,
            effectiveFrom,
            null);

        var holder = _mapper.Map<AccountHolder>(dto);

        holder.HolderType = holderType;
        holder.EffectiveFrom = effectiveFrom;
        holder.EffectiveTo = dto.EffectiveTo?.Date;
        holder.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.AccountHolder.AddAsync(holder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AccountHolderDto>(holder);
    }

    public async Task<AccountHolderDto?> UpdateAsync(UpdateAccountHolderDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.AccountHolderId, nameof(dto.AccountHolderId));

        var holder = await _unitOfWork.AccountHolder
            .GetByIdAsync(dto.AccountHolderId);

        if (holder == null)
            return null;

        // Preserve the original account/customer relationship.
        if (holder.AccountId != dto.AccountId ||
            holder.CustomerId != dto.CustomerId)
        {
            throw new InvalidOperationException(
                "An existing account holder cannot be transferred to another account or customer. Create a new relationship instead.");
        }

        var holderType = NormalizeHolderType(dto.HolderType);

        ValidateEffectivePeriod(dto.EffectiveFrom, dto.EffectiveTo);

        var customer = await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId);

        if (customer == null)
            throw new InvalidOperationException(
                "The associated customer does not exist.");

        if (dto.IsActive && customer.Status != "Active")
            throw new InvalidOperationException(
                "An inactive customer cannot have an active account holder relationship.");

        var duplicateExists = await _unitOfWork.AccountHolder.ExistsAsync(
            x => x.AccountId == dto.AccountId &&
                 x.CustomerId == dto.CustomerId &&
                 x.AccountHolderId != dto.AccountHolderId &&
                 x.IsActive);

        if (dto.IsActive && duplicateExists)
            throw new InvalidOperationException(
                "Another active holder relationship already exists for this account and customer.");

        await ValidateHolderRulesAsync(
            dto.AccountId,
            customer,
            holderType,
            dto.CanOperate,
            dto.IsActive,
            dto.EffectiveFrom,
            dto.AccountHolderId);

        // Maintain exactly one active primary holder when replacing the primary.
        var existingPrimary = holder.HolderType == "Primary" && holder.IsActive;
        var becomingPrimary = holderType == "Primary" && dto.IsActive;

        if (becomingPrimary && !existingPrimary)
        {
            var otherPrimary = await _unitOfWork.AccountHolder.FindAsync(
                x => x.AccountId == dto.AccountId &&
                     x.HolderType == "Primary" &&
                     x.IsActive &&
                     x.AccountHolderId != dto.AccountHolderId);

            if (otherPrimary.Any())
            {
                var account = await _unitOfWork.Account.GetByIdAsync(dto.AccountId);

                if (account?.ModeOfOperation == "Single")
                    throw new InvalidOperationException(
                        "The primary holder of a Single account cannot be replaced through this operation.");

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    foreach (var previousPrimary in otherPrimary)
                    {
                        previousPrimary.HolderType = "Joint";
                        previousPrimary.UpdatedAt = DateTime.UtcNow;
                        _unitOfWork.AccountHolder.Update(previousPrimary);
                    }

                    ApplyUpdate(dto, holder, holderType);

                    _unitOfWork.AccountHolder.Update(holder);
                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return _mapper.Map<AccountHolderDto>(holder);
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
        }

        if (existingPrimary &&
            (!dto.IsActive || holderType != "Primary"))
        {
            await EnsurePrimaryHolderRemainsAsync(dto.AccountId, dto.AccountHolderId);
        }

        if (holder.IsActive && !dto.IsActive)
        {
            await ValidateDeactivationRulesAsync(holder);
        }

        ApplyUpdate(dto, holder, holderType);

        _unitOfWork.AccountHolder.Update(holder);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AccountHolderDto>(holder);
    }

    public async Task<bool> DeleteAsync(DeleteAccountHolderDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.AccountHolderId, nameof(dto.AccountHolderId));

        var holder = await _unitOfWork.AccountHolder
            .GetByIdAsync(dto.AccountHolderId);

        if (holder == null)
            return false;

        if (!holder.IsActive)
            return true;

        await ValidateDeactivationRulesAsync(holder);

        holder.IsActive = false;
        holder.CanOperate = false;
        holder.EffectiveTo = DateTime.UtcNow.Date;
        holder.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.AccountHolder.Update(holder);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. ACCOUNT AND CUSTOMER RELATIONSHIPS

    public async Task<IEnumerable<AccountHolderDto>> GetAccountHoldersByAccountIdAsync(
        int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var holders = await _unitOfWork.AccountHolder
            .GetAccountHoldersByAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    public async Task<IEnumerable<AccountHolderDto>> GetAccountHoldersByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var holders = await _unitOfWork.AccountHolder
            .GetAccountHoldersByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    // 3. STATUS AND HOLDER CLASSIFICATION

    public async Task<IEnumerable<AccountHolderDto>> GetActiveAccountHoldersAsync()
    {
        var holders = await _unitOfWork.AccountHolder.GetActiveAccountHoldersAsync();
        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    public async Task<IEnumerable<AccountHolderDto>> GetInactiveAccountHoldersAsync()
    {
        var holders = await _unitOfWork.AccountHolder.GetInactiveAccountHoldersAsync();
        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    public async Task<IEnumerable<AccountHolderDto>> GetPrimaryAccountHoldersAsync()
    {
        var holders = await _unitOfWork.AccountHolder.GetPrimaryAccountHoldersAsync();
        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    public async Task<IEnumerable<AccountHolderDto>> GetJointAccountHoldersAsync()
    {
        var holders = await _unitOfWork.AccountHolder.GetJointAccountHoldersAsync();
        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    public async Task<IEnumerable<AccountHolderDto>> GetGuardianAccountHoldersAsync()
    {
        var holders = await _unitOfWork.AccountHolder.GetGuardianAccountHoldersAsync();
        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    public async Task<IEnumerable<AccountHolderDto>> GetAuthorizedSignatoriesAsync()
    {
        var holders = await _unitOfWork.AccountHolder.GetAuthorizedSignatoriesAsync();
        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    public async Task<IEnumerable<AccountHolderDto>> GetAccountOperatorsAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var holders = await _unitOfWork.AccountHolder
            .GetAccountOperatorsAsync(accountId);

        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    // 4. EFFECTIVE PERIOD MANAGEMENT

    public async Task<IEnumerable<AccountHolderDto>> GetAccountHoldersEffectiveOnDateAsync(
        DateTime effectiveDate)
    {
        if (effectiveDate == default)
            throw new ArgumentException(
                "Effective date is required.",
                nameof(effectiveDate));

        var holders = await _unitOfWork.AccountHolder
            .GetAccountHoldersEffectiveOnDateAsync(effectiveDate.Date);

        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    public async Task<IEnumerable<AccountHolderDto>> GetAccountHoldersEndingWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var holders = await _unitOfWork.AccountHolder
            .GetAccountHoldersEndingWithinRangeAsync(
                startDate.Date,
                endDate.Date);

        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    // 5. PERMISSIONS AND STATUS MANAGEMENT

    public async Task<bool> ActivateAsync(int accountHolderId)
    {
        ValidateId(accountHolderId, nameof(accountHolderId));

        var holder = await _unitOfWork.AccountHolder
            .GetByIdAsync(accountHolderId);

        if (holder == null)
            return false;

        if (holder.IsActive)
            return true;

        var customer = await _unitOfWork.Customer.GetByIdAsync(holder.CustomerId);

        if (customer == null || customer.Status != "Active")
            throw new InvalidOperationException(
                "Only an active customer can have an active account holder relationship.");

        await ValidateHolderRulesAsync(
            holder.AccountId,
            customer,
            holder.HolderType,
            holder.CanOperate,
            true,
            holder.EffectiveFrom,
            holder.AccountHolderId);

        var duplicateExists = await _unitOfWork.AccountHolder.ExistsAsync(
            x => x.AccountId == holder.AccountId &&
                 x.CustomerId == holder.CustomerId &&
                 x.IsActive &&
                 x.AccountHolderId != holder.AccountHolderId);

        if (duplicateExists)
            throw new InvalidOperationException(
                "Another active account holder relationship already exists.");

        if (holder.HolderType == "Primary")
        {
            var anotherPrimary = await _unitOfWork.AccountHolder.ExistsAsync(
                x => x.AccountId == holder.AccountId &&
                     x.HolderType == "Primary" &&
                     x.IsActive &&
                     x.AccountHolderId != holder.AccountHolderId);

            if (anotherPrimary)
                throw new InvalidOperationException(
                    "The account already has an active primary holder.");
        }

        holder.IsActive = true;
        holder.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.AccountHolder.Update(holder);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(int accountHolderId)
    {
        ValidateId(accountHolderId, nameof(accountHolderId));

        var holder = await _unitOfWork.AccountHolder
            .GetByIdAsync(accountHolderId);

        if (holder == null)
            return false;

        if (!holder.IsActive)
            return true;

        await ValidateDeactivationRulesAsync(holder);

        holder.IsActive = false;
        holder.CanOperate = false;
        holder.EffectiveTo = DateTime.UtcNow.Date;
        holder.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.AccountHolder.Update(holder);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateOperatingPermissionAsync(
        int accountHolderId,
        bool canOperate)
    {
        ValidateId(accountHolderId, nameof(accountHolderId));

        var holder = await _unitOfWork.AccountHolder
            .GetByIdAsync(accountHolderId);

        if (holder == null)
            return false;

        if (canOperate)
        {
            if (!holder.IsActive)
                throw new InvalidOperationException(
                    "An inactive account holder cannot be granted operating permission.");

            if (!IsEffectiveOnDate(holder, DateTime.UtcNow.Date))
                throw new InvalidOperationException(
                    "Operating permission cannot be granted outside the holder's effective period.");

            var customer = await _unitOfWork.Customer.GetByIdAsync(holder.CustomerId);

            if (customer == null || customer.Status != "Active")
                throw new InvalidOperationException(
                    "The customer must be active to receive operating permission.");

            if (IsMinor(customer))
                throw new InvalidOperationException(
                    "A minor customer cannot be granted independent operating permission.");
        }
        else if (holder.CanOperate)
        {
            await EnsureMinorAccountHasOperatorAsync(holder);
        }

        holder.CanOperate = canOperate;
        holder.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.AccountHolder.Update(holder);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateEffectivePeriodAsync(
        int accountHolderId,
        DateTime effectiveFrom,
        DateTime? effectiveTo)
    {
        ValidateId(accountHolderId, nameof(accountHolderId));
        ValidateEffectivePeriod(effectiveFrom, effectiveTo);

        var holder = await _unitOfWork.AccountHolder
            .GetByIdAsync(accountHolderId);

        if (holder == null)
            return false;

        if (holder.IsActive && effectiveTo.HasValue &&
            effectiveTo.Value.Date < DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException(
                "An active account holder cannot be assigned an effective end date in the past. Deactivate the relationship instead.");
        }

        holder.EffectiveFrom = effectiveFrom.Date;
        holder.EffectiveTo = effectiveTo?.Date;
        holder.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.AccountHolder.Update(holder);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 6. SEARCH AND VALIDATION

    public async Task<IEnumerable<AccountHolderDto>> SearchAsync(
        int? accountId,
        int? customerId,
        string? holderType,
        bool? isActive,
        bool? canOperate,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        if (accountId.HasValue)
            ValidateId(accountId.Value, nameof(accountId));

        if (customerId.HasValue)
            ValidateId(customerId.Value, nameof(customerId));

        var normalizedHolderType = string.IsNullOrWhiteSpace(holderType)
            ? null
            : NormalizeHolderType(holderType);

        var holders = await _unitOfWork.AccountHolder
            .SearchAccountHoldersAsync(
                accountId,
                customerId,
                normalizedHolderType,
                isActive,
                canOperate,
                pageNumber,
                pageSize);

        return _mapper.Map<IEnumerable<AccountHolderDto>>(holders);
    }

    public async Task<bool> IsActiveAccountHolderAsync(
        int accountId,
        int customerId)
    {
        ValidateId(accountId, nameof(accountId));
        ValidateId(customerId, nameof(customerId));

        var holder = await _unitOfWork.AccountHolder
            .GetByAccountAndCustomerAsync(accountId, customerId);

        return holder != null &&
               holder.IsActive &&
               IsEffectiveOnDate(holder, DateTime.UtcNow.Date);
    }

    public async Task<bool> CanCustomerOperateAccountAsync(
        int accountId,
        int customerId)
    {
        ValidateId(accountId, nameof(accountId));
        ValidateId(customerId, nameof(customerId));

        var account = await _unitOfWork.Account.GetByIdAsync(accountId);

        if (account == null || account.Status != "Active")
            return false;

        var holder = await _unitOfWork.AccountHolder
            .GetByAccountAndCustomerAsync(accountId, customerId);

        if (holder == null ||
            !holder.IsActive ||
            !holder.CanOperate ||
            !IsEffectiveOnDate(holder, DateTime.UtcNow.Date))
            return false;

        var customer = await _unitOfWork.Customer.GetByIdAsync(customerId);

        return customer != null &&
               customer.Status == "Active" &&
               !IsMinor(customer);
    }

    // PRIVATE BUSINESS-RULE HELPERS

    private async Task ValidateHolderRulesAsync(
        int accountId,
        Customer customer,
        string holderType,
        bool canOperate,
        bool isActive,
        DateTime effectiveFrom,
        int? excludeAccountHolderId)
    {
        var account = await _unitOfWork.Account.GetByIdAsync(accountId);

        if (account == null)
            throw new InvalidOperationException(
                "The selected account does not exist.");

        if (holderType == "Joint" &&
            account.ModeOfOperation == "Single")
        {
            throw new InvalidOperationException(
                "A Single account cannot have a Joint holder.");
        }

        if (isActive && holderType != "Primary" && holderType != "Guardian")
        {
            var hasPrimary = await _unitOfWork.AccountHolder.ExistsAsync(
                x => x.AccountId == accountId &&
                     x.HolderType == "Primary" &&
                     x.IsActive &&
                     x.AccountHolderId != excludeAccountHolderId);

            if (!hasPrimary)
                throw new InvalidOperationException(
                    "An active primary holder must be assigned before adding a joint holder or authorized signatory.");
        }

        if (isActive && IsMinor(customer))
        {
            if (canOperate)
                throw new InvalidOperationException(
                    "A minor customer cannot be granted independent operating permission.");

            var hasGuardian = await _unitOfWork.AccountHolder.ExistsAsync(
                x => x.AccountId == accountId &&
                     x.HolderType == "Guardian" &&
                     x.IsActive &&
                     x.AccountHolderId != excludeAccountHolderId);

            if (!hasGuardian)
                throw new InvalidOperationException(
                    "A minor account holder requires an active guardian relationship before the minor can be added.");
        }

        if (isActive && holderType == "Guardian")
        {
            if (IsMinor(customer))
                throw new InvalidOperationException(
                    "A minor customer cannot act as a guardian.");

            if (CalculateAge(customer.DateOfBirth) < 18)
                throw new InvalidOperationException(
                    "A guardian must be at least 18 years old.");
        }

        if (isActive && canOperate && effectiveFrom.Date <= DateTime.UtcNow.Date &&
            customer.Status != "Active")
        {
            throw new InvalidOperationException(
                "Only an active customer can operate an account.");
        }
    }

    private async Task ValidateDeactivationRulesAsync(AccountHolder holder)
    {
        if (!holder.IsActive)
            return;

        if (holder.HolderType == "Primary")
        {
            var account = await _unitOfWork.Account.GetByIdAsync(holder.AccountId);

            if (account != null && account.Status != "Closed")
            {
                throw new InvalidOperationException(
                    "The active primary holder cannot be deactivated while the account is open. Assign a replacement primary holder first.");
            }
        }

        if (holder.HolderType == "Guardian")
        {
            var hasActiveMinorHolder = await _unitOfWork.AccountHolder.ExistsAsync(
                x => x.AccountId == holder.AccountId &&
                     x.IsActive &&
                     x.HolderType != "Guardian");

            var anotherGuardianExists = await _unitOfWork.AccountHolder.ExistsAsync(
                x => x.AccountId == holder.AccountId &&
                     x.HolderType == "Guardian" &&
                     x.IsActive &&
                     x.AccountHolderId != holder.AccountHolderId);

            if (hasActiveMinorHolder && !anotherGuardianExists)
            {
                var activeHolders = await _unitOfWork.AccountHolder
                    .GetAccountHoldersByAccountIdAsync(holder.AccountId);

                var hasMinor = false;

                foreach (var activeHolder in activeHolders.Where(
                             x => x.IsActive && x.HolderType != "Guardian"))
                {
                    var customer = await _unitOfWork.Customer
                        .GetByIdAsync(activeHolder.CustomerId);

                    if (customer != null && IsMinor(customer))
                    {
                        hasMinor = true;
                        break;
                    }
                }

                if (hasMinor)
                    throw new InvalidOperationException(
                        "A guardian cannot be deactivated while a minor account holder depends on that guardian.");
            }
        }

        await EnsureMinorAccountHasOperatorAsync(holder);
    }

    private async Task EnsureMinorAccountHasOperatorAsync(AccountHolder holder)
    {
        if (!holder.CanOperate)
            return;

        var customer = await _unitOfWork.Customer.GetByIdAsync(holder.CustomerId);

        if (customer == null || !IsMinor(customer))
            return;

        var anotherGuardianOperator = await _unitOfWork.AccountHolder.ExistsAsync(
            x => x.AccountId == holder.AccountId &&
                 x.AccountHolderId != holder.AccountHolderId &&
                 x.HolderType == "Guardian" &&
                 x.IsActive &&
                 x.CanOperate);

        if (!anotherGuardianOperator)
            throw new InvalidOperationException(
                "A minor account must retain an active guardian with operating permission.");
    }

    private async Task EnsurePrimaryHolderRemainsAsync(
        int accountId,
        int currentAccountHolderId)
    {
        var anotherPrimaryExists = await _unitOfWork.AccountHolder.ExistsAsync(
            x => x.AccountId == accountId &&
                 x.HolderType == "Primary" &&
                 x.IsActive &&
                 x.AccountHolderId != currentAccountHolderId);

        var account = await _unitOfWork.Account.GetByIdAsync(accountId);

        if (!anotherPrimaryExists && account != null && account.Status != "Closed")
            throw new InvalidOperationException(
                "An open account must retain an active primary holder.");
    }

    private static void ApplyUpdate(
        UpdateAccountHolderDto dto,
        AccountHolder holder,
        string holderType)
    {
        holder.HolderType = holderType;
        holder.CanOperate = dto.CanOperate;
        holder.EffectiveFrom = dto.EffectiveFrom.Date;
        holder.EffectiveTo = dto.EffectiveTo?.Date;
        holder.IsActive = dto.IsActive;
        holder.UpdatedAt = DateTime.UtcNow;
    }

    private static bool IsEffectiveOnDate(
        AccountHolder holder,
        DateTime date)
    {
        return holder.EffectiveFrom.Date <= date.Date &&
               (!holder.EffectiveTo.HasValue ||
                holder.EffectiveTo.Value.Date >= date.Date);
    }

    private static bool IsMinor(Customer customer)
    {
        return customer.CustomerType == "Minor" ||
               CalculateAge(customer.DateOfBirth) < 18;
    }

    private static int CalculateAge(DateTime dateOfBirth)
    {
        var today = DateTime.UtcNow.Date;
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age))
            age--;

        return age;
    }

    private static string NormalizeHolderType(string holderType)
    {
        if (string.IsNullOrWhiteSpace(holderType))
            throw new ArgumentException(
                "Holder type is required.",
                nameof(holderType));

        var normalized = ValidHolderTypes.FirstOrDefault(
            x => string.Equals(
                x,
                holderType.Trim(),
                StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid holder type. Allowed values: {string.Join(", ", ValidHolderTypes)}.",
            nameof(holderType));
    }

    private static void ValidateEffectivePeriod(
        DateTime effectiveFrom,
        DateTime? effectiveTo)
    {
        if (effectiveFrom == default)
            throw new ArgumentException(
                "EffectiveFrom is required.",
                nameof(effectiveFrom));

        if (effectiveTo.HasValue &&
            effectiveTo.Value.Date < effectiveFrom.Date)
        {
            throw new ArgumentException(
                "EffectiveTo cannot precede EffectiveFrom.",
                nameof(effectiveTo));
        }
    }

    private static void ValidateDateRange(
        DateTime startDate,
        DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException(
                "Start date and end date are required.");

        if (endDate.Date < startDate.Date)
            throw new ArgumentException(
                "End date cannot precede start date.");
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "ID must be greater than zero.");
    }
}