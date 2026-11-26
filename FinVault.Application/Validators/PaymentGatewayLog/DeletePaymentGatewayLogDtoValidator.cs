using FluentValidation;
using FinVault.Application.DTOs.PaymentGatewayLogDtos;

namespace FinVault.Application.Validators.PaymentGatewayLog
{
    public class DeletePaymentGatewayLogDtoValidator : AbstractValidator<DeletePaymentGatewayLogDto>
    {
        public DeletePaymentGatewayLogDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.PaymentGatewayLogId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
