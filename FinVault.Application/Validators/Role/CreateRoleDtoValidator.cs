using FluentValidation;
using FinVault.Application.DTOs.RoleDtos;

namespace FinVault.Application.Validators.Role
{
    public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
    {
        public CreateRoleDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .MaximumLength(250);
        }
    }
}
