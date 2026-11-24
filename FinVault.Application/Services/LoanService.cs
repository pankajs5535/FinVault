using AutoMapper;
using FinVault.Application.DTOs.LoanDtos;
using FinVault.Application.DTOs.LoanRepaymentDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public sealed class LoanService : ILoanService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LoanService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieve one loan with its related details.
    public async Task<LoanDto?> GetByIdAsync(int loanId)
    {
        var loan = await _unitOfWork.Loan.GetLoanDetailsAsync(loanId);
        return loan == null ? null : _mapper.Map<LoanDto>(loan);
    }

    // 2. Retrieve all loans belonging to one customer.
    public async Task<IReadOnlyList<LoanDto>> GetByCustomerIdAsync(int customerId)
    {
        var loans = await _unitOfWork.Loan.GetByCustomerIdAsync(customerId);
        return _mapper.Map<IReadOnlyList<LoanDto>>(loans);
    }

    // 3. Create a new loan application after checking its unique loan number.
    public async Task<LoanDto> ApplyAsync(CreateLoanDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.RequestedAmount <= 0) throw new ArgumentOutOfRangeException(nameof(dto.RequestedAmount));
        if (dto.TenureMonths <= 0) throw new ArgumentOutOfRangeException(nameof(dto.TenureMonths));
        if (await _unitOfWork.Loan.ExistsByLoanNumberAsync(dto.LoanNumber.Trim()))
            throw new InvalidOperationException("A loan with this loan number already exists.");

        var loan = _mapper.Map<Loan>(dto);
        loan.LoanNumber = dto.LoanNumber.Trim();
        loan.Status = "Applied";
        loan.ApplicationDate = dto.ApplicationDate == default ? DateTime.UtcNow : dto.ApplicationDate;
        loan.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Loan.AddAsync(loan);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<LoanDto>(loan);
    }

    // 4. Update only an application that has not yet been approved or disbursed.
    public async Task<LoanDto?> UpdateApplicationAsync(int loanId, UpdateLoanDto dto)
    {
        var loan = await _unitOfWork.Loan.GetByIdAsync(loanId);
        if (loan == null) return null;
        if (!string.Equals(loan.Status, "Applied", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only applications in Applied status can be changed.");
        if (dto.LoanId != loanId) throw new ArgumentException("Route loan ID and DTO loan ID do not match.");
        _mapper.Map(dto, loan);
        loan.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Loan.Update(loan);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<LoanDto>(loan);
    }

    // 5. Mark an application as approved and record the approving employee.
    public async Task<bool> ApproveAsync(int loanId, int approvedByUserId)
    {
        var loan = await _unitOfWork.Loan.GetByIdAsync(loanId);
        if (loan == null) return false;
        if (!string.Equals(loan.Status, "Applied", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only an Applied loan can be approved.");
        loan.Status = "Approved";
        loan.ApprovedByEmployeeId = approvedByUserId;
        loan.SanctionDate = DateTime.UtcNow;
        loan.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Loan.Update(loan);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 6. Record the disbursement state. Actual account debit/credit must be handled by a ledger transaction.
    public async Task<bool> DisburseAsync(int loanId)
    {
        var loan = await _unitOfWork.Loan.GetByIdAsync(loanId);
        if (loan == null) return false;
        if (!string.Equals(loan.Status, "Approved", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only an Approved loan can be disbursed.");
        if (loan.SanctionedAmount is null or <= 0)
            throw new InvalidOperationException("A positive sanctioned amount is required before disbursement.");
        loan.Status = "Active";
        loan.DisbursedAmount = loan.SanctionedAmount.Value;
        loan.DisbursementDate = DateTime.UtcNow;
        loan.OutstandingPrincipal = loan.SanctionedAmount.Value;
        loan.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Loan.Update(loan);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 7. Record a repayment. Recalculation of schedules and balances must be done in the same ledger transaction.
    public async Task<LoanRepaymentDto> RecordRepaymentAsync(CreateLoanRepaymentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.AmountPaid <= 0 || dto.PrincipalComponent < 0 || dto.InterestComponent < 0 || dto.PenaltyComponent < 0)
            throw new ArgumentOutOfRangeException(nameof(dto), "Repayment amounts must be positive or zero.");
        var loan = await _unitOfWork.Loan.GetByIdAsync(dto.LoanId)
            ?? throw new KeyNotFoundException("Loan was not found.");
        if (!string.Equals(loan.Status, "Active", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Repayments can only be recorded for an Active loan.");
        if (dto.PrincipalComponent + dto.InterestComponent + dto.PenaltyComponent != dto.AmountPaid)
            throw new InvalidOperationException("Repayment components must add up to AmountPaid.");

        var repayment = _mapper.Map<LoanRepayment>(dto);
        repayment.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.LoanRepayment.AddAsync(repayment);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<LoanRepaymentDto>(repayment);
    }
}
