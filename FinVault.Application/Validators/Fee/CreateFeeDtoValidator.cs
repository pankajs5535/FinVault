using FluentValidation;
using FinVault.Application.DTOs.FeeDtos;

namespace FinVault.Application.Validators.Fee
{
    public class CreateFeeDtoValidator : AbstractValidator<CreateFeeDto>
    {
        public CreateFeeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.Code)
                .NotEmpty();

            RuleFor(x => x.Code)
                .MaximumLength(30);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(100);

            RuleFor(x => x.CalculationMethod)
                .NotEmpty();

            RuleFor(x => x.CalculationMethod)
                .MaximumLength(15);

            RuleFor(x => x.CalculationMethod)
                .Matches(@"^(Flat|Percentage|Slab)$");

            RuleFor(x => x.Frequency)
                .NotEmpty();

            RuleFor(x => x.Frequency)
                .MaximumLength(20);

            RuleFor(x => x.Frequency)
                .Matches(@"^(PerTransaction|Monthly|Quarterly|Yearly|OneTime)$");

            RuleFor(x => x.FlatAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Percentage)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MinimumAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MaximumAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.GstRate)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.TransactionTypeId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PaymentMethodId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AccountTypeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
