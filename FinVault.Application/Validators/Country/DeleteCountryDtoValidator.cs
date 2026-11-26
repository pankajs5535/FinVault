using FluentValidation;
using FinVault.Application.DTOs.CountryDtos;

namespace FinVault.Application.Validators.Country
{
    public class DeleteCountryDtoValidator : AbstractValidator<DeleteCountryDto>
    {
        public DeleteCountryDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.CountryId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
