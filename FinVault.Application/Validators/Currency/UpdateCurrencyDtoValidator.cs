using FluentValidation;
using FinVault.Application.DTOs.CurrencyDtos;

namespace FinVault.Application.Validators.Currency
{
    public class UpdateCurrencyDtoValidator : AbstractValidator<UpdateCurrencyDto>
    {
        public UpdateCurrencyDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.CurrencyId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CurrencyCode)
                .NotEmpty();

            RuleFor(x => x.CurrencyCode)
                .MaximumLength(3);

            RuleFor(x => x.NumericCode)
                .NotEmpty();

            RuleFor(x => x.NumericCode)
                .MaximumLength(3);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(100);

            RuleFor(x => x.Symbol)
                .NotEmpty();

            RuleFor(x => x.Symbol)
                .MaximumLength(5);

            RuleFor(x => x.DecimalPlaces)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CountryId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
