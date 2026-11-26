using FluentValidation;
using FinVault.Application.DTOs.JournalEntryLineDtos;

namespace FinVault.Application.Validators.JournalEntryLine
{
    public class CreateJournalEntryLineDtoValidator : AbstractValidator<CreateJournalEntryLineDto>
    {
        public CreateJournalEntryLineDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.LineNumber)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.DebitAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CreditAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Narration)
                .MaximumLength(250);

            RuleFor(x => x.JournalEntryId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AccountId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
