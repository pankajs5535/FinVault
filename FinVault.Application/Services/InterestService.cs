using AutoMapper;
using FinVault.Application.DTOs.InterestRateDtos;
using FinVault.Application.DTOs.InterestCalculationDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Manages rate configurations and calculates simple daily-accrual interest for eligible deposit accounts.</summary>
public sealed class InterestService : IInterestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InterestService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieves active interest rates, optionally limited to one account type.
    public async Task<IReadOnlyList<InterestRateDto>> GetRatesAsync(int? accountTypeId)
    {
        var rates = accountTypeId.HasValue
            ? await _unitOfWork.InterestRate.GetByAccountTypeIdAsync(accountTypeId.Value)
            : await _unitOfWork.InterestRate.GetActiveRatesAsync();
        return _mapper.Map<IReadOnlyList<InterestRateDto>>(rates);
    }

    // 2. Creates an interest-rate configuration after validating its rate and effective period.
    public async Task<InterestRateDto> CreateRateAsync(CreateInterestRateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.AnnualRate < 0 || dto.AnnualRate > 100) throw new ArgumentOutOfRangeException(nameof(dto.AnnualRate), "Annual rate must be between 0 and 100 percent.");
        if (dto.EffectiveTo.HasValue && dto.EffectiveTo.Value < dto.EffectiveFrom) throw new ArgumentException("Effective-to date cannot precede effective-from date.");
        if (dto.MinAmount.HasValue && dto.MaxAmount.HasValue && dto.MinAmount > dto.MaxAmount) throw new ArgumentException("Minimum amount cannot exceed maximum amount.");
        if (dto.MinTenureMonths.HasValue && dto.MaxTenureMonths.HasValue && dto.MinTenureMonths > dto.MaxTenureMonths) throw new ArgumentException("Minimum tenure cannot exceed maximum tenure.");

        var rate = _mapper.Map<InterestRate>(dto);
        rate.RateName = dto.RateName.Trim();
        rate.ProductCategory = dto.ProductCategory.Trim();
        rate.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.InterestRate.AddAsync(rate);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<InterestRateDto>(rate);
    }

    // 3. Updates an existing rate configuration after validating its date and percentage values.
    public async Task<InterestRateDto?> UpdateRateAsync(int interestRateId, UpdateInterestRateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (interestRateId <= 0 || dto.InterestRateId != interestRateId) throw new ArgumentException("Route rate ID and DTO rate ID must match.");
        if (dto.AnnualRate < 0 || dto.AnnualRate > 100) throw new ArgumentOutOfRangeException(nameof(dto.AnnualRate));
        if (dto.EffectiveTo.HasValue && dto.EffectiveTo.Value < dto.EffectiveFrom) throw new ArgumentException("Effective-to date cannot precede effective-from date.");
        var rate = await _unitOfWork.InterestRate.GetByIdAsync(interestRateId);
        if (rate is null) return null;

        _mapper.Map(dto, rate);
        rate.RateName = dto.RateName.Trim();
        rate.ProductCategory = dto.ProductCategory.Trim();
        rate.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.InterestRate.Update(rate);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<InterestRateDto>(rate);
    }

    // 4. Calculates simple daily-accrual interest using the account balance and the effective configured annual rate.
    public async Task<InterestCalculationDto> CalculateAsync(int accountId, DateTime asOfDate)
    {
        if (accountId <= 0) throw new ArgumentOutOfRangeException(nameof(accountId));
        if (asOfDate == default || asOfDate.Date > DateTime.UtcNow.Date) throw new ArgumentException("A valid as-of date no later than today is required.");
        var account = await _unitOfWork.Account.GetAccountDetailsAsync(accountId);
        if (account is null || account.Status != "Active") throw new InvalidOperationException("An active account is required for interest calculation.");
        var rates = await _unitOfWork.InterestRate.GetEffectiveRatesAsync(asOfDate);
        var rate = rates.FirstOrDefault(x => x.AccountTypeId == account.AccountTypeId && x.IsActive);
        if (rate is null) throw new InvalidOperationException("No active interest rate is configured for this account type and date.");

        var periodStart = account.LastTransactionAt?.Date ?? account.OpenedOn.Date;
        if (periodStart > asOfDate.Date) throw new InvalidOperationException("Interest period start cannot be after the calculation date.");
        var periodEnd = asOfDate.Date;
        var dayCount = Math.Max(1, (periodEnd - periodStart).Days);
        if (await _unitOfWork.InterestCalculation.ExistsForPeriodAsync(accountId, null, periodStart, periodEnd, "Accrual"))
            throw new InvalidOperationException("An interest calculation already exists for this account and period.");

        var principal = account.Balance;
        var interest = Math.Round(principal * (rate.AnnualRate / 100m) * dayCount / 365m, 2, MidpointRounding.AwayFromZero);
        var calculation = new InterestCalculation
        {
            CalculationType = "Accrual",
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            DayCount = dayCount,
            PrincipalBase = principal,
            AppliedRate = rate.AnnualRate,
            InterestAmount = interest,
            TdsAmount = 0,
            Status = "Calculated",
            CalculatedAt = DateTime.UtcNow,
            AccountId = accountId,
            InterestRateId = rate.InterestRateId,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.InterestCalculation.AddAsync(calculation);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<InterestCalculationDto>(calculation);
    }

    // 5. Marks interest as posted only after its linked journal entry has itself been posted.
    public async Task<bool> PostAccruedInterestAsync(int interestCalculationId)
    {
        if (interestCalculationId <= 0) throw new ArgumentOutOfRangeException(nameof(interestCalculationId));
        var calculation = await _unitOfWork.InterestCalculation.GetCalculationDetailsAsync(interestCalculationId);
        if (calculation is null) return false;
        if (calculation.Status != "Calculated") throw new InvalidOperationException("Only calculated interest can be posted.");
        if (!calculation.JournalEntryId.HasValue) throw new InvalidOperationException("Create and link a balanced journal entry before posting interest.");
        var journal = await _unitOfWork.JournalEntry.GetJournalEntryDetailsAsync(calculation.JournalEntryId.Value);
        if (journal is null || journal.Status != "Posted") throw new InvalidOperationException("The linked journal entry must be posted first.");
        calculation.Status = "Posted";
        calculation.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.InterestCalculation.Update(calculation);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
