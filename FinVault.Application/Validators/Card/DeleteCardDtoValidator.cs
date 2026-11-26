using FluentValidation;
using FinVault.Application.DTOs.CardDtos;

namespace FinVault.Application.Validators.Card
{
    public class DeleteCardDtoValidator : AbstractValidator<DeleteCardDto>
    {
        public DeleteCardDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.CardId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
