using FluentValidation;
using FinVault.Application.DTOs.UserDtos;

namespace FinVault.Application.Validators.User
{
    public class DeleteUserDtoValidator : AbstractValidator<DeleteUserDto>
    {
        public DeleteUserDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
