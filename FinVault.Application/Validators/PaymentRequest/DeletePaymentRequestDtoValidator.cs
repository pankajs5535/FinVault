using FluentValidation;
using FinVault.Application.DTOs.PaymentRequestDtos;

namespace FinVault.Application.Validators.PaymentRequest
{
    public class DeletePaymentRequestDtoValidator : AbstractValidator<DeletePaymentRequestDto>
    {
        public DeletePaymentRequestDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.PaymentRequestId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
