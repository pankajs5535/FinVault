using FluentValidation;
using FinVault.Application.DTOs.NotificationTemplateDtos;

namespace FinVault.Application.Validators.NotificationTemplate
{
    public class UpdateNotificationTemplateDtoValidator : AbstractValidator<UpdateNotificationTemplateDto>
    {
        public UpdateNotificationTemplateDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.NotificationTemplateId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.TemplateCode)
                .NotEmpty();

            RuleFor(x => x.TemplateCode)
                .MaximumLength(50);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(100);

            RuleFor(x => x.Channel)
                .NotEmpty();

            RuleFor(x => x.Channel)
                .MaximumLength(15);

            RuleFor(x => x.Channel)
                .Matches(@"^(Sms|Email|Push|InApp|WhatsApp)$");

            RuleFor(x => x.Language)
                .NotEmpty();

            RuleFor(x => x.Language)
                .MaximumLength(10);

            RuleFor(x => x.Category)
                .NotEmpty();

            RuleFor(x => x.Category)
                .MaximumLength(20);

            RuleFor(x => x.Category)
                .Matches(@"^(Transactional|Security|Promotional|Regulatory|Reminder)$");

            RuleFor(x => x.Subject)
                .MaximumLength(200);

            RuleFor(x => x.BodyTemplate)
                .NotEmpty();

            RuleFor(x => x.DltTemplateId)
                .MaximumLength(30);

            RuleFor(x => x.Version)
                .GreaterThanOrEqualTo(0);
        }
    }
}
