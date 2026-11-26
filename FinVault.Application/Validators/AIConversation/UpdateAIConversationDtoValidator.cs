using FluentValidation;
using FinVault.Application.DTOs.AIConversationDtos;

namespace FinVault.Application.Validators.AIConversation
{
    public class UpdateAIConversationDtoValidator : AbstractValidator<UpdateAIConversationDto>
    {
        public UpdateAIConversationDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AIConversationId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ConversationReference)
                .NotEmpty();

            RuleFor(x => x.ConversationReference)
                .MaximumLength(40);

            RuleFor(x => x.Title)
                .MaximumLength(200);

            RuleFor(x => x.Channel)
                .NotEmpty();

            RuleFor(x => x.Channel)
                .MaximumLength(10);

            RuleFor(x => x.Channel)
                .Matches(@"^(Web|Mobile|Voice)$");

            RuleFor(x => x.Language)
                .NotEmpty();

            RuleFor(x => x.Language)
                .MaximumLength(10);

            RuleFor(x => x.Summary)
                .MaximumLength(1000);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(15);

            RuleFor(x => x.Status)
                .Matches(@"^(Active|Closed|Archived|Escalated)$");

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.EscalatedToEmployeeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
