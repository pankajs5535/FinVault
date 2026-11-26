using FluentValidation;
using FinVault.Application.DTOs.JournalEntryDtos;

namespace FinVault.Application.Validators.JournalEntry
{
    public class UpdateJournalEntryDtoValidator : AbstractValidator<UpdateJournalEntryDto>
    {
        public UpdateJournalEntryDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.JournalEntryId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.JournalNumber)
                .NotEmpty();

            RuleFor(x => x.JournalNumber)
                .MaximumLength(30);

            RuleFor(x => x.JournalType)
                .NotEmpty();

            RuleFor(x => x.JournalType)
                .MaximumLength(20);

            RuleFor(x => x.JournalType)
                .Matches(@"^(Transaction|Interest|Fee|Loan|Adjustment|Reversal)$");

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.Description)
                .MaximumLength(250);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Draft|Posted|Reversed)$");

            RuleFor(x => x.CurrencyId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.TransactionId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PostedByUserId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ReversalOfJournalEntryId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RowVersion)
                .NotEmpty();
        }
    }
}
