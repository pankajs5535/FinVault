using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.LoanDtos;
using FinVault.Application.DTOs.LoanRepaymentDtos;
using FinVault.Application.DTOs.LoanScheduleDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Loan banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface ILoanService
{
        Task<LoanDto?> GetByIdAsync(int loanId);
        Task<IReadOnlyList<LoanDto>> GetByCustomerIdAsync(int customerId);
        Task<LoanDto> ApplyAsync(CreateLoanDto dto);
        Task<LoanDto?> UpdateApplicationAsync(int loanId, UpdateLoanDto dto);
        Task<bool> ApproveAsync(int loanId, int approvedByUserId);
        Task<bool> DisburseAsync(int loanId);
        Task<LoanRepaymentDto> RecordRepaymentAsync(CreateLoanRepaymentDto dto);
}
