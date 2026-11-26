using FluentValidation;
using FinVault.Application.DTOs.DisputeDtos;

namespace FinVault.Application.Validators.Dispute
{
    public class DeleteDisputeDtoValidator : AbstractValidator<DeleteDisputeDto>
    {
        public DeleteDisputeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.DisputeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
