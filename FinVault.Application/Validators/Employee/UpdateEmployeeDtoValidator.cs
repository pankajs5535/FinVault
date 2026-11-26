using FluentValidation;
using FinVault.Application.DTOs.EmployeeDtos;

namespace FinVault.Application.Validators.Employee
{
    public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
    {
        public UpdateEmployeeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.EmployeeId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.EmployeeCode)
                .NotEmpty();

            RuleFor(x => x.EmployeeCode)
                .MaximumLength(20);

            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.FirstName)
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .MaximumLength(100);

            RuleFor(x => x.Designation)
                .NotEmpty();

            RuleFor(x => x.Designation)
                .MaximumLength(100);

            RuleFor(x => x.Department)
                .NotEmpty();

            RuleFor(x => x.Department)
                .MaximumLength(100);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(15);

            RuleFor(x => x.PhoneNumber)
                .Must(value => string.IsNullOrWhiteSpace(value) || new System.ComponentModel.DataAnnotations.PhoneAttribute().IsValid(value))
                .WithMessage("PhoneNumber must be a valid phone number.");

            RuleFor(x => x.EmploymentStatus)
                .NotEmpty();

            RuleFor(x => x.EmploymentStatus)
                .MaximumLength(20);

            RuleFor(x => x.EmploymentStatus)
                .Matches(@"^(Active|OnLeave|Suspended|Resigned|Terminated)$");

            RuleFor(x => x.ApprovalLimit)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.BranchId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ReportsToEmployeeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
