using FluentValidation;
using FinVault.Application.DTOs.ReconciliationBatchDtos;

namespace FinVault.Application.Validators.ReconciliationBatch
{
    public class CreateReconciliationBatchDtoValidator : AbstractValidator<CreateReconciliationBatchDto>
    {
        public CreateReconciliationBatchDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.BatchNumber)
                .NotEmpty();

            RuleFor(x => x.BatchNumber)
                .MaximumLength(30);

            RuleFor(x => x.ReconciliationType)
                .NotEmpty();

            RuleFor(x => x.ReconciliationType)
                .MaximumLength(20);

            RuleFor(x => x.ReconciliationType)
                .Matches(@"^(Upi|Neft|Rtgs|Imps|CardSettlement|AtmSwitch|GeneralLedger)$");

            RuleFor(x => x.SourceName)
                .NotEmpty();

            RuleFor(x => x.SourceName)
                .MaximumLength(100);

            RuleFor(x => x.FileName)
                .MaximumLength(260);

            RuleFor(x => x.FileHash)
                .MaximumLength(64);

            RuleFor(x => x.TotalRecords)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MatchedRecords)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.UnmatchedRecords)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.TotalAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MatchedAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(30);

            RuleFor(x => x.Status)
                .Matches(@"^(Pending|InProgress|Completed|CompletedWithExceptions|Failed)$");

            RuleFor(x => x.PaymentMethodId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.InitiatedByUserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
