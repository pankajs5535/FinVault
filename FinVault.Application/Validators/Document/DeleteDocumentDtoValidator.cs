using FluentValidation;
using FinVault.Application.DTOs.DocumentDtos;

namespace FinVault.Application.Validators.Document
{
    public class DeleteDocumentDtoValidator : AbstractValidator<DeleteDocumentDto>
    {
        public DeleteDocumentDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.DocumentId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
