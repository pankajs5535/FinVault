using FluentValidation;
using FinVault.Application.DTOs.LoanDtos;

namespace FinVault.Application.Validators.Loan
{
    public class CreateLoanDtoValidator : AbstractValidator<CreateLoanDto>
    {
        public CreateLoanDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.LoanNumber)
                .NotEmpty();

            RuleFor(x => x.LoanNumber)
                .MaximumLength(20);

            RuleFor(x => x.LoanType)
                .NotEmpty();

            RuleFor(x => x.LoanType)
                .MaximumLength(20);

            RuleFor(x => x.LoanType)
                .Matches(@"^(Personal|Home|Vehicle|Education|Business|Gold|AgainstDeposit)$");

            RuleFor(x => x.Purpose)
                .MaximumLength(250);

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Applied|Approved|Rejected|Disbursed|Active|Closed|WrittenOff)$");

            RuleFor(x => x.RequestedAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.SanctionedAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.EmiAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.AnnualInterestRate)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.InterestType)
                .NotEmpty();

            RuleFor(x => x.InterestType)
                .MaximumLength(10);

            RuleFor(x => x.InterestType)
                .Matches(@"^(Fixed|Floating)$");

            RuleFor(x => x.TenureMonths)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.AssetClassification)
                .NotEmpty();

            RuleFor(x => x.AssetClassification)
                .MaximumLength(15);

            RuleFor(x => x.AssetClassification)
                .Matches(@"^(Standard|SMA0|SMA1|SMA2|SubStandard|Doubtful|Loss)$");

            RuleFor(x => x.CustomerId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.BranchId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.DisbursementAccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RepaymentAccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.InterestRateId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.ApprovedByEmployeeId)
                .GreaterThanOrEqualTo(1);
        }
    }
}
