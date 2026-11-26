using FluentValidation;
using FinVault.Application.DTOs.UserDtos;

namespace FinVault.Application.Validators.User
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Username)
                .NotEmpty();

            RuleFor(x => x.Username)
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty();

            RuleFor(x => x.Email)
                .MaximumLength(256);

            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15);

            RuleFor(x => x.PhoneNumber)
                .Must(value => string.IsNullOrWhiteSpace(value) || new System.ComponentModel.DataAnnotations.PhoneAttribute().IsValid(value))
                .WithMessage("PhoneNumber must be a valid phone number.");

            RuleFor(x => x.UserType)
                .NotEmpty();

            RuleFor(x => x.UserType)
                .MaximumLength(20);

            RuleFor(x => x.UserType)
                .Matches(@"^(Customer|Employee|Admin|System)$");

            RuleFor(x => x.RoleId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RowVersion)
                .NotEmpty();
        }
    }
}
