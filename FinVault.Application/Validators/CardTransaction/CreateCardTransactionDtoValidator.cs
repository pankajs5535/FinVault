using FluentValidation;
using FinVault.Application.DTOs.CardTransactionDtos;

namespace FinVault.Application.Validators.CardTransaction
{
    public class CreateCardTransactionDtoValidator : AbstractValidator<CreateCardTransactionDto>
    {
        public CreateCardTransactionDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AuthorizationCode)
                .MaximumLength(20);

            RuleFor(x => x.RetrievalReferenceNumber)
                .MaximumLength(12);

            RuleFor(x => x.AuthorizationStatus)
                .NotEmpty();

            RuleFor(x => x.AuthorizationStatus)
                .MaximumLength(20);

            RuleFor(x => x.AuthorizationStatus)
                .Matches(@"^(Authorized|Declined|Reversed|Settled|Expired)$");

            RuleFor(x => x.DeclineReason)
                .MaximumLength(100);

            RuleFor(x => x.MerchantName)
                .NotEmpty();

            RuleFor(x => x.MerchantName)
                .MaximumLength(150);

            RuleFor(x => x.MerchantCategoryCode)
                .NotEmpty();

            RuleFor(x => x.MerchantCategoryCode)
                .MaximumLength(4);

            RuleFor(x => x.MerchantCity)
                .MaximumLength(100);

            RuleFor(x => x.TerminalId)
                .MaximumLength(20);

            RuleFor(x => x.Channel)
                .NotEmpty();

            RuleFor(x => x.Channel)
                .MaximumLength(20);

            RuleFor(x => x.Channel)
                .Matches(@"^(POS|ATM|Ecommerce|Contactless|Recurring)$");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.SettlementAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CardId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.TransactionId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CurrencyId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
