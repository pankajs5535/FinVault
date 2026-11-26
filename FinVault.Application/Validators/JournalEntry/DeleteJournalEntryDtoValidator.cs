using FluentValidation;
using FinVault.Application.DTOs.JournalEntryDtos;

namespace FinVault.Application.Validators.JournalEntry
{
    public class DeleteJournalEntryDtoValidator : AbstractValidator<DeleteJournalEntryDto>
    {
        public DeleteJournalEntryDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.JournalEntryId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
