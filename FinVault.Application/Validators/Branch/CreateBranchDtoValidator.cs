using FluentValidation;
using FinVault.Application.DTOs.BranchDtos;

namespace FinVault.Application.Validators.Branch
{
    public class CreateBranchDtoValidator : AbstractValidator<CreateBranchDto>
    {
        public CreateBranchDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.BranchCode)
                .NotEmpty();

            RuleFor(x => x.BranchCode)
                .MaximumLength(10);

            RuleFor(x => x.BranchName)
                .NotEmpty();

            RuleFor(x => x.BranchName)
                .MaximumLength(150);

            RuleFor(x => x.IfscCode)
                .NotEmpty();

            RuleFor(x => x.IfscCode)
                .MaximumLength(11);

            RuleFor(x => x.IfscCode)
                .Matches(@"^[A-Z]{4}0[A-Z0-9]{6}$");

            RuleFor(x => x.MicrCode)
                .MaximumLength(9);

            RuleFor(x => x.BranchType)
                .NotEmpty();

            RuleFor(x => x.BranchType)
                .MaximumLength(20);

            RuleFor(x => x.BranchType)
                .Matches(@"^(Regular|Main|Digital|Service)$");

            RuleFor(x => x.AddressLine1)
                .NotEmpty();

            RuleFor(x => x.AddressLine1)
                .MaximumLength(200);

            RuleFor(x => x.AddressLine2)
                .MaximumLength(200);

            RuleFor(x => x.City)
                .NotEmpty();

            RuleFor(x => x.City)
                .MaximumLength(100);

            RuleFor(x => x.District)
                .NotEmpty();

            RuleFor(x => x.District)
                .MaximumLength(100);

            RuleFor(x => x.State)
                .NotEmpty();

            RuleFor(x => x.State)
                .MaximumLength(100);

            RuleFor(x => x.PinCode)
                .NotEmpty();

            RuleFor(x => x.PinCode)
                .MaximumLength(6);

            RuleFor(x => x.PinCode)
                .Matches(@"^\d{6}$");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15);

            RuleFor(x => x.PhoneNumber)
                .Must(value => string.IsNullOrWhiteSpace(value) || new System.ComponentModel.DataAnnotations.PhoneAttribute().IsValid(value))
                .WithMessage("PhoneNumber must be a valid phone number.");

            RuleFor(x => x.Email)
                .MaximumLength(256);

            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.CountryId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ParentBranchId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ManagerEmployeeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
