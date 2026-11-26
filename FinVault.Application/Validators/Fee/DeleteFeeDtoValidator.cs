using FluentValidation;
using FinVault.Application.DTOs.FeeDtos;

namespace FinVault.Application.Validators.Fee
{
    public class DeleteFeeDtoValidator : AbstractValidator<DeleteFeeDto>
    {
        public DeleteFeeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.FeeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
