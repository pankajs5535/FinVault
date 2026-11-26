using FluentValidation;
using FinVault.Application.DTOs.ReconciliationBatchDtos;

namespace FinVault.Application.Validators.ReconciliationBatch
{
    public class DeleteReconciliationBatchDtoValidator : AbstractValidator<DeleteReconciliationBatchDto>
    {
        public DeleteReconciliationBatchDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.ReconciliationBatchId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
