using FluentValidation;
using FinVault.Application.DTOs.AccountHolderDtos;

namespace FinVault.Application.Validators.AccountHolder
{
    public class DeleteAccountHolderDtoValidator : AbstractValidator<DeleteAccountHolderDto>
    {
        public DeleteAccountHolderDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AccountHolderId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
