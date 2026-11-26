using FluentValidation;
using FinVault.Application.DTOs.LoanRepaymentDtos;

namespace FinVault.Application.Validators.LoanRepayment
{
    public class UpdateLoanRepaymentDtoValidator : AbstractValidator<UpdateLoanRepaymentDto>
    {
        public UpdateLoanRepaymentDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.LoanRepaymentId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RepaymentReference)
                .NotEmpty();

            RuleFor(x => x.RepaymentReference)
                .MaximumLength(40);

            RuleFor(x => x.RepaymentType)
                .NotEmpty();

            RuleFor(x => x.RepaymentType)
                .MaximumLength(20);

            RuleFor(x => x.RepaymentType)
                .Matches(@"^(Emi|PartPayment|ForeClosure|Penalty)$");

            RuleFor(x => x.PaymentMode)
                .NotEmpty();

            RuleFor(x => x.PaymentMode)
                .MaximumLength(20);

            RuleFor(x => x.PaymentMode)
                .Matches(@"^(AutoDebit|Nach|Online|Branch)$");

            RuleFor(x => x.AmountPaid)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.PrincipalComponent)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.InterestComponent)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.PenaltyComponent)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Pending|Successful|Failed|Reversed)$");

            RuleFor(x => x.FailureReason)
                .MaximumLength(250);

            RuleFor(x => x.LoanId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.LoanScheduleId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.TransactionId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
