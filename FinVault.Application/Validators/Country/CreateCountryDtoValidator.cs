using FluentValidation;
using FinVault.Application.DTOs.CountryDtos;

namespace FinVault.Application.Validators.Country
{
    public class CreateCountryDtoValidator : AbstractValidator<CreateCountryDto>
    {
        public CreateCountryDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.IsoAlpha2Code)
                .NotEmpty();

            RuleFor(x => x.IsoAlpha2Code)
                .MaximumLength(2);

            RuleFor(x => x.IsoAlpha3Code)
                .NotEmpty();

            RuleFor(x => x.IsoAlpha3Code)
                .MaximumLength(3);

            RuleFor(x => x.NumericCode)
                .NotEmpty();

            RuleFor(x => x.NumericCode)
                .MaximumLength(3);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(100);

            RuleFor(x => x.PhoneCode)
                .MaximumLength(6);
        }
    }
}
