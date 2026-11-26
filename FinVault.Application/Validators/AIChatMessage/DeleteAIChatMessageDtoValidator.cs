using FluentValidation;
using FinVault.Application.DTOs.AIChatMessageDtos;

namespace FinVault.Application.Validators.AIChatMessage
{
    public class DeleteAIChatMessageDtoValidator : AbstractValidator<DeleteAIChatMessageDto>
    {
        public DeleteAIChatMessageDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AIChatMessageId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
