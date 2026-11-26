using FluentValidation;
using FinVault.Application.DTOs.AIConversationDtos;

namespace FinVault.Application.Validators.AIConversation
{
    public class DeleteAIConversationDtoValidator : AbstractValidator<DeleteAIConversationDto>
    {
        public DeleteAIConversationDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AIConversationId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
