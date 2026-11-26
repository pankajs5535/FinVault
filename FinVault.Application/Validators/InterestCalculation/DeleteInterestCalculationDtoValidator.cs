using FluentValidation;
using FinVault.Application.DTOs.InterestCalculationDtos;

namespace FinVault.Application.Validators.InterestCalculation
{
    public class DeleteInterestCalculationDtoValidator : AbstractValidator<DeleteInterestCalculationDto>
    {
        public DeleteInterestCalculationDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.InterestCalculationId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
