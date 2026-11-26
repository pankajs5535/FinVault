using FluentValidation;
using FinVault.Application.DTOs.JournalEntryLineDtos;

namespace FinVault.Application.Validators.JournalEntryLine
{
    public class DeleteJournalEntryLineDtoValidator : AbstractValidator<DeleteJournalEntryLineDto>
    {
        public DeleteJournalEntryLineDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.JournalEntryLineId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
