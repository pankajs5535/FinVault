using FluentValidation;
using FinVault.Application.DTOs.PaymentRequestDtos;

namespace FinVault.Application.Validators.PaymentRequest
{
    public class VerifyUpiVpaDtoValidator : AbstractValidator<VerifyUpiVpaDto>
    {
        public VerifyUpiVpaDtoValidator()
        {
            // 1. UPI IDENTIFICATION
            RuleFor(x => x.UpiVpa)
                .NotEmpty()
                .MaximumLength(100)
                .Matches(@"^[A-Za-z0-9._-]{2,256}@[A-Za-z0-9.-]{2,64}$");
        }
    }
}
