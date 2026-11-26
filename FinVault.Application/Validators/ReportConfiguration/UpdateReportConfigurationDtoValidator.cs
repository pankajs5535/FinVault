using FluentValidation;
using FinVault.Application.DTOs.ReportConfigurationDtos;

namespace FinVault.Application.Validators.ReportConfiguration
{
    public class UpdateReportConfigurationDtoValidator : AbstractValidator<UpdateReportConfigurationDto>
    {
        public UpdateReportConfigurationDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.ReportConfigurationId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ReportCode)
                .NotEmpty();

            RuleFor(x => x.ReportCode)
                .MaximumLength(50);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(150);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.ReportType)
                .NotEmpty();

            RuleFor(x => x.ReportType)
                .MaximumLength(20);

            RuleFor(x => x.ReportType)
                .Matches(@"^(Statement|Regulatory|Operational|Financial|Analytics)$");

            RuleFor(x => x.OutputFormat)
                .NotEmpty();

            RuleFor(x => x.OutputFormat)
                .MaximumLength(10);

            RuleFor(x => x.OutputFormat)
                .Matches(@"^(Pdf|Excel|Csv|Json)$");

            RuleFor(x => x.ScheduleCron)
                .MaximumLength(50);

            RuleFor(x => x.Recipients)
                .MaximumLength(1000);

            RuleFor(x => x.RetentionDays)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.OwnerUserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
