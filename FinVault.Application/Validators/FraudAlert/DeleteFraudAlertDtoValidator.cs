using FluentValidation;
using FinVault.Application.DTOs.FraudAlertDtos;

namespace FinVault.Application.Validators.FraudAlert
{
    public class DeleteFraudAlertDtoValidator : AbstractValidator<DeleteFraudAlertDto>
    {
        public DeleteFraudAlertDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.FraudAlertId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
