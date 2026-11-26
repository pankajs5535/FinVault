using FluentValidation;
using FinVault.Application.DTOs.PaymentRequestDtos;

namespace FinVault.Application.Validators.PaymentRequest
{
    public class CreatePaymentRequestDtoValidator : AbstractValidator<CreatePaymentRequestDto>
    {
        public CreatePaymentRequestDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.IdempotencyKey)
                .NotEmpty();

            RuleFor(x => x.IdempotencyKey)
                .MinimumLength(16)
                .MaximumLength(64);

            RuleFor(x => x.PayeeName)
                .MaximumLength(150);

            RuleFor(x => x.PayeeAccountNumber)
                .MaximumLength(20);

            RuleFor(x => x.PayeeIfsc)
                .MaximumLength(11);

            RuleFor(x => x.PayeeIfsc)
                .Matches(@"^[A-Z]{4}0[A-Z0-9]{6}$");

            RuleFor(x => x.PayeeVpa)
                .MaximumLength(100);

            RuleFor(x => x.Amount)
                .InclusiveBetween(0.01m, 9999999999999999.99m);
;

            RuleFor(x => x.Purpose)
                .MaximumLength(250);

            RuleFor(x => x.Remarks)
                .MaximumLength(250);

            RuleFor(x => x.PaymentMethodId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.SourceAccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.BeneficiaryId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
