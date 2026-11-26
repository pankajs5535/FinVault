using FluentValidation;
using FinVault.Application.DTOs.BeneficiaryDtos;

namespace FinVault.Application.Validators.Beneficiary
{
    public class DeleteBeneficiaryDtoValidator : AbstractValidator<DeleteBeneficiaryDto>
    {
        public DeleteBeneficiaryDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.BeneficiaryId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
