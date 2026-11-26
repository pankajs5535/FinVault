using FluentValidation;
using FinVault.Application.DTOs.AccountDtos;

namespace FinVault.Application.Validators.Account
{
    public class DeleteAccountDtoValidator : AbstractValidator<DeleteAccountDto>
    {
        public DeleteAccountDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AccountId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
