using FluentValidation;
using FinVault.Application.DTOs.CardTransactionDtos;

namespace FinVault.Application.Validators.CardTransaction
{
    public class DeleteCardTransactionDtoValidator : AbstractValidator<DeleteCardTransactionDto>
    {
        public DeleteCardTransactionDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.CardTransactionId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
