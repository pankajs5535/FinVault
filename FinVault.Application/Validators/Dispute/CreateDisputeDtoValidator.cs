using FluentValidation;
using FinVault.Application.DTOs.DisputeDtos;

namespace FinVault.Application.Validators.Dispute
{
    public class CreateDisputeDtoValidator : AbstractValidator<CreateDisputeDto>
    {
        public CreateDisputeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.DisputeNumber)
                .NotEmpty();

            RuleFor(x => x.DisputeNumber)
                .MaximumLength(30);

            RuleFor(x => x.DisputeType)
                .NotEmpty();

            RuleFor(x => x.DisputeType)
                .MaximumLength(40);

            RuleFor(x => x.DisputeType)
                .Matches(@"^(UnauthorizedTransaction|DuplicateCharge|WrongAmount|ServiceNotReceived|AtmCashNotDispensed|FailedTransactionDebited|Other)$");

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.DisputedAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(30);

            RuleFor(x => x.Status)
                .Matches(@"^(Open|UnderInvestigation|ProvisionalCreditGiven|Resolved|Rejected|Escalated|Closed)$");

            RuleFor(x => x.Priority)
                .NotEmpty();

            RuleFor(x => x.Priority)
                .MaximumLength(10);

            RuleFor(x => x.Priority)
                .Matches(@"^(Low|Medium|High|Critical)$");

            RuleFor(x => x.Resolution)
                .MaximumLength(30);

            RuleFor(x => x.Resolution)
                .Matches(@"^(InFavourOfCustomer|InFavourOfBank|Withdrawn)$");

            RuleFor(x => x.ResolutionNotes)
                .MaximumLength(1000);

            RuleFor(x => x.TransactionId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CardTransactionId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AssignedToEmployeeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
