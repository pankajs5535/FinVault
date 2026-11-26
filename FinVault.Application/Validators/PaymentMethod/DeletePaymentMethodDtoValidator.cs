using FluentValidation;
using FinVault.Application.DTOs.PaymentMethodDtos;

namespace FinVault.Application.Validators.PaymentMethod
{
    public class DeletePaymentMethodDtoValidator : AbstractValidator<DeletePaymentMethodDto>
    {
        public DeletePaymentMethodDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.PaymentMethodId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
