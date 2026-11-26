using FluentValidation;
using FinVault.Application.DTOs.KYCDetailsDtos;

namespace FinVault.Application.Validators.KYCDetails
{
    public class UpdateKYCDetailsDtoValidator : AbstractValidator<UpdateKYCDetailsDto>
    {
        public UpdateKYCDetailsDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.KYCDetailsId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.KycLevel)
                .NotEmpty();

            RuleFor(x => x.KycLevel)
                .MaximumLength(20);

            RuleFor(x => x.KycLevel)
                .Matches(@"^(Minimum|Full|Video|EKyc)$");

            RuleFor(x => x.VerificationStatus)
                .NotEmpty();

            RuleFor(x => x.VerificationStatus)
                .MaximumLength(20);

            RuleFor(x => x.VerificationStatus)
                .Matches(@"^(Pending|InProgress|Verified|Rejected|Expired)$");

            RuleFor(x => x.RejectionReason)
                .MaximumLength(500);

            RuleFor(x => x.PanMasked)
                .MaximumLength(10);

            RuleFor(x => x.AadhaarLast4)
                .MaximumLength(4);

            RuleFor(x => x.AadhaarLast4)
                .Matches(@"^\d{4}$");

            RuleFor(x => x.AadhaarVerificationMethod)
                .MaximumLength(20);

            RuleFor(x => x.AadhaarVerificationMethod)
                .Matches(@"^(OtpEkyc|BiometricEkyc|OfflineXml|VirtualId)$");

            RuleFor(x => x.CkycNumber)
                .MaximumLength(14);

            RuleFor(x => x.CkycStatus)
                .NotEmpty();

            RuleFor(x => x.CkycStatus)
                .MaximumLength(20);

            RuleFor(x => x.CkycStatus)
                .Matches(@"^(NotInitiated|Pending|Uploaded|Verified|Rejected)$");

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.VerifiedByEmployeeId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Pan)
                .MaximumLength(10);

            RuleFor(x => x.Pan)
                .Matches(@"^[A-Z]{5}[0-9]{4}[A-Z]$");
        }
    }
}
