using FluentValidation;
using FinVault.Application.DTOs.AIChatMessageDtos;

namespace FinVault.Application.Validators.AIChatMessage
{
    public class UpdateAIChatMessageDtoValidator : AbstractValidator<UpdateAIChatMessageDto>
    {
        public UpdateAIChatMessageDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AIChatMessageId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.SequenceNumber)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.SenderRole)
                .NotEmpty();

            RuleFor(x => x.SenderRole)
                .MaximumLength(10);

            RuleFor(x => x.SenderRole)
                .Matches(@"^(System|User|Assistant|Tool)$");

            RuleFor(x => x.Content)
                .NotEmpty();

            RuleFor(x => x.ModelName)
                .MaximumLength(100);

            RuleFor(x => x.InputTokens)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.OutputTokens)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.LatencyMs)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.FinishReason)
                .MaximumLength(30);

            RuleFor(x => x.FlagReason)
                .MaximumLength(250);

            RuleFor(x => x.AIConversationId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
