using FluentValidation;
using FinVault.Application.DTOs.BranchDtos;

namespace FinVault.Application.Validators.Branch
{
    public class DeleteBranchDtoValidator : AbstractValidator<DeleteBranchDto>
    {
        public DeleteBranchDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.BranchId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
