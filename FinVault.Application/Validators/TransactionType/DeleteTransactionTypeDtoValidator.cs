using FluentValidation;
using FinVault.Application.DTOs.TransactionTypeDtos;

namespace FinVault.Application.Validators.TransactionType
{
    public class DeleteTransactionTypeDtoValidator : AbstractValidator<DeleteTransactionTypeDto>
    {
        public DeleteTransactionTypeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.TransactionTypeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
