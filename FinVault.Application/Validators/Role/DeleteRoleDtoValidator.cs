using FluentValidation;
using FinVault.Application.DTOs.RoleDtos;

namespace FinVault.Application.Validators.Role
{
    public class DeleteRoleDtoValidator : AbstractValidator<DeleteRoleDto>
    {
        public DeleteRoleDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.RoleId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
