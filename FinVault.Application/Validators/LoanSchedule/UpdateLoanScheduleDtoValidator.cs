using FluentValidation;
using FinVault.Application.DTOs.LoanScheduleDtos;

namespace FinVault.Application.Validators.LoanSchedule
{
    public class UpdateLoanScheduleDtoValidator : AbstractValidator<UpdateLoanScheduleDto>
    {
        public UpdateLoanScheduleDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.LoanScheduleId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.InstallmentNumber)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.OpeningBalance)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.ClosingBalance)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.PrincipalDue)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.InterestDue)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.PenaltyAccrued)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.PrincipalPaid)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.InterestPaid)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.PenaltyPaid)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Pending|PartiallyPaid|Paid|Overdue|Waived)$");

            RuleFor(x => x.LoanId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RowVersion)
                .NotEmpty();
        }
    }
}
