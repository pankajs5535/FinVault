using FluentValidation;
using FinVault.Application.DTOs.CustomerDtos;

namespace FinVault.Application.Validators.Customer
{
    public class DeleteCustomerDtoValidator : AbstractValidator<DeleteCustomerDto>
    {
        public DeleteCustomerDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
