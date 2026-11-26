using FluentValidation;
using FinVault.Application.DTOs.AIUsageLogDtos;

namespace FinVault.Application.Validators.AIUsageLog
{
    public class CreateAIUsageLogDtoValidator : AbstractValidator<CreateAIUsageLogDto>
    {
        public CreateAIUsageLogDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.FeatureName)
                .NotEmpty();

            RuleFor(x => x.FeatureName)
                .MaximumLength(100);

            RuleFor(x => x.Provider)
                .NotEmpty();

            RuleFor(x => x.Provider)
                .MaximumLength(50);

            RuleFor(x => x.ModelName)
                .NotEmpty();

            RuleFor(x => x.ModelName)
                .MaximumLength(100);

            RuleFor(x => x.RequestId)
                .MaximumLength(64);

            RuleFor(x => x.ErrorCode)
                .MaximumLength(50);

            RuleFor(x => x.LatencyMs)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.InputTokens)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.OutputTokens)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.TotalTokens)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.EstimatedCostInr)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.UserId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AIConversationId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AIChatMessageId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
