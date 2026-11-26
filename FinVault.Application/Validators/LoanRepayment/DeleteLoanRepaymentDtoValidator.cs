using FluentValidation;
using FinVault.Application.DTOs.LoanRepaymentDtos;

namespace FinVault.Application.Validators.LoanRepayment
{
    public class DeleteLoanRepaymentDtoValidator : AbstractValidator<DeleteLoanRepaymentDto>
    {
        public DeleteLoanRepaymentDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.LoanRepaymentId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
