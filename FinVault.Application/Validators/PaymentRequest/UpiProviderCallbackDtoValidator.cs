using FluentValidation;
using FinVault.Application.DTOs.PaymentRequestDtos;

namespace FinVault.Application.Validators.PaymentRequest
{
    public class UpiProviderCallbackDtoValidator : AbstractValidator<UpiProviderCallbackDto>
    {
        public UpiProviderCallbackDtoValidator()
        {
            // 1. PROVIDER EVENT IDENTIFICATION
            RuleFor(x => x.ProviderEventId)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.ProviderReference)
                .NotEmpty()
                .MaximumLength(100);

            // 2. FINVAULT PAYMENT IDENTIFICATION
            RuleFor(x => x.RequestReference)
                .NotEmpty()
                .MaximumLength(40);

            // 3. PAYMENT RESULT
            RuleFor(x => x.Status)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.UtrNumber)
                .MaximumLength(30);

            RuleFor(x => x.ResponseCode)
                .MaximumLength(30);

            RuleFor(x => x.ResponseMessage)
                .MaximumLength(250);

            // 4. PROVIDER EVENT TIMESTAMP
            RuleFor(x => x.OccurredAt)
                .NotEmpty();
        }
    }
}
