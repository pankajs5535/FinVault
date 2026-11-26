using FluentValidation;
using FinVault.Application.DTOs.ComplianceRecordDtos;

namespace FinVault.Application.Validators.ComplianceRecord
{
    public class UpdateComplianceRecordDtoValidator : AbstractValidator<UpdateComplianceRecordDto>
    {
        public UpdateComplianceRecordDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.ComplianceRecordId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ReferenceNumber)
                .NotEmpty();

            RuleFor(x => x.ReferenceNumber)
                .MaximumLength(40);

            RuleFor(x => x.RecordType)
                .NotEmpty();

            RuleFor(x => x.RecordType)
                .MaximumLength(30);

            RuleFor(x => x.RecordType)
                .Matches(@"^(Str|Ctr|Ccr|Ntr|AmlScreening|PepCheck|SanctionsScreening|PeriodicKycReview|FatcaCrs)$");

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.RiskLevel)
                .NotEmpty();

            RuleFor(x => x.RiskLevel)
                .MaximumLength(10);

            RuleFor(x => x.RiskLevel)
                .Matches(@"^(Low|Medium|High|Critical)$");

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Open|UnderReview|Filed|Cleared|Closed)$");

            RuleFor(x => x.ReportingAuthority)
                .NotEmpty();

            RuleFor(x => x.ReportingAuthority)
                .MaximumLength(50);

            RuleFor(x => x.FilingReference)
                .MaximumLength(50);

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.TransactionId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.FraudAlertId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.FiledByEmployeeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
