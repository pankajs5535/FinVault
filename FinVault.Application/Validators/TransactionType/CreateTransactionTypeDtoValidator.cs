using FluentValidation;
using FinVault.Application.DTOs.TransactionTypeDtos;

namespace FinVault.Application.Validators.TransactionType
{
    public class CreateTransactionTypeDtoValidator : AbstractValidator<CreateTransactionTypeDto>
    {
        public CreateTransactionTypeDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
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
