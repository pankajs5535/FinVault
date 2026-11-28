
using AutoMapper;
using FinVault.Application.DTOs.InterestCalculationDtos;
using FinVault.Application.DTOs.InterestRateDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class InterestService : IInterestService
{
    private static readonly string[] ProductCategories =
    {
        "Savings", "Current", "FixedDeposit", "RecurringDeposit", "Loan"
    };

    private static readonly string[] CompoundingFrequencies =
    {
        "Simple", "Daily", "Monthly", "Quarterly", "HalfYearly", "Yearly"
    };

    private static readonly string[] CalculationTypes =
    {
        "Accrual", "Payout", "Capitalisation"
    };

    private static readonly string[] CalculationStatuses =
    {
        "Calculated", "Posted", "Reversed"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InterestService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // =========================================================
    // 1. INTEREST RATE IDENTIFICATION AND CRUD
    // =========================================================

    public async Task<IEnumerable<InterestRateDto>> GetAllRatesAsync()
    {
        var rates = await _unitOfWork.InterestRate.GetAllAsync();
        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<InterestRateDto?> GetRateByIdAsync(int interestRateId)
    {
        ValidateId(interestRateId, nameof(interestRateId));

        var rate = await _unitOfWork.InterestRate.GetByIdAsync(interestRateId);
        return _mapper.Map<InterestRateDto?>(rate);
    }

    public async Task<InterestRateDto?> GetRateDetailsAsync(int interestRateId)
    {
        ValidateId(interestRateId, nameof(interestRateId));

        var rate = await _unitOfWork.InterestRate
            .GetInterestRateDetailsAsync(interestRateId);

        return _mapper.Map<InterestRateDto?>(rate);
    }

    public async Task<IEnumerable<InterestRateDto>> GetRatesByNameAsync(string rateName)
    {
        ValidateRequiredString(rateName, nameof(rateName), 100);

        var rates = await _unitOfWork.InterestRate
            .GetByRateNameAsync(rateName.Trim());

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<InterestRateDto> CreateRateAsync(CreateInterestRateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateRate(
            dto.RateName,
            dto.ProductCategory,
            dto.AnnualRate,
            dto.SeniorCitizenAdditionalRate,
            dto.CompoundingFrequency,
            dto.MinAmount,
            dto.MaxAmount,
            dto.MinTenureMonths,
            dto.MaxTenureMonths,
            dto.EffectiveFrom,
            dto.EffectiveTo,
            dto.LoanType,
            dto.BenchmarkName,
            dto.SpreadOverBenchmark);

        if (dto.AccountTypeId.HasValue)
        {
            ValidateId(dto.AccountTypeId.Value, nameof(dto.AccountTypeId));

            if (await _unitOfWork.AccountType.GetByIdAsync(dto.AccountTypeId.Value) == null)
                throw new InvalidOperationException("Account type does not exist.");
        }

        if (dto.ProductCategory == "Loan" && dto.AccountTypeId.HasValue)
            throw new ArgumentException("Loan interest rates cannot reference an account type.");

        if (dto.ProductCategory != "Loan" && !string.IsNullOrWhiteSpace(dto.LoanType))
            throw new ArgumentException("LoanType is only applicable to loan interest rates.");

        if (dto.ProductCategory == "Loan" && string.IsNullOrWhiteSpace(dto.LoanType))
            throw new ArgumentException("LoanType is required for loan interest rates.");

        var rate = _mapper.Map<InterestRate>(dto);

        rate.RateName = dto.RateName.Trim();
        rate.ProductCategory = dto.ProductCategory.Trim();
        rate.LoanType = dto.LoanType?.Trim();
        rate.CompoundingFrequency = dto.CompoundingFrequency.Trim();
        rate.BenchmarkName = dto.BenchmarkName?.Trim();
        rate.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.InterestRate.AddAsync(rate);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<InterestRateDto>(rate);
    }

    public async Task<InterestRateDto?> UpdateRateAsync(UpdateInterestRateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.InterestRateId, nameof(dto.InterestRateId));

        ValidateRate(
            dto.RateName,
            dto.ProductCategory,
            dto.AnnualRate,
            dto.SeniorCitizenAdditionalRate,
            dto.CompoundingFrequency,
            dto.MinAmount,
            dto.MaxAmount,
            dto.MinTenureMonths,
            dto.MaxTenureMonths,
            dto.EffectiveFrom,
            dto.EffectiveTo,
            dto.LoanType,
            dto.BenchmarkName,
            dto.SpreadOverBenchmark);

        var rate = await _unitOfWork.InterestRate
            .GetByIdAsync(dto.InterestRateId);

        if (rate == null)
            return null;

        if (dto.AccountTypeId.HasValue)
        {
            ValidateId(dto.AccountTypeId.Value, nameof(dto.AccountTypeId));

            if (await _unitOfWork.AccountType.GetByIdAsync(dto.AccountTypeId.Value) == null)
                throw new InvalidOperationException("Account type does not exist.");
        }

        if (dto.ProductCategory == "Loan" && dto.AccountTypeId.HasValue)
            throw new ArgumentException("Loan interest rates cannot reference an account type.");

        if (dto.ProductCategory != "Loan" && !string.IsNullOrWhiteSpace(dto.LoanType))
            throw new ArgumentException("LoanType is only applicable to loan interest rates.");

        if (dto.ProductCategory == "Loan" && string.IsNullOrWhiteSpace(dto.LoanType))
            throw new ArgumentException("LoanType is required for loan interest rates.");

        _mapper.Map(dto, rate);

        rate.RateName = dto.RateName.Trim();
        rate.ProductCategory = dto.ProductCategory.Trim();
        rate.LoanType = dto.LoanType?.Trim();
        rate.CompoundingFrequency = dto.CompoundingFrequency.Trim();
        rate.BenchmarkName = dto.BenchmarkName?.Trim();
        rate.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.InterestRate.Update(rate);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<InterestRateDto>(rate);
    }

    public async Task<bool> DeleteRateAsync(DeleteInterestRateDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.InterestRateId, nameof(dto.InterestRateId));

        var rate = await _unitOfWork.InterestRate
            .GetInterestRateDetailsAsync(dto.InterestRateId);

        if (rate == null)
            return false;

        if (rate.Loans.Any() || rate.InterestCalculations.Any())
            throw new InvalidOperationException(
                "Interest rates referenced by loans or calculations cannot be deleted. Deactivate the rate instead.");

        _unitOfWork.InterestRate.Delete(rate);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // =========================================================
    // 2. PRODUCT CATEGORY AND TYPE
    // =========================================================

    public async Task<IEnumerable<InterestRateDto>> GetRatesByProductCategoryAsync(string productCategory)
    {
        ValidateProductCategory(productCategory);

        var rates = await _unitOfWork.InterestRate
            .GetByProductCategoryAsync(productCategory.Trim());

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetRatesByLoanTypeAsync(string loanType)
    {
        ValidateRequiredString(loanType, nameof(loanType), 20);

        var rates = await _unitOfWork.InterestRate
            .GetByLoanTypeAsync(loanType.Trim());

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetRatesByAccountTypeIdAsync(int accountTypeId)
    {
        ValidateId(accountTypeId, nameof(accountTypeId));

        var rates = await _unitOfWork.InterestRate
            .GetByAccountTypeIdAsync(accountTypeId);

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetRatesByCompoundingFrequencyAsync(string compoundingFrequency)
    {
        ValidateCompoundingFrequency(compoundingFrequency);

        var rates = await _unitOfWork.InterestRate
            .GetByCompoundingFrequencyAsync(compoundingFrequency.Trim());

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    // =========================================================
    // 3. RATE EFFECTIVE DATE MANAGEMENT
    // =========================================================

    public async Task<IEnumerable<InterestRateDto>> GetEffectiveRatesAsync(DateTime effectiveDate)
    {
        ValidateDate(effectiveDate, nameof(effectiveDate));

        var rates = await _unitOfWork.InterestRate
            .GetEffectiveRatesAsync(effectiveDate);

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetUpcomingRatesAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var rates = await _unitOfWork.InterestRate
            .GetUpcomingRatesAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetActiveRatesAsync()
    {
        var rates = await _unitOfWork.InterestRate.GetActiveRatesAsync();
        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetInactiveRatesAsync()
    {
        var rates = await _unitOfWork.InterestRate.GetInactiveRatesAsync();
        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetExpiredRatesAsync(DateTime currentDate)
    {
        ValidateDate(currentDate, nameof(currentDate));

        var rates = await _unitOfWork.InterestRate
            .GetExpiredRatesAsync(currentDate);

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    // =========================================================
    // 4. AMOUNT AND TENURE APPLICABILITY
    // =========================================================

    public async Task<IEnumerable<InterestRateDto>> GetRatesByAmountAsync(decimal amount, DateTime effectiveDate)
    {
        ValidateAmount(amount, nameof(amount));
        ValidateDate(effectiveDate, nameof(effectiveDate));

        var rates = await _unitOfWork.InterestRate
            .GetRatesByAmountAsync(amount, effectiveDate);

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetRatesByTenureAsync(int tenureMonths, DateTime effectiveDate)
    {
        if (tenureMonths <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenureMonths));

        ValidateDate(effectiveDate, nameof(effectiveDate));

        var rates = await _unitOfWork.InterestRate
            .GetRatesByTenureAsync(tenureMonths, effectiveDate);

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetApplicableRatesAsync(
        string productCategory,
        decimal amount,
        int tenureMonths,
        DateTime effectiveDate)
    {
        ValidateProductCategory(productCategory);
        ValidateAmount(amount, nameof(amount));

        if (tenureMonths <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenureMonths));

        ValidateDate(effectiveDate, nameof(effectiveDate));

        var rates = await _unitOfWork.InterestRate
            .GetApplicableRatesAsync(
                productCategory.Trim(),
                amount,
                tenureMonths,
                effectiveDate);

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    // =========================================================
    // 5. SPECIAL RATE CONFIGURATIONS
    // =========================================================

    public async Task<IEnumerable<InterestRateDto>> GetSeniorCitizenRatesAsync(DateTime effectiveDate)
    {
        ValidateDate(effectiveDate, nameof(effectiveDate));

        var rates = await _unitOfWork.InterestRate
            .GetSeniorCitizenRatesAsync(effectiveDate);

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetBenchmarkLinkedRatesAsync()
    {
        var rates = await _unitOfWork.InterestRate.GetBenchmarkLinkedRatesAsync();
        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetRatesWithLoansAsync()
    {
        var rates = await _unitOfWork.InterestRate.GetRatesWithLoansAsync();
        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetRatesWithInterestCalculationsAsync()
    {
        var rates = await _unitOfWork.InterestRate
            .GetRatesWithInterestCalculationsAsync();

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> GetRatesCreatedByUserAsync(int userId)
    {
        ValidateId(userId, nameof(userId));

        var rates = await _unitOfWork.InterestRate
            .GetRatesCreatedByUserAsync(userId);

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    public async Task<IEnumerable<InterestRateDto>> SearchInterestRatesAsync(string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm), 200);

        var rates = await _unitOfWork.InterestRate
            .SearchInterestRatesAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<InterestRateDto>>(rates);
    }

    // =========================================================
    // 6. INTEREST CALCULATION IDENTIFICATION
    // =========================================================

    public async Task<IEnumerable<InterestCalculationDto>> GetAllCalculationsAsync()
    {
        var calculations = await _unitOfWork.InterestCalculation.GetAllAsync();
        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<InterestCalculationDto?> GetCalculationByIdAsync(int interestCalculationId)
    {
        ValidateId(interestCalculationId, nameof(interestCalculationId));

        var calculation = await _unitOfWork.InterestCalculation
            .GetByIdAsync(interestCalculationId);

        return _mapper.Map<InterestCalculationDto?>(calculation);
    }

    public async Task<InterestCalculationDto?> GetCalculationDetailsAsync(int interestCalculationId)
    {
        ValidateId(interestCalculationId, nameof(interestCalculationId));

        var calculation = await _unitOfWork.InterestCalculation
            .GetCalculationDetailsAsync(interestCalculationId);

        return _mapper.Map<InterestCalculationDto?>(calculation);
    }

    public async Task<InterestCalculationDto> CreateCalculationAsync(CreateInterestCalculationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateCalculation(
            dto.CalculationType,
            dto.PeriodStart,
            dto.PeriodEnd,
            dto.DayCount,
            dto.PrincipalBase,
            dto.AppliedRate,
            dto.InterestAmount,
            dto.TdsAmount);

        ValidateId(dto.InterestRateId, nameof(dto.InterestRateId));

        if (dto.Status != "Calculated")
            throw new InvalidOperationException(
                "New interest calculations must have Calculated status. Posting must be performed through an accounting workflow.");

        if (dto.JournalEntryId.HasValue || dto.TransactionId.HasValue)
            throw new ArgumentException(
                "Journal entry and transaction references cannot be supplied during calculation creation.");

        ValidateCalculationSubject(dto.AccountId, dto.LoanId);

        if (dto.AccountId.HasValue &&
            await _unitOfWork.Account.GetByIdAsync(dto.AccountId.Value) == null)
            throw new InvalidOperationException("Account does not exist.");

        if (dto.LoanId.HasValue &&
            await _unitOfWork.Loan.GetByIdAsync(dto.LoanId.Value) == null)
            throw new InvalidOperationException("Loan does not exist.");

        var rate = await _unitOfWork.InterestRate
            .GetByIdAsync(dto.InterestRateId);

        if (rate == null)
            throw new InvalidOperationException("Interest rate does not exist.");

        if (!rate.IsActive ||
            rate.EffectiveFrom > dto.PeriodStart ||
            (rate.EffectiveTo.HasValue && rate.EffectiveTo.Value < dto.PeriodEnd))
        {
            throw new InvalidOperationException(
                "The selected interest rate is not active for the complete calculation period.");
        }

        if (dto.AccountId.HasValue && rate.ProductCategory == "Loan")
            throw new InvalidOperationException(
                "A loan interest rate cannot be used for an account calculation.");

        if (dto.LoanId.HasValue && rate.ProductCategory != "Loan")
            throw new InvalidOperationException(
                "A non-loan interest rate cannot be used for a loan calculation.");

        if (await _unitOfWork.InterestCalculation.ExistsForPeriodAsync(
            dto.AccountId,
            dto.LoanId,
            dto.PeriodStart,
            dto.PeriodEnd,
            dto.CalculationType.Trim()))
        {
            throw new InvalidOperationException(
                "An interest calculation already exists for this subject, period and calculation type.");
        }

        var calculation = _mapper.Map<InterestCalculation>(dto);

        calculation.CalculationType = dto.CalculationType.Trim();
        calculation.Status = "Calculated";
        calculation.CalculatedAt = dto.CalculatedAt == default
            ? DateTime.UtcNow
            : dto.CalculatedAt;

        // Preserve the applied rate and principal base supplied for this calculation.
        calculation.CreatedAt = DateTime.UtcNow;
        calculation.JournalEntryId = null;
        calculation.TransactionId = null;

        await _unitOfWork.InterestCalculation.AddAsync(calculation);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<InterestCalculationDto>(calculation);
    }

    public async Task<InterestCalculationDto?> UpdateCalculationAsync(UpdateInterestCalculationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.InterestCalculationId, nameof(dto.InterestCalculationId));

        ValidateCalculation(
            dto.CalculationType,
            dto.PeriodStart,
            dto.PeriodEnd,
            dto.DayCount,
            dto.PrincipalBase,
            dto.AppliedRate,
            dto.InterestAmount,
            dto.TdsAmount);

        ValidateId(dto.InterestRateId, nameof(dto.InterestRateId));
        ValidateCalculationSubject(dto.AccountId, dto.LoanId);

        var calculation = await _unitOfWork.InterestCalculation
            .GetByIdAsync(dto.InterestCalculationId);

        if (calculation == null)
            return null;

        if (calculation.Status != "Calculated")
            throw new InvalidOperationException(
                "Posted or reversed interest calculations cannot be modified. Use an accounting reversal process.");

        if (dto.Status != "Calculated")
            throw new InvalidOperationException(
                "Interest calculation status can only be changed through an accounting posting or reversal workflow.");

        if (dto.JournalEntryId.HasValue || dto.TransactionId.HasValue)
            throw new ArgumentException(
                "Journal entry and transaction references cannot be assigned through this update operation.");

        if (dto.AccountId.HasValue &&
            await _unitOfWork.Account.GetByIdAsync(dto.AccountId.Value) == null)
            throw new InvalidOperationException("Account does not exist.");

        if (dto.LoanId.HasValue &&
            await _unitOfWork.Loan.GetByIdAsync(dto.LoanId.Value) == null)
            throw new InvalidOperationException("Loan does not exist.");

        var rate = await _unitOfWork.InterestRate
            .GetByIdAsync(dto.InterestRateId);

        if (rate == null)
            throw new InvalidOperationException("Interest rate does not exist.");

        if (!rate.IsActive ||
            rate.EffectiveFrom > dto.PeriodStart ||
            (rate.EffectiveTo.HasValue && rate.EffectiveTo.Value < dto.PeriodEnd))
        {
            throw new InvalidOperationException(
                "The selected interest rate is not active for the complete calculation period.");
        }

        if (dto.AccountId.HasValue && rate.ProductCategory == "Loan")
            throw new InvalidOperationException("Invalid rate category for account calculation.");

        if (dto.LoanId.HasValue && rate.ProductCategory != "Loan")
            throw new InvalidOperationException("Invalid rate category for loan calculation.");

        if (await _unitOfWork.InterestCalculation.ExistsForPeriodAsync(
            dto.AccountId,
            dto.LoanId,
            dto.PeriodStart,
            dto.PeriodEnd,
            dto.CalculationType.Trim()))
        {
            var sameCalculation =
                calculation.AccountId == dto.AccountId &&
                calculation.LoanId == dto.LoanId &&
                calculation.PeriodStart == dto.PeriodStart &&
                calculation.PeriodEnd == dto.PeriodEnd &&
                calculation.CalculationType == dto.CalculationType;

            if (!sameCalculation)
                throw new InvalidOperationException(
                    "Another interest calculation already exists for the supplied subject, period and calculation type.");
        }

        _mapper.Map(dto, calculation);

        calculation.CalculationType = dto.CalculationType.Trim();
        calculation.Status = "Calculated";
        calculation.UpdatedAt = DateTime.UtcNow;
        calculation.JournalEntryId = null;
        calculation.TransactionId = null;

        _unitOfWork.InterestCalculation.Update(calculation);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<InterestCalculationDto>(calculation);
    }

    public async Task<bool> DeleteCalculationAsync(DeleteInterestCalculationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.InterestCalculationId, nameof(dto.InterestCalculationId));

        var calculation = await _unitOfWork.InterestCalculation
            .GetCalculationDetailsAsync(dto.InterestCalculationId);

        if (calculation == null)
            return false;

        if (calculation.Status != "Calculated")
            throw new InvalidOperationException(
                "Only unposted calculated interest records can be deleted.");

        if (calculation.JournalEntryId.HasValue ||
            calculation.TransactionId.HasValue)
        {
            throw new InvalidOperationException(
                "Interest calculations associated with accounting records cannot be deleted.");
        }

        _unitOfWork.InterestCalculation.Delete(calculation);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // =========================================================
    // 7. ACCOUNT, LOAN AND RATE CALCULATIONS
    // =========================================================

    public async Task<IEnumerable<InterestCalculationDto>> GetCalculationsByAccountIdAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var calculations = await _unitOfWork.InterestCalculation
            .GetByAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetCalculationsByLoanIdAsync(int loanId)
    {
        ValidateId(loanId, nameof(loanId));

        var calculations = await _unitOfWork.InterestCalculation
            .GetByLoanIdAsync(loanId);

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetCalculationsByInterestRateIdAsync(int interestRateId)
    {
        ValidateId(interestRateId, nameof(interestRateId));

        var calculations = await _unitOfWork.InterestCalculation
            .GetByInterestRateIdAsync(interestRateId);

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetLatestCalculationsByAccountIdAsync(int accountId, int count)
    {
        ValidateId(accountId, nameof(accountId));
        ValidateCount(count);

        var calculations = await _unitOfWork.InterestCalculation
            .GetLatestCalculationsByAccountIdAsync(accountId, count);

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetLatestCalculationsByLoanIdAsync(int loanId, int count)
    {
        ValidateId(loanId, nameof(loanId));
        ValidateCount(count);

        var calculations = await _unitOfWork.InterestCalculation
            .GetLatestCalculationsByLoanIdAsync(loanId, count);

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    // =========================================================
    // 8. CALCULATION TYPE AND STATUS
    // =========================================================

    public async Task<IEnumerable<InterestCalculationDto>> GetCalculationsByTypeAsync(string calculationType)
    {
        ValidateCalculationType(calculationType);

        var calculations = await _unitOfWork.InterestCalculation
            .GetByCalculationTypeAsync(calculationType.Trim());

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetCalculationsByStatusAsync(string status)
    {
        ValidateCalculationStatus(status);

        var calculations = await _unitOfWork.InterestCalculation
            .GetByStatusAsync(status.Trim());

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetPendingCalculationsAsync()
    {
        var calculations = await _unitOfWork.InterestCalculation
            .GetPendingCalculationsAsync();

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetPostedCalculationsAsync()
    {
        var calculations = await _unitOfWork.InterestCalculation
            .GetPostedCalculationsAsync();

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetReversedCalculationsAsync()
    {
        var calculations = await _unitOfWork.InterestCalculation
            .GetReversedCalculationsAsync();

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    // =========================================================
    // 9. ACCOUNTING POSTING AND PERIOD MANAGEMENT
    // =========================================================

    public async Task<InterestCalculationDto?> GetCalculationByJournalEntryIdAsync(int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        var calculation = await _unitOfWork.InterestCalculation
            .GetByJournalEntryIdAsync(journalEntryId);

        return _mapper.Map<InterestCalculationDto?>(calculation);
    }

    public async Task<InterestCalculationDto?> GetCalculationByTransactionIdAsync(int transactionId)
    {
        ValidateId(transactionId, nameof(transactionId));

        var calculation = await _unitOfWork.InterestCalculation
            .GetByTransactionIdAsync(transactionId);

        return _mapper.Map<InterestCalculationDto?>(calculation);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetPostedCalculationsWithoutJournalEntryAsync()
    {
        var calculations = await _unitOfWork.InterestCalculation
            .GetPostedCalculationsWithoutJournalEntryAsync();

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetCalculationsByPeriodRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var calculations = await _unitOfWork.InterestCalculation
            .GetByPeriodRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetCalculationsWithinDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var calculations = await _unitOfWork.InterestCalculation
            .GetCalculatedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    // =========================================================
    // 10. AMOUNT, TAX AND REPORTING
    // =========================================================

    public async Task<IEnumerable<InterestCalculationDto>> GetCalculationsByInterestAmountRangeAsync(decimal minimumAmount, decimal maximumAmount)
    {
        if (minimumAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumAmount));

        if (maximumAmount < minimumAmount)
            throw new ArgumentException(
                "Maximum interest amount cannot be less than minimum amount.");

        var calculations = await _unitOfWork.InterestCalculation
            .GetByInterestAmountRangeAsync(minimumAmount, maximumAmount);

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetCalculationsWithTdsAsync()
    {
        var calculations = await _unitOfWork.InterestCalculation
            .GetCalculationsWithTdsAsync();

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    public async Task<decimal> GetTotalInterestByAccountIdAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        return await _unitOfWork.InterestCalculation
            .GetTotalInterestByAccountIdAsync(accountId);
    }

    public async Task<decimal> GetTotalInterestByLoanIdAsync(int loanId)
    {
        ValidateId(loanId, nameof(loanId));

        return await _unitOfWork.InterestCalculation
            .GetTotalInterestByLoanIdAsync(loanId);
    }

    public async Task<IEnumerable<InterestCalculationDto>> GetCalculationsCreatedByUserIdAsync(int userId)
    {
        ValidateId(userId, nameof(userId));

        var calculations = await _unitOfWork.InterestCalculation
            .GetByCreatedByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<InterestCalculationDto>>(calculations);
    }

    // =========================================================
    // 11. VALIDATION
    // =========================================================

    public async Task<bool> ExistsForPeriodAsync(
        int? accountId,
        int? loanId,
        DateTime periodStart,
        DateTime periodEnd,
        string calculationType)
    {
        ValidateCalculationSubject(accountId, loanId);
        ValidateDateRange(periodStart, periodEnd);
        ValidateCalculationType(calculationType);

        return await _unitOfWork.InterestCalculation
            .ExistsForPeriodAsync(
                accountId,
                loanId,
                periodStart,
                periodEnd,
                calculationType.Trim());
    }

    // =========================================================
    // PRIVATE VALIDATION HELPERS
    // =========================================================

    private static void ValidateRate(
        string rateName,
        string productCategory,
        decimal annualRate,
        decimal? seniorCitizenAdditionalRate,
        string compoundingFrequency,
        decimal? minAmount,
        decimal? maxAmount,
        int? minTenureMonths,
        int? maxTenureMonths,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        string? loanType,
        string? benchmarkName,
        decimal? spreadOverBenchmark)
    {
        ValidateRequiredString(rateName, nameof(rateName), 100);
        ValidateProductCategory(productCategory);
        ValidateCompoundingFrequency(compoundingFrequency);

        if (annualRate < 0)
            throw new ArgumentOutOfRangeException(nameof(annualRate));

        if (seniorCitizenAdditionalRate < 0)
            throw new ArgumentOutOfRangeException(nameof(seniorCitizenAdditionalRate));

        if (minAmount < 0 || maxAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(minAmount));

        if (minAmount.HasValue && maxAmount.HasValue &&
            maxAmount.Value < minAmount.Value)
            throw new ArgumentException(
                "Maximum amount cannot be less than minimum amount.");

        if (minTenureMonths < 0 || maxTenureMonths < 0)
            throw new ArgumentOutOfRangeException(nameof(minTenureMonths));

        if (minTenureMonths.HasValue && maxTenureMonths.HasValue &&
            maxTenureMonths.Value < minTenureMonths.Value)
            throw new ArgumentException(
                "Maximum tenure cannot be less than minimum tenure.");

        ValidateDate(effectiveFrom, nameof(effectiveFrom));

        if (effectiveTo.HasValue)
        {
            ValidateDate(effectiveTo.Value, nameof(effectiveTo));

            if (effectiveTo.Value < effectiveFrom)
                throw new ArgumentException(
                    "Effective end date cannot be earlier than effective start date.");
        }

        ValidateOptionalString(loanType, nameof(loanType), 20);
        ValidateOptionalString(benchmarkName, nameof(benchmarkName), 50);

        if (spreadOverBenchmark < 0)
            throw new ArgumentOutOfRangeException(nameof(spreadOverBenchmark));
    }

    private static void ValidateCalculation(
        string calculationType,
        DateTime periodStart,
        DateTime periodEnd,
        int dayCount,
        decimal principalBase,
        decimal appliedRate,
        decimal interestAmount,
        decimal tdsAmount)
    {
        ValidateCalculationType(calculationType);
        ValidateDateRange(periodStart, periodEnd);

        if (dayCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(dayCount));

        if (principalBase <= 0)
            throw new ArgumentOutOfRangeException(nameof(principalBase));

        if (appliedRate < 0)
            throw new ArgumentOutOfRangeException(nameof(appliedRate));

        if (interestAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(interestAmount));

        if (tdsAmount < 0 || tdsAmount > interestAmount)
            throw new ArgumentOutOfRangeException(
                nameof(tdsAmount),
                "TDS must be between zero and the interest amount.");
    }

    private static void ValidateCalculationSubject(int? accountId, int? loanId)
    {
        if (accountId.HasValue == loanId.HasValue)
            throw new ArgumentException(
                "An interest calculation must belong to exactly one account or one loan.");

        if (accountId.HasValue)
            ValidateId(accountId.Value, nameof(accountId));

        if (loanId.HasValue)
            ValidateId(loanId.Value, nameof(loanId));
    }

    private static void ValidateProductCategory(string value)
    {
        ValidateRequiredString(value, nameof(value), 20);

        if (!ProductCategories.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException(
                "Invalid interest product category.", nameof(value));
    }

    private static void ValidateCompoundingFrequency(string value)
    {
        ValidateRequiredString(value, nameof(value), 15);

        if (!CompoundingFrequencies.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException(
                "Invalid compounding frequency.", nameof(value));
    }

    private static void ValidateCalculationType(string value)
    {
        ValidateRequiredString(value, nameof(value), 20);

        if (!CalculationTypes.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException(
                "Invalid calculation type.", nameof(value));
    }

    private static void ValidateCalculationStatus(string value)
    {
        ValidateRequiredString(value, nameof(value), 20);

        if (!CalculationStatuses.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException(
                "Invalid calculation status.", nameof(value));
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName, "ID must be greater than zero.");
    }

    private static void ValidateCount(int count)
    {
        if (count <= 0 || count > 500)
            throw new ArgumentOutOfRangeException(
                nameof(count), "Count must be between 1 and 500.");
    }

    private static void ValidateAmount(decimal amount, string parameterName)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(
                parameterName, "Amount cannot be negative.");
    }

    private static void ValidateRequiredString(
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

    private static void ValidateOptionalString(
        string? value,
        string parameterName,
        int maximumLength)
    {
        if (value?.Length > maximumLength)
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength} characters.",
                parameterName);
    }

    private static void ValidateDate(DateTime date, string parameterName)
    {
        if (date == default)
            throw new ArgumentException(
                "A valid date is required.", parameterName);
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        ValidateDate(startDate, nameof(startDate));
        ValidateDate(endDate, nameof(endDate));

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }
}
