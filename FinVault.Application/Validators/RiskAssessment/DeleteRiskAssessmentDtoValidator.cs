using FluentValidation;
using FinVault.Application.DTOs.RiskAssessmentDtos;

namespace FinVault.Application.Validators.RiskAssessment
{
    public class DeleteRiskAssessmentDtoValidator : AbstractValidator<DeleteRiskAssessmentDto>
    {
        public DeleteRiskAssessmentDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.RiskAssessmentId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
