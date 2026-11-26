using FluentValidation;
using FinVault.Application.DTOs.AddressDtos;

namespace FinVault.Application.Validators.Address
{
    public class DeleteAddressDtoValidator : AbstractValidator<DeleteAddressDto>
    {
        public DeleteAddressDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AddressId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
