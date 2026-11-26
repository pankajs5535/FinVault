using FluentValidation;
using FinVault.Application.DTOs.AccountHolderDtos;

namespace FinVault.Application.Validators.AccountHolder
{
    public class UpdateAccountHolderDtoValidator : AbstractValidator<UpdateAccountHolderDto>
    {
        public UpdateAccountHolderDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AccountHolderId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.HolderType)
                .NotEmpty();

            RuleFor(x => x.HolderType)
                .MaximumLength(30);

            RuleFor(x => x.HolderType)
                .Matches(@"^(Primary|Joint|Guardian|AuthorizedSignatory)$");

            RuleFor(x => x.AccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
