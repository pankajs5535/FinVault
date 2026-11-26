using FluentValidation;
using FinVault.Application.DTOs.InterestRateDtos;

namespace FinVault.Application.Validators.InterestRate
{
    public class DeleteInterestRateDtoValidator : AbstractValidator<DeleteInterestRateDto>
    {
        public DeleteInterestRateDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.InterestRateId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
