using FluentValidation;
using FinVault.Application.DTOs.PaymentGatewayLogDtos;

namespace FinVault.Application.Validators.PaymentGatewayLog
{
    public class UpdatePaymentGatewayLogDtoValidator : AbstractValidator<UpdatePaymentGatewayLogDto>
    {
        public UpdatePaymentGatewayLogDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.PaymentGatewayLogId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
