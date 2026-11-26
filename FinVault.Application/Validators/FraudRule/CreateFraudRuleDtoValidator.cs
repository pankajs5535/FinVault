using FluentValidation;
using FinVault.Application.DTOs.FraudRuleDtos;

namespace FinVault.Application.Validators.FraudRule
{
    public class CreateFraudRuleDtoValidator : AbstractValidator<CreateFraudRuleDto>
    {
        public CreateFraudRuleDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.RuleCode)
                .NotEmpty();

            RuleFor(x => x.RuleCode)
                .MaximumLength(30);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.RuleType)
                .NotEmpty();

            RuleFor(x => x.RuleType)
                .MaximumLength(20);

            RuleFor(x => x.RuleType)
                .Matches(@"^(Velocity|Amount|Geo|Device|Behavioural|Blacklist|Pattern)$");

            RuleFor(x => x.Severity)
                .NotEmpty();

            RuleFor(x => x.Severity)
                .MaximumLength(10);

            RuleFor(x => x.Severity)
                .Matches(@"^(Low|Medium|High|Critical)$");

            RuleFor(x => x.RuleDefinitionJson)
                .NotEmpty();

            RuleFor(x => x.ThresholdAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.TimeWindowMinutes)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.ActionOnMatch)
                .NotEmpty();

            RuleFor(x => x.ActionOnMatch)
                .MaximumLength(20);

            RuleFor(x => x.ActionOnMatch)
                .Matches(@"^(Alert|Block|StepUpAuth|Review)$");

            RuleFor(x => x.Priority)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Version)
                .GreaterThanOrEqualTo(0);
        }
    }
}
