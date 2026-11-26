using FluentValidation;
using FinVault.Application.DTOs.TransactionDtos;

namespace FinVault.Application.Validators.Transaction
{
    public class DeleteTransactionDtoValidator : AbstractValidator<DeleteTransactionDto>
    {
        public DeleteTransactionDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.TransactionId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
