using FluentValidation;
using FinVault.Application.DTOs.AIUsageLogDtos;

namespace FinVault.Application.Validators.AIUsageLog
{
    public class UpdateAIUsageLogDtoValidator : AbstractValidator<UpdateAIUsageLogDto>
    {
        public UpdateAIUsageLogDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AIUsageLogId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
