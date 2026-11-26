using FluentValidation;
using FinVault.Application.DTOs.RefreshTokenDtos;

namespace FinVault.Application.Validators.RefreshToken
{
    public class DeleteRefreshTokenDtoValidator : AbstractValidator<DeleteRefreshTokenDto>
    {
        public DeleteRefreshTokenDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.RefreshTokenId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
