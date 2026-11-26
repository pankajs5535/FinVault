using FluentValidation;
using FinVault.Application.DTOs.KYCDetailsDtos;

namespace FinVault.Application.Validators.KYCDetails
{
    public class DeleteKYCDetailsDtoValidator : AbstractValidator<DeleteKYCDetailsDto>
    {
        public DeleteKYCDetailsDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.KYCDetailsId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
