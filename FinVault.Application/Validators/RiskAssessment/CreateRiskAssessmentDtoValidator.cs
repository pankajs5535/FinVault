using FluentValidation;
using FinVault.Application.DTOs.RiskAssessmentDtos;

namespace FinVault.Application.Validators.RiskAssessment
{
    public class CreateRiskAssessmentDtoValidator : AbstractValidator<CreateRiskAssessmentDto>
    {
        public CreateRiskAssessmentDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AssessmentType)
                .NotEmpty();

            RuleFor(x => x.AssessmentType)
                .MaximumLength(20);

            RuleFor(x => x.AssessmentType)
                .Matches(@"^(Customer|Account|Transaction)$");

            RuleFor(x => x.RiskScore)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.RiskLevel)
                .NotEmpty();

            RuleFor(x => x.RiskLevel)
                .MaximumLength(10);

            RuleFor(x => x.RiskLevel)
                .Matches(@"^(Low|Medium|High|Critical)$");

            RuleFor(x => x.Decision)
                .NotEmpty();

            RuleFor(x => x.Decision)
                .MaximumLength(15);

            RuleFor(x => x.Decision)
                .Matches(@"^(Approve|Review|Challenge|Decline)$");

            RuleFor(x => x.ModelVersion)
                .NotEmpty();

            RuleFor(x => x.ModelVersion)
                .MaximumLength(30);

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.TransactionId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
