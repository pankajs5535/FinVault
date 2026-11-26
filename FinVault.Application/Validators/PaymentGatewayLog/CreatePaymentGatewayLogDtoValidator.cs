using FluentValidation;
using FinVault.Application.DTOs.PaymentGatewayLogDtos;

namespace FinVault.Application.Validators.PaymentGatewayLog
{
    public class CreatePaymentGatewayLogDtoValidator : AbstractValidator<CreatePaymentGatewayLogDto>
    {
        public CreatePaymentGatewayLogDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.GatewayName)
                .NotEmpty();

            RuleFor(x => x.GatewayName)
                .MaximumLength(50);

            RuleFor(x => x.Operation)
                .NotEmpty();

            RuleFor(x => x.Operation)
                .MaximumLength(50);

            RuleFor(x => x.Operation)
                .Matches(@"^(Initiate|StatusCheck|Callback|Reversal|Refund)$");

            RuleFor(x => x.CorrelationId)
                .NotEmpty();

            RuleFor(x => x.CorrelationId)
                .MaximumLength(64);

            RuleFor(x => x.AttemptNumber)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.HttpStatusCode)
                .GreaterThanOrEqualTo(100)
                .LessThanOrEqualTo(599);

            RuleFor(x => x.ResponseCode)
                .MaximumLength(30);

            RuleFor(x => x.ResponseMessage)
                .MaximumLength(500);

            RuleFor(x => x.DurationMs)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.ErrorMessage)
                .MaximumLength(1000);

            RuleFor(x => x.PaymentRequestId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
