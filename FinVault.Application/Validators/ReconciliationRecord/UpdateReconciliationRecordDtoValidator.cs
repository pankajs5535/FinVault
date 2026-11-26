using FluentValidation;
using FinVault.Application.DTOs.ReconciliationRecordDtos;

namespace FinVault.Application.Validators.ReconciliationRecord
{
    public class UpdateReconciliationRecordDtoValidator : AbstractValidator<UpdateReconciliationRecordDto>
    {
        public UpdateReconciliationRecordDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.ReconciliationRecordId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ExternalReference)
                .NotEmpty();

            RuleFor(x => x.ExternalReference)
                .MaximumLength(50);

            RuleFor(x => x.ExternalAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.InternalAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MatchStatus)
                .NotEmpty();

            RuleFor(x => x.MatchStatus)
                .MaximumLength(20);

            RuleFor(x => x.MatchStatus)
                .Matches(@"^(Matched|Unmatched|AmountMismatch|MissingInBank|MissingInExternal|Manual)$");

            RuleFor(x => x.ExceptionReason)
                .MaximumLength(250);

            RuleFor(x => x.ResolutionNotes)
                .MaximumLength(500);

            RuleFor(x => x.ReconciliationBatchId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.TransactionId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ResolvedByUserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
