using FluentValidation;
using FinVault.Application.DTOs.ReportConfigurationDtos;

namespace FinVault.Application.Validators.ReportConfiguration
{
    public class DeleteReportConfigurationDtoValidator : AbstractValidator<DeleteReportConfigurationDto>
    {
        public DeleteReportConfigurationDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.ReportConfigurationId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
