using FluentValidation;
using FinVault.Application.DTOs.RefundDtos;

namespace FinVault.Application.Validators.Refund
{
    public class DeleteRefundDtoValidator : AbstractValidator<DeleteRefundDto>
    {
        public DeleteRefundDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.RefundId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
