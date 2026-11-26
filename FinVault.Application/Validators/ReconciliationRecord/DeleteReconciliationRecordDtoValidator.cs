using FluentValidation;
using FinVault.Application.DTOs.ReconciliationRecordDtos;

namespace FinVault.Application.Validators.ReconciliationRecord
{
    public class DeleteReconciliationRecordDtoValidator : AbstractValidator<DeleteReconciliationRecordDto>
    {
        public DeleteReconciliationRecordDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.ReconciliationRecordId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
