using FluentValidation;
using FinVault.Application.DTOs.NotificationDtos;

namespace FinVault.Application.Validators.Notification
{
    public class DeleteNotificationDtoValidator : AbstractValidator<DeleteNotificationDto>
    {
        public DeleteNotificationDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.NotificationId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
