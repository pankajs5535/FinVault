using FluentValidation;
using FinVault.Application.DTOs.NotificationTemplateDtos;

namespace FinVault.Application.Validators.NotificationTemplate
{
    public class DeleteNotificationTemplateDtoValidator : AbstractValidator<DeleteNotificationTemplateDto>
    {
        public DeleteNotificationTemplateDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.NotificationTemplateId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
