using FluentValidation;
using FinVault.Application.DTOs.FraudAlertDtos;

namespace FinVault.Application.Validators.FraudAlert
{
    public class UpdateFraudAlertDtoValidator : AbstractValidator<UpdateFraudAlertDto>
    {
        public UpdateFraudAlertDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.FraudAlertId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AlertNumber)
                .NotEmpty();

            RuleFor(x => x.AlertNumber)
                .MaximumLength(30);

            RuleFor(x => x.Severity)
                .NotEmpty();

            RuleFor(x => x.Severity)
                .MaximumLength(10);

            RuleFor(x => x.Severity)
                .Matches(@"^(Low|Medium|High|Critical)$");

            RuleFor(x => x.Score)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Open|UnderReview|ConfirmedFraud|FalsePositive|Closed)$");

            RuleFor(x => x.ReviewNotes)
                .MaximumLength(1000);

            RuleFor(x => x.ActionTaken)
                .MaximumLength(100);

            RuleFor(x => x.FraudRuleId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.TransactionId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CardId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RiskAssessmentId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ReviewedByEmployeeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
