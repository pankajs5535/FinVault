using FluentValidation;
using FinVault.Application.DTOs.InterestRateDtos;

namespace FinVault.Application.Validators.InterestRate
{
    public class CreateInterestRateDtoValidator : AbstractValidator<CreateInterestRateDto>
    {
        public CreateInterestRateDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.RateName)
                .NotEmpty();

            RuleFor(x => x.RateName)
                .MaximumLength(100);

            RuleFor(x => x.ProductCategory)
                .NotEmpty();

            RuleFor(x => x.ProductCategory)
                .MaximumLength(20);

            RuleFor(x => x.ProductCategory)
                .Matches(@"^(Savings|Current|FixedDeposit|RecurringDeposit|Loan)$");

            RuleFor(x => x.LoanType)
                .MaximumLength(20);

            RuleFor(x => x.AnnualRate)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.SeniorCitizenAdditionalRate)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CompoundingFrequency)
                .NotEmpty();

            RuleFor(x => x.CompoundingFrequency)
                .MaximumLength(15);

            RuleFor(x => x.CompoundingFrequency)
                .Matches(@"^(Simple|Daily|Monthly|Quarterly|HalfYearly|Yearly)$");

            RuleFor(x => x.MinAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MaxAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MinTenureMonths)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MaxTenureMonths)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.BenchmarkName)
                .MaximumLength(50);

            RuleFor(x => x.SpreadOverBenchmark)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.AccountTypeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
