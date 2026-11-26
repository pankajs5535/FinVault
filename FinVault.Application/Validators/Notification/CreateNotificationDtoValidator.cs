using FluentValidation;
using FinVault.Application.DTOs.NotificationDtos;

namespace FinVault.Application.Validators.Notification
{
    public class CreateNotificationDtoValidator : AbstractValidator<CreateNotificationDto>
    {
        public CreateNotificationDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.Channel)
                .NotEmpty();

            RuleFor(x => x.Channel)
                .MaximumLength(15);

            RuleFor(x => x.Channel)
                .Matches(@"^(Sms|Email|Push|InApp|WhatsApp)$");

            RuleFor(x => x.Priority)
                .NotEmpty();

            RuleFor(x => x.Priority)
                .MaximumLength(10);

            RuleFor(x => x.Priority)
                .Matches(@"^(Low|Normal|High|Urgent)$");

            RuleFor(x => x.RecipientAddress)
                .NotEmpty();

            RuleFor(x => x.RecipientAddress)
                .MaximumLength(256);

            RuleFor(x => x.Subject)
                .MaximumLength(200);

            RuleFor(x => x.Body)
                .NotEmpty();

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(15);

            RuleFor(x => x.Status)
                .Matches(@"^(Pending|Queued|Sent|Delivered|Failed|Read)$");

            RuleFor(x => x.RetryCount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.ProviderMessageId)
                .MaximumLength(100);

            RuleFor(x => x.FailureReason)
                .MaximumLength(500);

            RuleFor(x => x.RelatedEntityType)
                .MaximumLength(50);

            RuleFor(x => x.RelatedEntityId)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.NotificationTemplateId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
