using FluentValidation;
using FinVault.Application.DTOs.AddressDtos;

namespace FinVault.Application.Validators.Address
{
    public class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
    {
        public CreateAddressDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AddressType)
                .NotEmpty();

            RuleFor(x => x.AddressType)
                .MaximumLength(20);

            RuleFor(x => x.AddressType)
                .Matches(@"^(Residential|Permanent|Correspondence|Office|Registered)$");

            RuleFor(x => x.AddressLine1)
                .NotEmpty();

            RuleFor(x => x.AddressLine1)
                .MaximumLength(200);

            RuleFor(x => x.AddressLine2)
                .MaximumLength(200);

            RuleFor(x => x.Landmark)
                .MaximumLength(100);

            RuleFor(x => x.City)
                .NotEmpty();

            RuleFor(x => x.City)
                .MaximumLength(100);

            RuleFor(x => x.District)
                .MaximumLength(100);

            RuleFor(x => x.State)
                .NotEmpty();

            RuleFor(x => x.State)
                .MaximumLength(100);

            RuleFor(x => x.PinCode)
                .NotEmpty();

            RuleFor(x => x.PinCode)
                .MaximumLength(6);

            RuleFor(x => x.PinCode)
                .Matches(@"^\d{6}$");

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CountryId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
