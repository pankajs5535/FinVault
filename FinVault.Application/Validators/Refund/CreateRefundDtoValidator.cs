using FluentValidation;
using FinVault.Application.DTOs.RefundDtos;

namespace FinVault.Application.Validators.Refund
{
    public class CreateRefundDtoValidator : AbstractValidator<CreateRefundDto>
    {
        public CreateRefundDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.RefundReference)
                .NotEmpty();

            RuleFor(x => x.RefundReference)
                .MaximumLength(40);

            RuleFor(x => x.RefundType)
                .NotEmpty();

            RuleFor(x => x.RefundType)
                .MaximumLength(10);

            RuleFor(x => x.RefundType)
                .Matches(@"^(Full|Partial)$");

            RuleFor(x => x.Amount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Reason)
                .NotEmpty();

            RuleFor(x => x.Reason)
                .MaximumLength(250);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Requested|Approved|Rejected|Processed|Failed)$");

            RuleFor(x => x.RejectionReason)
                .MaximumLength(250);

            RuleFor(x => x.OriginalTransactionId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RefundTransactionId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.DisputeId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RequestedByUserId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ApprovedByUserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
