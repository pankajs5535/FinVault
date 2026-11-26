using FluentValidation;
using FinVault.Application.DTOs.TransactionTypeDtos;

namespace FinVault.Application.Validators.TransactionType
{
    public class UpdateTransactionTypeDtoValidator : AbstractValidator<UpdateTransactionTypeDto>
    {
        public UpdateTransactionTypeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.TransactionTypeId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.Code)
                .NotEmpty();

            RuleFor(x => x.Code)
                .MaximumLength(30);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .MaximumLength(100);

            RuleFor(x => x.Category)
                .NotEmpty();

            RuleFor(x => x.Category)
                .MaximumLength(30);

            RuleFor(x => x.Category)
                .Matches(@"^(Deposit|Withdrawal|Transfer|Payment|Fee|Interest|Refund|Reversal|Loan|Card)$");

            RuleFor(x => x.Description)
                .MaximumLength(250);

            RuleFor(x => x.ApprovalThreshold)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.DailyLimit)
                .GreaterThanOrEqualTo(0);
        }
    }
}
