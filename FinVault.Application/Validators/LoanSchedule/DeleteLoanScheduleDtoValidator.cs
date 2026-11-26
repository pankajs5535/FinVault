using FluentValidation;
using FinVault.Application.DTOs.LoanScheduleDtos;

namespace FinVault.Application.Validators.LoanSchedule
{
    public class DeleteLoanScheduleDtoValidator : AbstractValidator<DeleteLoanScheduleDto>
    {
        public DeleteLoanScheduleDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.LoanScheduleId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
