using FluentValidation;
using FinVault.Application.DTOs.PaymentMethodDtos;

namespace FinVault.Application.Validators.PaymentMethod
{
    public class CreatePaymentMethodDtoValidator : AbstractValidator<CreatePaymentMethodDto>
    {
        public CreatePaymentMethodDtoValidator()
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

            RuleFor(x => x.Rail)
                .NotEmpty();

            RuleFor(x => x.Rail)
                .MaximumLength(20);

            RuleFor(x => x.Rail)
                .Matches(@"^(UPI|NEFT|RTGS|IMPS|NACH|Cheque|Card|Internal)$");

            RuleFor(x => x.Description)
                .MaximumLength(250);

            RuleFor(x => x.MinAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MaxAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.DailyLimit)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.SettlementType)
                .NotEmpty();

            RuleFor(x => x.SettlementType)
                .MaximumLength(20);

            RuleFor(x => x.SettlementType)
                .Matches(@"^(RealTime|DeferredNet|Batch)$");
        }
    }
}
