using FluentValidation;
using FinVault.Application.DTOs.ComplianceRecordDtos;

namespace FinVault.Application.Validators.ComplianceRecord
{
    public class DeleteComplianceRecordDtoValidator : AbstractValidator<DeleteComplianceRecordDto>
    {
        public DeleteComplianceRecordDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.ComplianceRecordId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
