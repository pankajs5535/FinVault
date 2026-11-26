using FluentValidation;
using FinVault.Application.DTOs.AccountTypeDtos;

namespace FinVault.Application.Validators.AccountType
{
    public class CreateAccountTypeDtoValidator : AbstractValidator<CreateAccountTypeDto>
    {
        public CreateAccountTypeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.Code)
                .NotEmpty();

            RuleFor(x => x.Code)
                .MaximumLength(20);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(100);

            RuleFor(x => x.Category)
                .NotEmpty();

            RuleFor(x => x.Category)
                .MaximumLength(20);

            RuleFor(x => x.Category)
                .Matches(@"^(Savings|Current|FixedDeposit|RecurringDeposit|Internal)$");

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.MinimumBalance)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MinimumAverageBalance)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.DailyTransactionLimit)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.InterestPayoutFrequency)
                .MaximumLength(20);

            RuleFor(x => x.InterestPayoutFrequency)
                .Matches(@"^(Monthly|Quarterly|HalfYearly|Yearly|AtMaturity)$");

            RuleFor(x => x.MinTenureMonths)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MaxTenureMonths)
                .GreaterThanOrEqualTo(0);
        }
    }
}
