using FluentValidation;
using FinVault.Application.DTOs.DocumentDtos;

namespace FinVault.Application.Validators.Document
{
    public class CreateDocumentDtoValidator : AbstractValidator<CreateDocumentDto>
    {
        public CreateDocumentDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.DocumentType)
                .NotEmpty();

            RuleFor(x => x.DocumentType)
                .MaximumLength(30);

            RuleFor(x => x.DocumentType)
                .Matches(@"^(Pan|Aadhaar|Passport|VoterId|DrivingLicence|Photo|Signature|AddressProof|IncomeProof|Form60|LoanAgreement|Other)$");

            RuleFor(x => x.DocumentNumberMasked)
                .MaximumLength(30);

            RuleFor(x => x.FileName)
                .NotEmpty();

            RuleFor(x => x.FileName)
                .MaximumLength(260);

            RuleFor(x => x.ContentType)
                .NotEmpty();

            RuleFor(x => x.ContentType)
                .MaximumLength(100);

            RuleFor(x => x.FileSizeBytes)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.VerificationStatus)
                .NotEmpty();

            RuleFor(x => x.VerificationStatus)
                .MaximumLength(20);

            RuleFor(x => x.VerificationStatus)
                .Matches(@"^(Pending|Verified|Rejected|Expired)$");

            RuleFor(x => x.RejectionReason)
                .MaximumLength(500);

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.KYCDetailsId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.LoanId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.VerifiedByEmployeeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
