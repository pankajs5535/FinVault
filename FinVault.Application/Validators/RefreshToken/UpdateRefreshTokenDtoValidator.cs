using FluentValidation;
using FinVault.Application.DTOs.RefreshTokenDtos;

namespace FinVault.Application.Validators.RefreshToken
{
    public class UpdateRefreshTokenDtoValidator : AbstractValidator<UpdateRefreshTokenDto>
    {
        public UpdateRefreshTokenDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.RefreshTokenId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.JwtId)
                .NotEmpty();

            RuleFor(x => x.JwtId)
                .MaximumLength(64);

            RuleFor(x => x.CreatedByIp)
                .NotEmpty();

            RuleFor(x => x.CreatedByIp)
                .MaximumLength(45);

            RuleFor(x => x.UserAgent)
                .MaximumLength(512);

            RuleFor(x => x.DeviceName)
                .MaximumLength(100);

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
