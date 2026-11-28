using AutoMapper;
using FinVault.Application.DTOs.LoanDtos;
using FinVault.Application.DTOs.LoanScheduleDtos;
using FinVault.Application.DTOs.LoanRepaymentDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class LoanService : ILoanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LoanService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    // LOAN IDENTIFICATION AND CRUD

    public async Task<IEnumerable<LoanDto>> GetAllAsync()
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetAllAsync());

    public async Task<LoanDto?> GetByIdAsync(int loanId)
        => await MapOneAsync<Loan, LoanDto>(_unitOfWork.Loan.GetByIdAsync(loanId));

    public async Task<LoanDto?> GetByLoanNumberAsync(string loanNumber)
        => await MapOneAsync<Loan, LoanDto>(_unitOfWork.Loan.GetByLoanNumberAsync(loanNumber));

    public async Task<LoanDto?> GetLoanDetailsAsync(int loanId)
        => await MapOneAsync<Loan, LoanDto>(_unitOfWork.Loan.GetLoanDetailsAsync(loanId));

    public async Task<bool> ExistsByLoanNumberAsync(string loanNumber)
        => await _unitOfWork.Loan.ExistsByLoanNumberAsync(loanNumber);

    public async Task<IEnumerable<LoanDto>> GetByCustomerIdAsync(int customerId)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetByCustomerIdAsync(customerId));

    public async Task<IEnumerable<LoanDto>> GetByBranchIdAsync(int branchId)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetByBranchIdAsync(branchId));

    public async Task<IEnumerable<LoanDto>> GetByLoanTypeAsync(string loanType)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetByLoanTypeAsync(loanType));

    public async Task<IEnumerable<LoanDto>> GetByDisbursementAccountIdAsync(int accountId)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetByDisbursementAccountIdAsync(accountId));

    public async Task<IEnumerable<LoanDto>> GetByRepaymentAccountIdAsync(int accountId)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetByRepaymentAccountIdAsync(accountId));

    public async Task<IEnumerable<LoanDto>> GetByStatusAsync(string status)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetByStatusAsync(status));

    public async Task<IEnumerable<LoanDto>> GetAppliedLoansAsync()
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetAppliedLoansAsync());

    public async Task<IEnumerable<LoanDto>> GetApprovedUndisbursedLoansAsync()
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetApprovedUndisbursedLoansAsync());

    public async Task<IEnumerable<LoanDto>> GetActiveLoansAsync()
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetActiveLoansAsync());

    public async Task<IEnumerable<LoanDto>> GetClosedLoansAsync()
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetClosedLoansAsync());

    public async Task<IEnumerable<LoanDto>> GetByApprovedByEmployeeIdAsync(int employeeId)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetByApprovedByEmployeeIdAsync(employeeId));

    public async Task<IEnumerable<LoanDto>> GetDisbursedWithinRangeAsync(DateTime startDate, DateTime endDate)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetDisbursedWithinRangeAsync(startDate, endDate));

    public async Task<IEnumerable<LoanDto>> GetApplicationsWithinRangeAsync(DateTime startDate, DateTime endDate)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetApplicationsWithinRangeAsync(startDate, endDate));

    public async Task<IEnumerable<LoanDto>> GetLoansWithOutstandingPrincipalAboveAsync(decimal minimumAmount)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetLoansWithOutstandingPrincipalAboveAsync(minimumAmount));

    public async Task<IEnumerable<LoanDto>> GetByAssetClassificationAsync(string assetClassification)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetByAssetClassificationAsync(assetClassification));

    public async Task<IEnumerable<LoanDto>> GetLoansPastDueAsync(int minimumDaysPastDue)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetLoansPastDueAsync(minimumDaysPastDue));

    public async Task<IEnumerable<LoanDto>> GetLoansMaturingWithinRangeAsync(DateTime startDate, DateTime endDate)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetLoansMaturingWithinRangeAsync(startDate, endDate));

    public async Task<IEnumerable<LoanDto>> GetLoansWithSchedulesAsync(int customerId)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetLoansWithSchedulesAsync(customerId));

    public async Task<IEnumerable<LoanDto>> GetLoansWithRepaymentsAsync(int customerId)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.GetLoansWithRepaymentsAsync(customerId));

    public async Task<LoanDto?> GetLoanWithInterestCalculationsAsync(int loanId)
        => await MapOneAsync<Loan, LoanDto>(_unitOfWork.Loan.GetLoanWithInterestCalculationsAsync(loanId));

    public async Task<decimal> GetTotalOutstandingPrincipalByBranchIdAsync(int branchId)
        => await _unitOfWork.Loan.GetTotalOutstandingPrincipalByBranchIdAsync(branchId);

    public async Task<decimal> GetTotalOutstandingPrincipalByCustomerIdAsync(int customerId)
        => await _unitOfWork.Loan.GetTotalOutstandingPrincipalByCustomerIdAsync(customerId);

    public async Task<IEnumerable<LoanDto>> SearchLoansAsync(string searchTerm)
        => await MapManyAsync<Loan, LoanDto>(_unitOfWork.Loan.SearchLoansAsync(searchTerm));

    // LOAN SCHEDULE IDENTIFICATION AND QUERIES

    public async Task<IEnumerable<LoanScheduleDto>> GetAllSchedulesAsync()
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetAllAsync());

    public async Task<LoanScheduleDto?> GetScheduleByIdAsync(int loanScheduleId)
        => await MapOneAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetByIdAsync(loanScheduleId));

    public async Task<LoanScheduleDto?> GetByLoanAndInstallmentAsync(int loanId, int installmentNumber)
        => await MapOneAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetByLoanAndInstallmentAsync(loanId, installmentNumber));

    public async Task<LoanScheduleDto?> GetScheduleDetailsAsync(int loanScheduleId)
        => await MapOneAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetScheduleDetailsAsync(loanScheduleId));

    public async Task<bool> ExistsByLoanAndInstallmentAsync(int loanId, int installmentNumber)
        => await _unitOfWork.LoanSchedule.ExistsByLoanAndInstallmentAsync(loanId, installmentNumber);

    public async Task<IEnumerable<LoanScheduleDto>> GetSchedulesByLoanIdAsync(int loanId)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetByLoanIdAsync(loanId));

    public async Task<IEnumerable<LoanScheduleDto>> GetLoanSchedulesWithRepaymentsAsync(int loanId)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetLoanSchedulesWithRepaymentsAsync(loanId));

    public async Task<LoanScheduleDto?> GetScheduleWithRepaymentsAsync(int loanScheduleId)
        => await MapOneAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetScheduleWithRepaymentsAsync(loanScheduleId));

    public async Task<IEnumerable<LoanScheduleDto>> GetSchedulesByStatusAsync(string status)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetByStatusAsync(status));

    public async Task<IEnumerable<LoanScheduleDto>> GetPendingSchedulesByLoanIdAsync(int loanId)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetPendingSchedulesByLoanIdAsync(loanId));

    public async Task<IEnumerable<LoanScheduleDto>> GetPartiallyPaidSchedulesByLoanIdAsync(int loanId)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetPartiallyPaidSchedulesByLoanIdAsync(loanId));

    public async Task<IEnumerable<LoanScheduleDto>> GetPaidSchedulesByLoanIdAsync(int loanId)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetPaidSchedulesByLoanIdAsync(loanId));

    public async Task<IEnumerable<LoanScheduleDto>> GetOverdueSchedulesByLoanIdAsync(int loanId)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetOverdueSchedulesByLoanIdAsync(loanId));

    public async Task<IEnumerable<LoanScheduleDto>> GetWaivedSchedulesByLoanIdAsync(int loanId)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetWaivedSchedulesByLoanIdAsync(loanId));

    public async Task<IEnumerable<LoanScheduleDto>> GetSchedulesDueWithinRangeAsync(DateTime startDate, DateTime endDate)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetSchedulesDueWithinRangeAsync(startDate, endDate));

    public async Task<IEnumerable<LoanScheduleDto>> GetUnpaidSchedulesDueBeforeAsync(DateTime date)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetUnpaidSchedulesDueBeforeAsync(date));

    public async Task<IEnumerable<LoanScheduleDto>> GetSchedulesDueOnDateAsync(int loanId, DateTime dueDate)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetSchedulesDueOnDateAsync(loanId, dueDate));

    public async Task<IEnumerable<LoanScheduleDto>> GetAllOverdueSchedulesAsync()
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetAllOverdueSchedulesAsync());

    public async Task<IEnumerable<LoanScheduleDto>> GetSchedulesPaidWithinRangeAsync(DateTime startDate, DateTime endDate)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetSchedulesPaidWithinRangeAsync(startDate, endDate));

    public async Task<IEnumerable<LoanScheduleDto>> GetSchedulesWithOutstandingAmountAboveAsync(decimal minimumAmount)
        => await MapManyAsync<LoanSchedule, LoanScheduleDto>(_unitOfWork.LoanSchedule.GetSchedulesWithOutstandingAmountAboveAsync(minimumAmount));

    public async Task<decimal> GetTotalPrincipalDueByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanSchedule.GetTotalPrincipalDueByLoanIdAsync(loanId);

    public async Task<decimal> GetTotalInterestDueByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanSchedule.GetTotalInterestDueByLoanIdAsync(loanId);

    public async Task<decimal> GetTotalPrincipalPaidByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanSchedule.GetTotalPrincipalPaidByLoanIdAsync(loanId);

    public async Task<decimal> GetTotalInterestPaidByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanSchedule.GetTotalInterestPaidByLoanIdAsync(loanId);

    public async Task<decimal> GetTotalPenaltyAccruedByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanSchedule.GetTotalPenaltyAccruedByLoanIdAsync(loanId);

    public async Task<decimal> GetTotalPenaltyPaidByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanSchedule.GetTotalPenaltyPaidByLoanIdAsync(loanId);

    public async Task<int> CountSchedulesByLoanAndStatusAsync(int loanId, string status)
        => await _unitOfWork.LoanSchedule.CountSchedulesByLoanAndStatusAsync(loanId, status);

    public async Task<int> CountUnpaidSchedulesByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanSchedule.CountUnpaidSchedulesByLoanIdAsync(loanId);

    // LOAN REPAYMENT IDENTIFICATION AND QUERIES

    public async Task<IEnumerable<LoanRepaymentDto>> GetAllRepaymentsAsync()
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetAllAsync());

    public async Task<LoanRepaymentDto?> GetRepaymentByIdAsync(int loanRepaymentId)
        => await MapOneAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetByIdAsync(loanRepaymentId));

    public async Task<LoanRepaymentDto?> GetByRepaymentReferenceAsync(string repaymentReference)
        => await MapOneAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetByRepaymentReferenceAsync(repaymentReference));

    public async Task<LoanRepaymentDto?> GetRepaymentDetailsAsync(int loanRepaymentId)
        => await MapOneAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetRepaymentDetailsAsync(loanRepaymentId));

    public async Task<bool> ExistsByRepaymentReferenceAsync(string repaymentReference)
        => await _unitOfWork.LoanRepayment.ExistsByRepaymentReferenceAsync(repaymentReference);

    public async Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByLoanIdAsync(int loanId)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetByLoanIdAsync(loanId));

    public async Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByLoanScheduleIdAsync(int loanScheduleId)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetByLoanScheduleIdAsync(loanScheduleId));

    public async Task<LoanRepaymentDto?> GetRepaymentByTransactionIdAsync(int transactionId)
        => await MapOneAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetByTransactionIdAsync(transactionId));

    public async Task<IEnumerable<LoanRepaymentDto>> GetLoanRepaymentsWithTransactionsAsync(int loanId)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetLoanRepaymentsWithTransactionsAsync(loanId));

    public async Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByStatusAsync(string status)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetByStatusAsync(status));

    public async Task<IEnumerable<LoanRepaymentDto>> GetSuccessfulRepaymentsByLoanIdAsync(int loanId)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetSuccessfulRepaymentsByLoanIdAsync(loanId));

    public async Task<IEnumerable<LoanRepaymentDto>> GetPendingRepaymentsAsync()
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetPendingRepaymentsAsync());

    public async Task<IEnumerable<LoanRepaymentDto>> GetFailedRepaymentsAsync()
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetFailedRepaymentsAsync());

    public async Task<IEnumerable<LoanRepaymentDto>> GetReversedRepaymentsAsync()
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetReversedRepaymentsAsync());

    public async Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByTypeAsync(string repaymentType)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetByRepaymentTypeAsync(repaymentType));

    public async Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByPaymentModeAsync(string paymentMode)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetByPaymentModeAsync(paymentMode));

    public async Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsWithinRangeAsync(DateTime startDate, DateTime endDate)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetRepaymentsWithinRangeAsync(startDate, endDate));

    public async Task<IEnumerable<LoanRepaymentDto>> GetSuccessfulRepaymentsWithinRangeAsync(DateTime startDate, DateTime endDate)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetSuccessfulRepaymentsWithinRangeAsync(startDate, endDate));

    public async Task<IEnumerable<LoanRepaymentDto>> GetRecentRepaymentsAsync(DateTime fromDate)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.GetRecentRepaymentsAsync(fromDate));

    public async Task<decimal> GetTotalSuccessfulAmountByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanRepayment.GetTotalSuccessfulAmountByLoanIdAsync(loanId);

    public async Task<decimal> GetTotalPrincipalCollectedByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanRepayment.GetTotalPrincipalCollectedByLoanIdAsync(loanId);

    public async Task<decimal> GetTotalInterestCollectedByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanRepayment.GetTotalInterestCollectedByLoanIdAsync(loanId);

    public async Task<decimal> GetTotalPenaltyCollectedByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanRepayment.GetTotalPenaltyCollectedByLoanIdAsync(loanId);

    public async Task<decimal> GetTotalSuccessfulCollectionsWithinRangeAsync(DateTime startDate, DateTime endDate)
        => await _unitOfWork.LoanRepayment.GetTotalSuccessfulCollectionsWithinRangeAsync(startDate, endDate);

    public async Task<IEnumerable<LoanRepaymentDto>> SearchRepaymentsAsync(string searchTerm)
        => await MapManyAsync<LoanRepayment, LoanRepaymentDto>(_unitOfWork.LoanRepayment.SearchRepaymentsAsync(searchTerm));

    public async Task<int> CountSuccessfulRepaymentsByLoanIdAsync(int loanId)
        => await _unitOfWork.LoanRepayment.CountSuccessfulRepaymentsByLoanIdAsync(loanId);

    // LOAN CRUD OPERATIONS

    public async Task<LoanDto> CreateAsync(CreateLoanDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.CustomerId, nameof(dto.CustomerId));
        ValidateId(dto.BranchId, nameof(dto.BranchId));
        ValidateLoanValues(dto.LoanNumber, dto.RequestedAmount, dto.AnnualInterestRate, dto.TenureMonths);

        var loanNumber = RequiredText(dto.LoanNumber, nameof(dto.LoanNumber)).ToUpperInvariant();

        if (await _unitOfWork.Loan.ExistsByLoanNumberAsync(loanNumber))
            throw new InvalidOperationException($"Loan number '{loanNumber}' already exists.");

        if (await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId) is null)
            throw new InvalidOperationException("The specified customer does not exist.");

        if (await _unitOfWork.Branch.GetByIdAsync(dto.BranchId) is null)
            throw new InvalidOperationException("The specified branch does not exist.");

        if (dto.DisbursementAccountId.HasValue &&
            await _unitOfWork.Account.GetByIdAsync(dto.DisbursementAccountId.Value) is null)
            throw new InvalidOperationException("The specified disbursement account does not exist.");

        if (dto.RepaymentAccountId.HasValue &&
            await _unitOfWork.Account.GetByIdAsync(dto.RepaymentAccountId.Value) is null)
            throw new InvalidOperationException("The specified repayment account does not exist.");

        if (dto.InterestRateId.HasValue &&
            await _unitOfWork.InterestRate.GetByIdAsync(dto.InterestRateId.Value) is null)
            throw new InvalidOperationException("The specified interest rate does not exist.");

        if (!string.Equals(dto.Status, "Applied", StringComparison.Ordinal))
            throw new InvalidOperationException("A new loan application must start with Applied status.");

        var loan = _mapper.Map<Loan>(dto);

        loan.LoanNumber = loanNumber;
        loan.Status = "Applied";
        loan.DisbursedAmount = 0m;
        loan.OutstandingPrincipal = 0m;
        loan.OutstandingInterest = 0m;
        loan.DaysPastDue = 0;
        loan.SanctionDate = null;
        loan.DisbursementDate = null;
        loan.ClosedOn = null;
        loan.ApplicationDate = dto.ApplicationDate == default
            ? DateTime.UtcNow
            : dto.ApplicationDate;
        loan.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Loan.AddAsync(loan);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LoanDto>(loan);
    }

    public async Task<LoanDto?> UpdateAsync(UpdateLoanDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.LoanId, nameof(dto.LoanId));
        ValidateId(dto.CustomerId, nameof(dto.CustomerId));
        ValidateId(dto.BranchId, nameof(dto.BranchId));
        ValidateLoanValues(dto.LoanNumber, dto.RequestedAmount, dto.AnnualInterestRate, dto.TenureMonths);

        var loan = await _unitOfWork.Loan.GetByIdAsync(dto.LoanId);

        if (loan is null)
            return null;

        EnsureRowVersion(loan.RowVersion, dto.RowVersion, "loan");

        var loanNumber = RequiredText(dto.LoanNumber, nameof(dto.LoanNumber)).ToUpperInvariant();

        var duplicate = await _unitOfWork.Loan.GetByLoanNumberAsync(loanNumber);

        if (duplicate is not null && duplicate.LoanId != dto.LoanId)
            throw new InvalidOperationException($"Loan number '{loanNumber}' already exists.");

        if (await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId) is null)
            throw new InvalidOperationException("The specified customer does not exist.");

        if (await _unitOfWork.Branch.GetByIdAsync(dto.BranchId) is null)
            throw new InvalidOperationException("The specified branch does not exist.");

        if (dto.DisbursementAccountId.HasValue &&
            await _unitOfWork.Account.GetByIdAsync(dto.DisbursementAccountId.Value) is null)
            throw new InvalidOperationException("The specified disbursement account does not exist.");

        if (dto.RepaymentAccountId.HasValue &&
            await _unitOfWork.Account.GetByIdAsync(dto.RepaymentAccountId.Value) is null)
            throw new InvalidOperationException("The specified repayment account does not exist.");

        if (dto.InterestRateId.HasValue &&
            await _unitOfWork.InterestRate.GetByIdAsync(dto.InterestRateId.Value) is null)
            throw new InvalidOperationException("The specified interest rate does not exist.");

        var originalStatus = loan.Status;
        var originalSanctionedAmount = loan.SanctionedAmount;
        var originalDisbursedAmount = loan.DisbursedAmount;
        var originalOutstandingPrincipal = loan.OutstandingPrincipal;
        var originalOutstandingInterest = loan.OutstandingInterest;
        var originalSanctionDate = loan.SanctionDate;
        var originalDisbursementDate = loan.DisbursementDate;
        var originalClosedOn = loan.ClosedOn;

        _mapper.Map(dto, loan);

        loan.LoanNumber = loanNumber;
        loan.Status = originalStatus;
        loan.SanctionedAmount = originalSanctionedAmount;
        loan.DisbursedAmount = originalDisbursedAmount;
        loan.OutstandingPrincipal = originalOutstandingPrincipal;
        loan.OutstandingInterest = originalOutstandingInterest;
        loan.SanctionDate = originalSanctionDate;
        loan.DisbursementDate = originalDisbursementDate;
        loan.ClosedOn = originalClosedOn;
        loan.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Loan.Update(loan);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LoanDto>(loan);
    }

    public async Task<bool> DeleteAsync(DeleteLoanDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.LoanId, nameof(dto.LoanId));

        var loan = await _unitOfWork.Loan.GetByIdAsync(dto.LoanId);

        if (loan is null)
            return false;

        if (loan.Status is not ("Applied" or "Rejected"))
            throw new InvalidOperationException("Only applied or rejected loan records can be deleted.");

        var schedules = await _unitOfWork.LoanSchedule.FindAsync(x => x.LoanId == dto.LoanId);
        var repayments = await _unitOfWork.LoanRepayment.FindAsync(x => x.LoanId == dto.LoanId);

        if (schedules.Any() || repayments.Any())
            throw new InvalidOperationException("The loan cannot be deleted because schedule or repayment history exists.");

        _unitOfWork.Loan.Delete(loan);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // LOAN SCHEDULE CRUD OPERATIONS

    public async Task<LoanScheduleDto> CreateScheduleAsync(CreateLoanScheduleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.LoanId, nameof(dto.LoanId));

        if (dto.InstallmentNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.InstallmentNumber));

        if (dto.DueDate == default)
            throw new ArgumentException("A valid due date is required.", nameof(dto.DueDate));

        ValidateNonNegative(
            dto.OpeningBalance,
            dto.ClosingBalance,
            dto.PrincipalDue,
            dto.InterestDue,
            dto.PenaltyAccrued,
            dto.PrincipalPaid,
            dto.InterestPaid,
            dto.PenaltyPaid);

        if (await _unitOfWork.Loan.GetByIdAsync(dto.LoanId) is null)
            throw new InvalidOperationException("The specified loan does not exist.");

        if (await _unitOfWork.LoanSchedule.ExistsByLoanAndInstallmentAsync(dto.LoanId, dto.InstallmentNumber))
            throw new InvalidOperationException("This installment number already exists for the loan.");

        var schedule = _mapper.Map<LoanSchedule>(dto);

        schedule.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.LoanSchedule.AddAsync(schedule);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LoanScheduleDto>(schedule);
    }

    public async Task<LoanScheduleDto?> UpdateScheduleAsync(UpdateLoanScheduleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.LoanScheduleId, nameof(dto.LoanScheduleId));
        ValidateId(dto.LoanId, nameof(dto.LoanId));

        var schedule = await _unitOfWork.LoanSchedule.GetByIdAsync(dto.LoanScheduleId);

        if (schedule is null)
            return null;

        EnsureRowVersion(schedule.RowVersion, dto.RowVersion, "loan schedule");

        if (await _unitOfWork.Loan.GetByIdAsync(dto.LoanId) is null)
            throw new InvalidOperationException("The specified loan does not exist.");

        if (dto.InstallmentNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.InstallmentNumber));

        ValidateNonNegative(
            dto.OpeningBalance,
            dto.ClosingBalance,
            dto.PrincipalDue,
            dto.InterestDue,
            dto.PenaltyAccrued,
            dto.PrincipalPaid,
            dto.InterestPaid,
            dto.PenaltyPaid);

        var duplicate = await _unitOfWork.LoanSchedule.GetByLoanAndInstallmentAsync(dto.LoanId, dto.InstallmentNumber);

        if (duplicate is not null && duplicate.LoanScheduleId != dto.LoanScheduleId)
            throw new InvalidOperationException("This installment number already exists for the loan.");

        var repayments = await _unitOfWork.LoanRepayment.GetByLoanScheduleIdAsync(dto.LoanScheduleId);

        if (repayments.Any(x => x.Status == "Successful"))
            throw new InvalidOperationException("A schedule with successful repayments cannot be edited through the general update operation.");

        _mapper.Map(dto, schedule);

        schedule.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.LoanSchedule.Update(schedule);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LoanScheduleDto>(schedule);
    }

    public async Task<bool> DeleteScheduleAsync(DeleteLoanScheduleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.LoanScheduleId, nameof(dto.LoanScheduleId));

        var schedule = await _unitOfWork.LoanSchedule.GetByIdAsync(dto.LoanScheduleId);

        if (schedule is null)
            return false;

        var repayments = await _unitOfWork.LoanRepayment.GetByLoanScheduleIdAsync(dto.LoanScheduleId);

        if (repayments.Any())
            throw new InvalidOperationException("A schedule with repayment history cannot be deleted.");

        _unitOfWork.LoanSchedule.Delete(schedule);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // LOAN REPAYMENT CRUD OPERATIONS

    public async Task<LoanRepaymentDto> CreateRepaymentAsync(CreateLoanRepaymentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var reference = RequiredText(dto.RepaymentReference, nameof(dto.RepaymentReference)).ToUpperInvariant();

        if (await _unitOfWork.LoanRepayment.ExistsByRepaymentReferenceAsync(reference))
            throw new InvalidOperationException($"Repayment reference '{reference}' already exists.");

        ValidateId(dto.LoanId, nameof(dto.LoanId));

        if (await _unitOfWork.Loan.GetByIdAsync(dto.LoanId) is null)
            throw new InvalidOperationException("The specified loan does not exist.");

        if (dto.LoanScheduleId.HasValue)
        {
            ValidateId(dto.LoanScheduleId.Value, nameof(dto.LoanScheduleId));

            var schedule = await _unitOfWork.LoanSchedule.GetByIdAsync(dto.LoanScheduleId.Value);

            if (schedule is null || schedule.LoanId != dto.LoanId)
                throw new InvalidOperationException("The specified schedule does not belong to the selected loan.");
        }

        if (dto.TransactionId.HasValue &&
            await _unitOfWork.Transaction.GetByIdAsync(dto.TransactionId.Value) is null)
            throw new InvalidOperationException("The specified transaction does not exist.");

        ValidateNonNegative(
            dto.AmountPaid,
            dto.PrincipalComponent,
            dto.InterestComponent,
            dto.PenaltyComponent);

        if (dto.AmountPaid != dto.PrincipalComponent + dto.InterestComponent + dto.PenaltyComponent)
            throw new InvalidOperationException("AmountPaid must equal the sum of principal, interest and penalty components.");

        var repayment = _mapper.Map<LoanRepayment>(dto);

        repayment.RepaymentReference = reference;
        repayment.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.LoanRepayment.AddAsync(repayment);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LoanRepaymentDto>(repayment);
    }

    public async Task<LoanRepaymentDto?> UpdateRepaymentAsync(UpdateLoanRepaymentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.LoanRepaymentId, nameof(dto.LoanRepaymentId));
        ValidateId(dto.LoanId, nameof(dto.LoanId));

        var repayment = await _unitOfWork.LoanRepayment.GetByIdAsync(dto.LoanRepaymentId);

        if (repayment is null)
            return null;

        if (repayment.Status is "Successful" or "Reversed")
            throw new InvalidOperationException("Successful or reversed repayment records are immutable. Use the reversal workflow for corrections.");

        var reference = RequiredText(dto.RepaymentReference, nameof(dto.RepaymentReference)).ToUpperInvariant();

        var duplicate = await _unitOfWork.LoanRepayment.GetByRepaymentReferenceAsync(reference);

        if (duplicate is not null && duplicate.LoanRepaymentId != dto.LoanRepaymentId)
            throw new InvalidOperationException($"Repayment reference '{reference}' already exists.");

        if (await _unitOfWork.Loan.GetByIdAsync(dto.LoanId) is null)
            throw new InvalidOperationException("The specified loan does not exist.");

        if (dto.LoanScheduleId.HasValue)
        {
            ValidateId(dto.LoanScheduleId.Value, nameof(dto.LoanScheduleId));

            var schedule = await _unitOfWork.LoanSchedule.GetByIdAsync(dto.LoanScheduleId.Value);

            if (schedule is null || schedule.LoanId != dto.LoanId)
                throw new InvalidOperationException("The specified schedule does not belong to the selected loan.");
        }

        if (dto.TransactionId.HasValue &&
            await _unitOfWork.Transaction.GetByIdAsync(dto.TransactionId.Value) is null)
            throw new InvalidOperationException("The specified transaction does not exist.");

        ValidateNonNegative(
            dto.AmountPaid,
            dto.PrincipalComponent,
            dto.InterestComponent,
            dto.PenaltyComponent);

        if (dto.AmountPaid != dto.PrincipalComponent + dto.InterestComponent + dto.PenaltyComponent)
            throw new InvalidOperationException("AmountPaid must equal the sum of principal, interest and penalty components.");

        _mapper.Map(dto, repayment);

        repayment.RepaymentReference = reference;
        repayment.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.LoanRepayment.Update(repayment);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<LoanRepaymentDto>(repayment);
    }

    public async Task<bool> DeleteRepaymentAsync(DeleteLoanRepaymentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.LoanRepaymentId, nameof(dto.LoanRepaymentId));

        var repayment = await _unitOfWork.LoanRepayment.GetByIdAsync(dto.LoanRepaymentId);

        if (repayment is null)
            return false;

        if (repayment.Status is "Successful" or "Reversed")
            throw new InvalidOperationException("Successful or reversed repayment records must be retained for financial audit history.");

        _unitOfWork.LoanRepayment.Delete(repayment);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // MAPPING HELPERS

    private async Task<TDto?> MapOneAsync<TEntity, TDto>(Task<TEntity?> entityTask)
        where TEntity : class
    {
        var entity = await entityTask;

        return entity is null
            ? default
            : _mapper.Map<TDto>(entity);
    }

    private async Task<IEnumerable<TDto>> MapManyAsync<TEntity, TDto>(Task<IEnumerable<TEntity>> entitiesTask)
        where TEntity : class
    {
        var entities = await entitiesTask;

        return _mapper.Map<IEnumerable<TDto>>(entities);
    }

    // VALIDATION HELPERS

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(parameterName, "Identifier must be greater than zero.");
    }

    private static string RequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A value is required.", parameterName);

        return value.Trim();
    }

    private static void ValidateLoanValues(
        string loanNumber,
        decimal requestedAmount,
        decimal annualInterestRate,
        int tenureMonths)
    {
        RequiredText(loanNumber, nameof(loanNumber));

        if (requestedAmount <= 0)
            throw new ArgumentOutOfRangeException(nameof(requestedAmount), "Requested amount must be greater than zero.");

        if (annualInterestRate < 0 || annualInterestRate > 100)
            throw new ArgumentOutOfRangeException(nameof(annualInterestRate), "Annual interest rate must be between 0 and 100.");

        if (tenureMonths <= 0)
            throw new ArgumentOutOfRangeException(nameof(tenureMonths), "Loan tenure must be greater than zero.");
    }

    private static void ValidateNonNegative(params decimal[] values)
    {
        if (values.Any(value => value < 0))
            throw new ArgumentOutOfRangeException(nameof(values), "Financial amounts cannot be negative.");
    }

    private static void EnsureRowVersion(byte[]? current, byte[]? supplied, string entityName)
    {
        if (current is null ||
            supplied is null ||
            supplied.Length == 0 ||
            !current.SequenceEqual(supplied))
        {
            throw new InvalidOperationException(
                $"The {entityName} was modified by another operation. Reload it and try again.");
        }
    }
}