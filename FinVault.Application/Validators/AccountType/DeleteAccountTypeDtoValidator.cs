using FluentValidation;
using FinVault.Application.DTOs.AccountTypeDtos;

namespace FinVault.Application.Validators.AccountType
{
    public class DeleteAccountTypeDtoValidator : AbstractValidator<DeleteAccountTypeDto>
    {
        public DeleteAccountTypeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AccountTypeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
