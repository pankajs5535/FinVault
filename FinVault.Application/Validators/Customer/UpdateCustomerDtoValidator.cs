using FluentValidation;
using FinVault.Application.DTOs.CustomerDtos;

namespace FinVault.Application.Validators.Customer
{
    public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
    {
        public UpdateCustomerDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CustomerNumber)
                .NotEmpty();

            RuleFor(x => x.CustomerNumber)
                .MaximumLength(20);

            RuleFor(x => x.CustomerType)
                .NotEmpty();

            RuleFor(x => x.CustomerType)
                .MaximumLength(20);

            RuleFor(x => x.CustomerType)
                .Matches(@"^(Individual|Minor|Business)$");

            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.FirstName)
                .MaximumLength(100);

            RuleFor(x => x.MiddleName)
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .MaximumLength(100);

            RuleFor(x => x.Gender)
                .MaximumLength(10);

            RuleFor(x => x.Gender)
                .Matches(@"^(Male|Female|Other)$");

            RuleFor(x => x.FatherOrSpouseName)
                .MaximumLength(200);

            RuleFor(x => x.MobileNumber)
                .NotEmpty();

            RuleFor(x => x.MobileNumber)
                .MaximumLength(15);

            RuleFor(x => x.MobileNumber)
                .Must(value => string.IsNullOrWhiteSpace(value) || new System.ComponentModel.DataAnnotations.PhoneAttribute().IsValid(value))
                .WithMessage("MobileNumber must be a valid phone number.");

            RuleFor(x => x.AlternateMobileNumber)
                .MaximumLength(15);

            RuleFor(x => x.AlternateMobileNumber)
                .Must(value => string.IsNullOrWhiteSpace(value) || new System.ComponentModel.DataAnnotations.PhoneAttribute().IsValid(value))
                .WithMessage("AlternateMobileNumber must be a valid phone number.");

            RuleFor(x => x.Email)
                .MaximumLength(256);

            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.Occupation)
                .MaximumLength(100);

            RuleFor(x => x.AnnualIncome)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.PreferredLanguage)
                .NotEmpty();

            RuleFor(x => x.PreferredLanguage)
                .MaximumLength(10);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Active|Inactive|Dormant|Blocked|Closed)$");

            RuleFor(x => x.RiskCategory)
                .NotEmpty();

            RuleFor(x => x.RiskCategory)
                .MaximumLength(10);

            RuleFor(x => x.RiskCategory)
                .Matches(@"^(Low|Medium|High)$");

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.HomeBranchId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RelationshipManagerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.NationalityCountryId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RowVersion)
                .NotEmpty();
        }
    }
}
