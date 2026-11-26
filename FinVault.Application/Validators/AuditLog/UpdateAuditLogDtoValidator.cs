using FluentValidation;
using FinVault.Application.DTOs.AuditLogDtos;

namespace FinVault.Application.Validators.AuditLog
{
    public class UpdateAuditLogDtoValidator : AbstractValidator<UpdateAuditLogDto>
    {
        public UpdateAuditLogDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AuditLogId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
