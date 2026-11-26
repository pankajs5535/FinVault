using FluentValidation;
using FinVault.Application.DTOs.PermissionDtos;

namespace FinVault.Application.Validators.Permission
{
    public class UpdatePermissionDtoValidator : AbstractValidator<UpdatePermissionDto>
    {
        public UpdatePermissionDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.PermissionId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Code)
                .NotEmpty();

            RuleFor(x => x.Code)
                .MaximumLength(100);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(100);

            RuleFor(x => x.ModuleName)
                .NotEmpty();

            RuleFor(x => x.ModuleName)
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .MaximumLength(250);
        }
    }
}
