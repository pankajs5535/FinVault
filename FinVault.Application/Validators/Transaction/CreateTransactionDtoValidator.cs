using FluentValidation;
using FinVault.Application.DTOs.TransactionDtos;

namespace FinVault.Application.Validators.Transaction
{
    public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
    {
        public CreateTransactionDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.TransactionReference)
                .NotEmpty();

            RuleFor(x => x.TransactionReference)
                .MaximumLength(40);

            RuleFor(x => x.IdempotencyKey)
                .MaximumLength(64);

            RuleFor(x => x.ExternalReference)
                .MaximumLength(50);

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.FeeAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.TaxAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.SourceBalanceAfter)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Initiated|Pending|Processing|Posted|Failed|Reversed|Cancelled|OnHold)$");

            RuleFor(x => x.Channel)
                .NotEmpty();

            RuleFor(x => x.Channel)
                .MaximumLength(20);

            RuleFor(x => x.Channel)
                .Matches(@"^(Branch|ATM|NetBanking|MobileApp|UPI|POS|Api|Batch)$");

            RuleFor(x => x.FailureReason)
                .MaximumLength(250);

            RuleFor(x => x.Description)
                .MaximumLength(250);

            RuleFor(x => x.Remarks)
                .MaximumLength(250);

            RuleFor(x => x.TransactionTypeId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.SourceAccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.DestinationAccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CurrencyId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.FeeId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.InitiatedByUserId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ApprovedByUserId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ReversalOfTransactionId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
