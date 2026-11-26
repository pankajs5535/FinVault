using FluentValidation;
using FinVault.Application.DTOs.LoanDtos;

namespace FinVault.Application.Validators.Loan
{
    public class DeleteLoanDtoValidator : AbstractValidator<DeleteLoanDto>
    {
        public DeleteLoanDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.LoanId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
