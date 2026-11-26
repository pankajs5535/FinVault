using FluentValidation;
using FinVault.Application.DTOs.CardDtos;

namespace FinVault.Application.Validators.Card
{
    public class UpdateCardDtoValidator : AbstractValidator<UpdateCardDto>
    {
        public UpdateCardDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.CardId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CardNumberMasked)
                .NotEmpty();

            RuleFor(x => x.CardNumberMasked)
                .MaximumLength(19);

            RuleFor(x => x.CardHolderName)
                .NotEmpty();

            RuleFor(x => x.CardHolderName)
                .MaximumLength(100);

            RuleFor(x => x.CardType)
                .NotEmpty();

            RuleFor(x => x.CardType)
                .MaximumLength(10);

            RuleFor(x => x.CardType)
                .Matches(@"^(Debit|Credit|Prepaid)$");

            RuleFor(x => x.Network)
                .NotEmpty();

            RuleFor(x => x.Network)
                .MaximumLength(15);

            RuleFor(x => x.Network)
                .Matches(@"^(RuPay|Visa|Mastercard)$");

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Pending|Active|Blocked|Hotlisted|Expired|Cancelled)$");

            RuleFor(x => x.BlockReason)
                .MaximumLength(250);

            RuleFor(x => x.DailyAtmLimit)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.DailyPosLimit)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.DailyOnlineLimit)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CreditLimit)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.AvailableCredit)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.OutstandingAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.BillingCycleDay)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.AccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RowVersion)
                .NotEmpty();
        }
    }
}
