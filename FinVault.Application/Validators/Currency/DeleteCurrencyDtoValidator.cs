using FluentValidation;
using FinVault.Application.DTOs.CurrencyDtos;

namespace FinVault.Application.Validators.Currency
{
    public class DeleteCurrencyDtoValidator : AbstractValidator<DeleteCurrencyDto>
    {
        public DeleteCurrencyDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.CurrencyId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
