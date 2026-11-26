using FluentValidation;
using FinVault.Application.DTOs.AccountDtos;

namespace FinVault.Application.Validators.Account
{
    public class UpdateAccountDtoValidator : AbstractValidator<UpdateAccountDto>
    {
        public UpdateAccountDtoValidator()
        {
            // Validation rules derived from the DTO data annotations.
            RuleFor(x => x.AccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.AccountNumber)
                .NotEmpty();

            RuleFor(x => x.AccountNumber)
                .MaximumLength(18);

            RuleFor(x => x.AccountNumber)
                .Matches(@"^\d{9,18}$");

            RuleFor(x => x.Status)
                .NotEmpty();

            RuleFor(x => x.Status)
                .MaximumLength(20);

            RuleFor(x => x.Status)
                .Matches(@"^(Pending|Active|Dormant|Inactive|Frozen|Closed)$");

            RuleFor(x => x.StatusReason)
                .MaximumLength(250);

            RuleFor(x => x.ModeOfOperation)
                .NotEmpty();

            RuleFor(x => x.ModeOfOperation)
                .MaximumLength(30);

            RuleFor(x => x.ModeOfOperation)
                .Matches(@"^(Single|Joint|EitherOrSurvivor|FormerOrSurvivor)$");

            RuleFor(x => x.OverdraftLimit)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.NomineeName)
                .MaximumLength(150);

            RuleFor(x => x.NomineeRelationship)
                .MaximumLength(50);

            RuleFor(x => x.PrincipalAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MaturityAmount)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.ContractedInterestRate)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.TenureMonths)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.AccountTypeId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.BranchId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.CurrencyId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PayoutAccountId)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.RowVersion)
                .NotEmpty();
        }
    }
}
