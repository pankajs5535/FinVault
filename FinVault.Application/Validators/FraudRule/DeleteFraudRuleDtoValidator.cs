using FluentValidation;
using FinVault.Application.DTOs.FraudRuleDtos;

namespace FinVault.Application.Validators.FraudRule
{
    public class DeleteFraudRuleDtoValidator : AbstractValidator<DeleteFraudRuleDto>
    {
        public DeleteFraudRuleDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.FraudRuleId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
