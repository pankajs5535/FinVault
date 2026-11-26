using FluentValidation;
using FinVault.Application.DTOs.BeneficiaryDtos;

namespace FinVault.Application.Validators.Beneficiary
{
    public class CreateBeneficiaryDtoValidator : AbstractValidator<CreateBeneficiaryDto>
    {
        public CreateBeneficiaryDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.BeneficiaryName)
                .NotEmpty();

            RuleFor(x => x.BeneficiaryName)
                .MaximumLength(150);

            RuleFor(x => x.Nickname)
                .MaximumLength(50);

            RuleFor(x => x.BeneficiaryType)
                .NotEmpty();

            RuleFor(x => x.BeneficiaryType)
                .MaximumLength(20);

            RuleFor(x => x.BeneficiaryType)
                .Matches(@"^(IntraBank|InterBank|UPI)$");

            RuleFor(x => x.AccountNumber)
                .MaximumLength(20);

            RuleFor(x => x.IfscCode)
                .MaximumLength(11);

            RuleFor(x => x.IfscCode)
                .Matches(@"^[A-Z]{4}0[A-Z0-9]{6}$");

            RuleFor(x => x.BankName)
                .MaximumLength(150);

            RuleFor(x => x.BranchName)
                .MaximumLength(150);

            RuleFor(x => x.UpiVpa)
                .MaximumLength(100);

            RuleFor(x => x.MobileNumber)
                .MaximumLength(15);

            RuleFor(x => x.MobileNumber)
                .Must(value => string.IsNullOrWhiteSpace(value) || new System.ComponentModel.DataAnnotations.PhoneAttribute().IsValid(value))
                .WithMessage("MobileNumber must be a valid phone number.");

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Pending|Active|Blocked|Removed)$");

            RuleFor(x => x.PerTransactionLimit)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.InternalAccountId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
