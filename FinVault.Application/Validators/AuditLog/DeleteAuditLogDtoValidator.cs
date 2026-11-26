using FluentValidation;
using FinVault.Application.DTOs.AuditLogDtos;

namespace FinVault.Application.Validators.AuditLog
{
    public class DeleteAuditLogDtoValidator : AbstractValidator<DeleteAuditLogDto>
    {
        public DeleteAuditLogDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AuditLogId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
