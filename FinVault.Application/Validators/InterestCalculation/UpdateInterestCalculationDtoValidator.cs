using FluentValidation;
using FinVault.Application.DTOs.InterestCalculationDtos;

namespace FinVault.Application.Validators.InterestCalculation
{
    public class UpdateInterestCalculationDtoValidator : AbstractValidator<UpdateInterestCalculationDto>
    {
        public UpdateInterestCalculationDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.InterestCalculationId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CalculationType)
                .NotEmpty();

            RuleFor(x => x.CalculationType)
                .MaximumLength(20);

            RuleFor(x => x.CalculationType)
                .Matches(@"^(Accrual|Payout|Capitalisation)$");

            RuleFor(x => x.DayCount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.PrincipalBase)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.AppliedRate)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.InterestAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.TdsAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Calculated|Posted|Reversed)$");

            RuleFor(x => x.AccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.LoanId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.InterestRateId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.JournalEntryId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.TransactionId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
