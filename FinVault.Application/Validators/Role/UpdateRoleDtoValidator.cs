using FluentValidation;
using FinVault.Application.DTOs.RoleDtos;

namespace FinVault.Application.Validators.Role
{
    public class UpdateRoleDtoValidator : AbstractValidator<UpdateRoleDto>
    {
        public UpdateRoleDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.RoleId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .MaximumLength(250);
        }
    }
}
