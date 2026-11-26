using FluentValidation;
using FinVault.Application.DTOs.AuditLogDtos;

namespace FinVault.Application.Validators.AuditLog
{
    public class CreateAuditLogDtoValidator : AbstractValidator<CreateAuditLogDto>
    {
        public CreateAuditLogDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.Action)
                .NotEmpty();

            RuleFor(x => x.Action)
                .MaximumLength(50);

            RuleFor(x => x.EntityName)
                .NotEmpty();

            RuleFor(x => x.EntityName)
                .MaximumLength(100);

            RuleFor(x => x.EntityId)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Outcome)
                .NotEmpty();

            RuleFor(x => x.Outcome)
                .MaximumLength(10);

            RuleFor(x => x.Outcome)
                .Matches(@"^(Success|Failure)$");

            RuleFor(x => x.IpAddress)
                .MaximumLength(45);

            RuleFor(x => x.UserAgent)
                .MaximumLength(500);

            RuleFor(x => x.CorrelationId)
                .MaximumLength(64);

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
