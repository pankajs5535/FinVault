using FluentValidation;
using FinVault.Application.DTOs.PermissionDtos;

namespace FinVault.Application.Validators.Permission
{
    public class DeletePermissionDtoValidator : AbstractValidator<DeletePermissionDto>
    {
        public DeletePermissionDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.PermissionId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
