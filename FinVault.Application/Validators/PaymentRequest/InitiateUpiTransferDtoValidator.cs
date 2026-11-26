using FluentValidation;
using FinVault.Application.DTOs.PaymentRequestDtos;

namespace FinVault.Application.Validators.PaymentRequest
{
    public class InitiateUpiTransferDtoValidator : AbstractValidator<InitiateUpiTransferDto>
    {
        public InitiateUpiTransferDtoValidator()
        {
            // 1. SOURCE BANK ACCOUNT
            RuleFor(x => x.SourceAccountId)
                .GreaterThan(0);

            // 2. BENEFICIARY
            RuleFor(x => x.BeneficiaryId)
                .GreaterThan(0);

            // 3. TRANSFER AMOUNT
            RuleFor(x => x.Amount)
                .InclusiveBetween(0.01m, 100000000m);

            // 4. PURPOSE AND REMARKS
            RuleFor(x => x.Purpose)
                .MaximumLength(100);

            RuleFor(x => x.Remarks)
                .MaximumLength(250);

            // 5. IDEMPOTENCY
            RuleFor(x => x.IdempotencyKey)
                .NotEmpty()
                .MinimumLength(16)
                .MaximumLength(64);
        }
    }
}
