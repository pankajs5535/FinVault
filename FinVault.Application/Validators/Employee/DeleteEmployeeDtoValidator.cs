using FluentValidation;
using FinVault.Application.DTOs.EmployeeDtos;

namespace FinVault.Application.Validators.Employee
{
    public class DeleteEmployeeDtoValidator : AbstractValidator<DeleteEmployeeDto>
    {
        public DeleteEmployeeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.EmployeeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
