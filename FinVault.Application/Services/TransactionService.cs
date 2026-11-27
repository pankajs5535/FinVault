using AutoMapper;
using FinVault.Application.DTOs.TransactionDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class TransactionService : ITransactionService
{
    private static readonly string[] ValidStatuses =
    {
        "Initiated", "Pending", "Processing", "Posted",
        "Failed", "Reversed", "Cancelled", "OnHold"
    };

    private static readonly string[] ValidChannels =
    {
        "Branch", "ATM", "NetBanking", "MobileApp",
        "UPI", "POS", "Api", "Batch"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND TRANSACTION IDENTIFICATION

    public async Task<IEnumerable<TransactionDto>> GetAllAsync()
    {
        var transactions = await _unitOfWork.Transaction.GetAllAsync();
        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<TransactionDto?> GetByIdAsync(int transactionId)
    {
        ValidateId(transactionId, nameof(transactionId));

        var transaction = await _unitOfWork.Transaction.GetByIdAsync(transactionId);
        return _mapper.Map<TransactionDto?>(transaction);
    }

    public async Task<TransactionDto?> GetByTransactionReferenceAsync(
        string transactionReference)
    {
        ValidateRequiredString(transactionReference, nameof(transactionReference));

        var transaction = await _unitOfWork.Transaction
            .GetByTransactionReferenceAsync(transactionReference.Trim());

        return _mapper.Map<TransactionDto?>(transaction);
    }

    public async Task<TransactionDto?> GetByIdempotencyKeyAsync(string idempotencyKey)
    {
        ValidateRequiredString(idempotencyKey, nameof(idempotencyKey));

        var transaction = await _unitOfWork.Transaction
            .GetByIdempotencyKeyAsync(idempotencyKey.Trim());

        return _mapper.Map<TransactionDto?>(transaction);
    }

    public async Task<TransactionDto?> GetByExternalReferenceAsync(
        string externalReference)
    {
        ValidateRequiredString(externalReference, nameof(externalReference));

        var transaction = await _unitOfWork.Transaction
            .GetByExternalReferenceAsync(externalReference.Trim());

        return _mapper.Map<TransactionDto?>(transaction);
    }

    public async Task<TransactionDto?> GetTransactionDetailsAsync(int transactionId)
    {
        ValidateId(transactionId, nameof(transactionId));

        var transaction = await _unitOfWork.Transaction
            .GetTransactionDetailsAsync(transactionId);

        return _mapper.Map<TransactionDto?>(transaction);
    }

    public async Task<TransactionDto> CreateAsync(CreateTransactionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateRequiredString(dto.TransactionReference, nameof(dto.TransactionReference));
        ValidateId(dto.TransactionTypeId, nameof(dto.TransactionTypeId));
        ValidateId(dto.CurrencyId, nameof(dto.CurrencyId));

        if (dto.TransactionReference.Length > 40)
            throw new ArgumentException(
                "Transaction reference cannot exceed 40 characters.");

        if (dto.Amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(dto.Amount),
                "Transaction amount must be greater than zero.");

        if (dto.FeeAmount < 0 || dto.TaxAmount < 0)
            throw new ArgumentOutOfRangeException(
                nameof(dto.FeeAmount),
                "Fee and tax amounts cannot be negative.");

        var status = NormalizeValue(dto.Status, ValidStatuses, nameof(dto.Status));
        var channel = NormalizeValue(dto.Channel, ValidChannels, nameof(dto.Channel));

        // Posting and reversal must be performed through dedicated financial workflows.
        if (status != "Initiated")
            throw new InvalidOperationException(
                "New transactions must be created with Initiated status. Posting, reversal and final status changes must use the dedicated transaction-processing workflow.");

        if (dto.TransactionAt == default || dto.ValueDate == default)
            throw new ArgumentException(
                "TransactionAt and ValueDate are required.");

        if (dto.TransactionAt > DateTime.UtcNow.AddMinutes(5))
            throw new ArgumentException(
                "Transaction date cannot be in the future.");

        if (!string.IsNullOrWhiteSpace(dto.IdempotencyKey))
        {
            var existingByKey = await _unitOfWork.Transaction
                .GetByIdempotencyKeyAsync(dto.IdempotencyKey.Trim());

            if (existingByKey != null)
                throw new InvalidOperationException(
                    "A transaction with this idempotency key already exists.");
        }

        var existingReference = await _unitOfWork.Transaction
            .GetByTransactionReferenceAsync(dto.TransactionReference.Trim());

        if (existingReference != null)
            throw new InvalidOperationException(
                "Transaction reference already exists.");

        if (!string.IsNullOrWhiteSpace(dto.ExternalReference))
        {
            var existingExternal = await _unitOfWork.Transaction
                .GetByExternalReferenceAsync(dto.ExternalReference.Trim());

            if (existingExternal != null)
                throw new InvalidOperationException(
                    "External transaction reference already exists.");
        }

        var transactionType = await _unitOfWork.TransactionType
            .GetByIdAsync(dto.TransactionTypeId);

        if (transactionType == null || !transactionType.IsActive)
            throw new InvalidOperationException(
                "The selected transaction type does not exist or is inactive.");

        var currency = await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId);

        if (currency == null || !currency.IsActive)
            throw new InvalidOperationException(
                "The selected currency does not exist or is inactive.");

        await ValidateAccountsAsync(
            dto.SourceAccountId,
            dto.DestinationAccountId,
            dto.CurrencyId,
            transactionType.Category);

        await ValidateFeeAsync(
            dto.FeeId,
            dto.TransactionTypeId,
            dto.FeeAmount,
            dto.TaxAmount);

        await ValidateInitiatorAndApprovalAsync(
            dto.InitiatedByUserId,
            dto.ApprovedByUserId,
            transactionType,
            dto.Amount);

        await ValidateDailyLimitAsync(
            dto.SourceAccountId,
            transactionType,
            dto.Amount);

        await ValidateReversalAsync(
            dto.ReversalOfTransactionId,
            transactionType);

        var transaction = _mapper.Map<Transaction>(dto);

        transaction.TransactionReference = dto.TransactionReference.Trim();
        transaction.IdempotencyKey = string.IsNullOrWhiteSpace(dto.IdempotencyKey)
            ? null
            : dto.IdempotencyKey.Trim();
        transaction.ExternalReference = string.IsNullOrWhiteSpace(dto.ExternalReference)
            ? null
            : dto.ExternalReference.Trim();

        transaction.Status = "Initiated";
        transaction.Channel = channel;
        transaction.SourceBalanceAfter = null;
        transaction.PostedAt = null;
        transaction.FailureReason = null;
        transaction.CreatedAt = DateTime.UtcNow;
        transaction.UpdatedAt = null;

        await _unitOfWork.Transaction.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransactionDto>(transaction);
    }

    public async Task<TransactionDto?> UpdateAsync(UpdateTransactionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.TransactionId, nameof(dto.TransactionId));

        var transaction = await _unitOfWork.Transaction
            .GetByIdAsync(dto.TransactionId);

        if (transaction == null)
            return null;

        if (dto.RowVersion == null ||
            dto.RowVersion.Length == 0 ||
            transaction.RowVersion == null ||
            !transaction.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                "Transaction data has been modified by another operation. Reload the transaction and try again.");
        }

        if (transaction.Status is "Posted" or "Reversed")
            throw new InvalidOperationException(
                "Posted and reversed transactions are immutable.");

        if (transaction.Status is "Failed" or "Cancelled")
            throw new InvalidOperationException(
                "Failed and cancelled transactions cannot be updated.");

        if (dto.Amount <= 0 || dto.FeeAmount < 0 || dto.TaxAmount < 0)
            throw new ArgumentOutOfRangeException(
                nameof(dto.Amount),
                "Transaction amount must be positive and fee/tax amounts cannot be negative.");

        var newStatus = NormalizeValue(dto.Status, ValidStatuses, nameof(dto.Status));
        var newChannel = NormalizeValue(dto.Channel, ValidChannels, nameof(dto.Channel));

        if (newStatus == "Posted" || newStatus == "Reversed")
            throw new InvalidOperationException(
                "Posting and reversal must be performed through the dedicated financial workflow.");

        ValidateStatusTransition(transaction.Status, newStatus);

        ValidateRequiredString(dto.TransactionReference, nameof(dto.TransactionReference));

        var existingReference = await _unitOfWork.Transaction
            .GetByTransactionReferenceAsync(dto.TransactionReference.Trim());

        if (existingReference != null &&
            existingReference.TransactionId != dto.TransactionId)
        {
            throw new InvalidOperationException(
                "Transaction reference already belongs to another transaction.");
        }

        if (!string.IsNullOrWhiteSpace(dto.IdempotencyKey))
        {
            var existingKey = await _unitOfWork.Transaction
                .GetByIdempotencyKeyAsync(dto.IdempotencyKey.Trim());

            if (existingKey != null &&
                existingKey.TransactionId != dto.TransactionId)
            {
                throw new InvalidOperationException(
                    "Idempotency key already belongs to another transaction.");
            }
        }

        var transactionType = await _unitOfWork.TransactionType
            .GetByIdAsync(dto.TransactionTypeId);

        if (transactionType == null || !transactionType.IsActive)
            throw new InvalidOperationException(
                "The selected transaction type does not exist or is inactive.");

        var currency = await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId);

        if (currency == null || !currency.IsActive)
            throw new InvalidOperationException(
                "The selected currency does not exist or is inactive.");

        await ValidateAccountsAsync(
            dto.SourceAccountId,
            dto.DestinationAccountId,
            dto.CurrencyId,
            transactionType.Category);

        await ValidateFeeAsync(
            dto.FeeId,
            dto.TransactionTypeId,
            dto.FeeAmount,
            dto.TaxAmount);

        await ValidateInitiatorAndApprovalAsync(
            dto.InitiatedByUserId,
            dto.ApprovedByUserId,
            transactionType,
            dto.Amount);

        await ValidateDailyLimitAsync(
            dto.SourceAccountId,
            transactionType,
            dto.Amount,
            dto.TransactionId);

        await ValidateReversalAsync(
            dto.ReversalOfTransactionId,
            transactionType,
            dto.TransactionId);

        // A transaction's account/currency/type/amount data is locked
        // once processing has moved beyond the Initiated state.
        if (transaction.Status != "Initiated")
        {
            if (transaction.Amount != dto.Amount ||
                transaction.FeeAmount != dto.FeeAmount ||
                transaction.TaxAmount != dto.TaxAmount ||
                transaction.TransactionTypeId != dto.TransactionTypeId ||
                transaction.SourceAccountId != dto.SourceAccountId ||
                transaction.DestinationAccountId != dto.DestinationAccountId ||
                transaction.CurrencyId != dto.CurrencyId)
            {
                throw new InvalidOperationException(
                    "Financial details cannot be changed after transaction processing has started.");
            }
        }

        var originalId = transaction.TransactionId;
        var originalRowVersion = transaction.RowVersion;
        var originalCreatedAt = transaction.CreatedAt;
        var originalCreatedBy = transaction.CreatedBy;
        var originalPostedAt = transaction.PostedAt;
        var originalSourceBalanceAfter = transaction.SourceBalanceAfter;

        _mapper.Map(dto, transaction);

        transaction.TransactionId = originalId;
        transaction.RowVersion = originalRowVersion;
        transaction.CreatedAt = originalCreatedAt;
        transaction.CreatedBy = originalCreatedBy;
        transaction.PostedAt = originalPostedAt;
        transaction.SourceBalanceAfter = originalSourceBalanceAfter;
        transaction.TransactionReference = dto.TransactionReference.Trim();
        transaction.IdempotencyKey = string.IsNullOrWhiteSpace(dto.IdempotencyKey)
            ? null
            : dto.IdempotencyKey.Trim();
        transaction.ExternalReference = string.IsNullOrWhiteSpace(dto.ExternalReference)
            ? null
            : dto.ExternalReference.Trim();
        transaction.Status = newStatus;
        transaction.Channel = newChannel;
        transaction.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Transaction.Update(transaction);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<TransactionDto>(transaction);
    }

    public async Task<bool> DeleteAsync(DeleteTransactionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.TransactionId, nameof(dto.TransactionId));

        var transaction = await _unitOfWork.Transaction
            .GetByIdAsync(dto.TransactionId);

        if (transaction == null)
            return false;

        if (transaction.Status == "Posted" ||
            transaction.Status == "Reversed")
        {
            throw new InvalidOperationException(
                "Posted or reversed financial transactions cannot be deleted. Use the reversal workflow where applicable.");
        }

        if (transaction.Status == "Cancelled")
            return true;

        if (transaction.Status == "Processing")
            throw new InvalidOperationException(
                "A transaction currently being processed cannot be cancelled.");

        transaction.Status = "Cancelled";
        transaction.FailureReason = "Cancelled through transaction service.";
        transaction.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Transaction.Update(transaction);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsByTransactionReferenceAsync(
        string transactionReference)
    {
        ValidateRequiredString(transactionReference, nameof(transactionReference));

        return await _unitOfWork.Transaction
            .ExistsByTransactionReferenceAsync(transactionReference.Trim());
    }

    public async Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey)
    {
        ValidateRequiredString(idempotencyKey, nameof(idempotencyKey));

        return await _unitOfWork.Transaction
            .ExistsByIdempotencyKeyAsync(idempotencyKey.Trim());
    }

    // 2. ACCOUNT TRANSACTION HISTORY

    public async Task<IEnumerable<TransactionDto>> GetBySourceAccountIdAsync(
        int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var transactions = await _unitOfWork.Transaction
            .GetBySourceAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetByDestinationAccountIdAsync(
        int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var transactions = await _unitOfWork.Transaction
            .GetByDestinationAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetAccountTransactionHistoryAsync(
        int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var transactions = await _unitOfWork.Transaction
            .GetAccountTransactionHistoryAsync(accountId);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetByTransactionTypeIdAsync(
        int transactionTypeId)
    {
        ValidateId(transactionTypeId, nameof(transactionTypeId));

        var transactions = await _unitOfWork.Transaction
            .GetByTransactionTypeIdAsync(transactionTypeId);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetByCurrencyIdAsync(int currencyId)
    {
        ValidateId(currencyId, nameof(currencyId));

        var transactions = await _unitOfWork.Transaction
            .GetByCurrencyIdAsync(currencyId);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    // 3. TRANSACTION STATUS AND CHANNEL

    public async Task<IEnumerable<TransactionDto>> GetByStatusAsync(string status)
    {
        var normalizedStatus = NormalizeValue(
            status,
            ValidStatuses,
            nameof(status));

        var transactions = await _unitOfWork.Transaction
            .GetByStatusAsync(normalizedStatus);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetByChannelAsync(string channel)
    {
        var normalizedChannel = NormalizeValue(
            channel,
            ValidChannels,
            nameof(channel));

        var transactions = await _unitOfWork.Transaction
            .GetByChannelAsync(normalizedChannel);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetPostedTransactionsAsync()
    {
        var transactions = await _unitOfWork.Transaction
            .GetPostedTransactionsAsync();

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetPendingTransactionsAsync()
    {
        var transactions = await _unitOfWork.Transaction
            .GetPendingTransactionsAsync();

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetOnHoldTransactionsAsync()
    {
        var transactions = await _unitOfWork.Transaction
            .GetOnHoldTransactionsAsync();

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetFailedTransactionsAsync()
    {
        var transactions = await _unitOfWork.Transaction
            .GetFailedTransactionsAsync();

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    // 4. DATE-BASED TRANSACTION HISTORY

    public async Task<IEnumerable<TransactionDto>> GetTransactionsWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var transactions = await _unitOfWork.Transaction
            .GetTransactionsWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>>
        GetPostedTransactionsWithinRangeAsync(
            DateTime startDate,
            DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var transactions = await _unitOfWork.Transaction
            .GetPostedTransactionsWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetTransactionsByValueDateAsync(
        DateTime valueDate)
    {
        ValidateDate(valueDate, nameof(valueDate));

        var transactions = await _unitOfWork.Transaction
            .GetTransactionsByValueDateAsync(valueDate.Date);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetRecentTransactionsByAccountIdAsync(
        int accountId,
        int count)
    {
        ValidateId(accountId, nameof(accountId));

        if (count <= 0 || count > 200)
            throw new ArgumentOutOfRangeException(
                nameof(count),
                "Count must be between 1 and 200.");

        var transactions = await _unitOfWork.Transaction
            .GetRecentTransactionsByAccountIdAsync(accountId, count);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    // 5. APPROVAL AND REVERSAL TRACKING

    public async Task<IEnumerable<TransactionDto>> GetByInitiatedByUserIdAsync(
        int userId)
    {
        ValidateId(userId, nameof(userId));

        var transactions = await _unitOfWork.Transaction
            .GetByInitiatedByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetByApprovedByUserIdAsync(
        int userId)
    {
        ValidateId(userId, nameof(userId));

        var transactions = await _unitOfWork.Transaction
            .GetByApprovedByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>> GetReversalTransactionsAsync(
        int originalTransactionId)
    {
        ValidateId(originalTransactionId, nameof(originalTransactionId));

        var transactions = await _unitOfWork.Transaction
            .GetReversalTransactionsAsync(originalTransactionId);

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>>
        GetTransactionsWithOriginalTransactionAsync()
    {
        var transactions = await _unitOfWork.Transaction
            .GetTransactionsWithOriginalTransactionAsync();

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    // 6. JOURNAL AND RELATED BANKING RECORDS

    public async Task<TransactionDto?> GetWithJournalEntriesAsync(int transactionId)
    {
        ValidateId(transactionId, nameof(transactionId));

        var transaction = await _unitOfWork.Transaction
            .GetWithJournalEntriesAsync(transactionId);

        return _mapper.Map<TransactionDto?>(transaction);
    }

    public async Task<TransactionDto?> GetWithPaymentRequestAsync(int transactionId)
    {
        ValidateId(transactionId, nameof(transactionId));

        var transaction = await _unitOfWork.Transaction
            .GetWithPaymentRequestAsync(transactionId);

        return _mapper.Map<TransactionDto?>(transaction);
    }

    public async Task<IEnumerable<TransactionDto>>
        GetTransactionsWithCardTransactionsAsync()
    {
        var transactions = await _unitOfWork.Transaction
            .GetTransactionsWithCardTransactionsAsync();

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<IEnumerable<TransactionDto>>
        GetTransactionsWithLoanRepaymentsAsync()
    {
        var transactions = await _unitOfWork.Transaction
            .GetTransactionsWithLoanRepaymentsAsync();

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    // 7. FINANCIAL REPORTING AND SEARCH

    public async Task<decimal> GetTotalPostedOutgoingAmountAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        return await _unitOfWork.Transaction
            .GetTotalPostedOutgoingAmountAsync(accountId);
    }

    public async Task<decimal> GetTotalPostedIncomingAmountAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        return await _unitOfWork.Transaction
            .GetTotalPostedIncomingAmountAsync(accountId);
    }

    public async Task<decimal> GetTotalPostedAmountWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        return await _unitOfWork.Transaction
            .GetTotalPostedAmountWithinRangeAsync(startDate, endDate);
    }

    public async Task<IEnumerable<TransactionDto>> SearchTransactionsAsync(
        string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm));

        var transactions = await _unitOfWork.Transaction
            .SearchTransactionsAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
    }

    public async Task<int> CountAccountTransactionsByStatusAsync(
        int accountId,
        string status)
    {
        ValidateId(accountId, nameof(accountId));

        var normalizedStatus = NormalizeValue(
            status,
            ValidStatuses,
            nameof(status));

        return await _unitOfWork.Transaction
            .CountAccountTransactionsByStatusAsync(accountId, normalizedStatus);
    }

    // PRIVATE BUSINESS-RULE METHODS

    private async Task ValidateAccountsAsync(
        int? sourceAccountId,
        int? destinationAccountId,
        int currencyId,
        string category)
    {
        if (!sourceAccountId.HasValue && !destinationAccountId.HasValue)
            throw new InvalidOperationException(
                "At least one source or destination account is required.");

        if (sourceAccountId.HasValue && destinationAccountId.HasValue &&
            sourceAccountId.Value == destinationAccountId.Value)
        {
            throw new InvalidOperationException(
                "Source and destination accounts cannot be the same.");
        }

        if (sourceAccountId.HasValue)
        {
            var sourceAccount = await _unitOfWork.Account
                .GetByIdAsync(sourceAccountId.Value);

            if (sourceAccount == null || sourceAccount.Status != "Active")
                throw new InvalidOperationException(
                    "Source account does not exist or is not active.");

            if (sourceAccount.CurrencyId != currencyId)
                throw new InvalidOperationException(
                    "Transaction currency must match the source account currency.");
        }

        if (destinationAccountId.HasValue)
        {
            var destinationAccount = await _unitOfWork.Account
                .GetByIdAsync(destinationAccountId.Value);

            if (destinationAccount == null ||
                destinationAccount.Status != "Active")
            {
                throw new InvalidOperationException(
                    "Destination account does not exist or is not active.");
            }

            if (destinationAccount.CurrencyId != currencyId)
                throw new InvalidOperationException(
                    "Transaction currency must match the destination account currency.");
        }

        switch (category)
        {
            case "Deposit":
                if (!destinationAccountId.HasValue)
                    throw new InvalidOperationException(
                        "Deposit transactions require a destination account.");
                break;

            case "Withdrawal":
                if (!sourceAccountId.HasValue)
                    throw new InvalidOperationException(
                        "Withdrawal transactions require a source account.");
                break;

            case "Transfer":
                if (!sourceAccountId.HasValue ||
                    !destinationAccountId.HasValue)
                {
                    throw new InvalidOperationException(
                        "Transfer transactions require both source and destination accounts.");
                }
                break;

            case "Payment":
            case "Fee":
            case "Card":
                if (!sourceAccountId.HasValue)
                    throw new InvalidOperationException(
                        $"{category} transactions require a source account.");
                break;

            case "Interest":
            case "Refund":
            case "Loan":
                if (!sourceAccountId.HasValue &&
                    !destinationAccountId.HasValue)
                {
                    throw new InvalidOperationException(
                        $"{category} transactions require an account.");
                }
                break;

            case "Reversal":
                if (!sourceAccountId.HasValue &&
                    !destinationAccountId.HasValue)
                {
                    throw new InvalidOperationException(
                        "Reversal transactions require an account.");
                }
                break;
        }
    }

    private async Task ValidateFeeAsync(
        int? feeId,
        int transactionTypeId,
        decimal feeAmount,
        decimal taxAmount)
    {
        if (!feeId.HasValue)
        {
            if (feeAmount > 0 || taxAmount > 0)
                throw new InvalidOperationException(
                    "Fee and tax amounts require a linked fee definition.");

            return;
        }

        var fee = await _unitOfWork.Fee.GetByIdAsync(feeId.Value);

        if (fee == null || !fee.IsActive)
            throw new InvalidOperationException(
                "The selected fee does not exist or is inactive.");

        if (fee.TransactionTypeId.HasValue &&
            fee.TransactionTypeId.Value != transactionTypeId)
        {
            throw new InvalidOperationException(
                "The selected fee is not configured for this transaction type.");
        }

        var today = DateTime.UtcNow.Date;

        if (fee.EffectiveFrom.Date > today ||
            (fee.EffectiveTo.HasValue && fee.EffectiveTo.Value.Date < today))
        {
            throw new InvalidOperationException(
                "The selected fee is outside its effective period.");
        }

        if (!fee.IsGstApplicable && taxAmount > 0)
            throw new InvalidOperationException(
                "Tax cannot be charged for a fee that is not GST applicable.");
    }

    private async Task ValidateInitiatorAndApprovalAsync(
        int? initiatedByUserId,
        int? approvedByUserId,
        TransactionType transactionType,
        decimal amount)
    {
        User? initiator = null;
        User? approver = null;

        if (initiatedByUserId.HasValue)
        {
            initiator = await _unitOfWork.User
                .GetByIdAsync(initiatedByUserId.Value);

            if (initiator == null || !initiator.IsActive)
                throw new InvalidOperationException(
                    "The initiating user does not exist or is inactive.");
        }

        if (approvedByUserId.HasValue)
        {
            approver = await _unitOfWork.User
                .GetByIdAsync(approvedByUserId.Value);

            if (approver == null || !approver.IsActive)
                throw new InvalidOperationException(
                    "The approving user does not exist or is inactive.");

            if (initiatedByUserId == approvedByUserId)
                throw new InvalidOperationException(
                    "Maker-checker control violation: the initiating user cannot approve their own transaction.");
        }

        var approvalRequired = transactionType.RequiresApproval ||
            (transactionType.ApprovalThreshold.HasValue &&
             amount >= transactionType.ApprovalThreshold.Value);

        if (approvalRequired && !approvedByUserId.HasValue)
            throw new InvalidOperationException(
                "This transaction requires approval before it can proceed.");

        if (approvedByUserId.HasValue && initiator == null)
            throw new InvalidOperationException(
                "An initiating user must be recorded when an approver is assigned.");
    }

    private async Task ValidateDailyLimitAsync(
        int? sourceAccountId,
        TransactionType transactionType,
        decimal amount,
        int? excludeTransactionId = null)
    {
        if (!sourceAccountId.HasValue ||
            !transactionType.DailyLimit.HasValue)
        {
            return;
        }

        var dailyLimit = transactionType.DailyLimit.Value;

        if (dailyLimit <= 0)
            throw new InvalidOperationException(
                "Transaction type daily limit must be configured with a positive amount.");

        var today = DateTime.UtcNow.Date;

        var outgoingTransactions = await _unitOfWork.Transaction
            .GetBySourceAccountIdAsync(sourceAccountId.Value);

        var totalToday = outgoingTransactions
            .Where(x =>
                x.Status == "Posted" &&
                x.TransactionAt.Date == today &&
                x.TransactionId != excludeTransactionId)
            .Sum(x => x.Amount);

        if (totalToday + amount > dailyLimit)
            throw new InvalidOperationException(
                $"Transaction exceeds the configured daily limit of {dailyLimit}.");
    }

    private async Task ValidateReversalAsync(
        int? reversalOfTransactionId,
        TransactionType transactionType,
        int? currentTransactionId = null)
    {
        if (!reversalOfTransactionId.HasValue)
        {
            if (transactionType.Category == "Reversal")
                throw new InvalidOperationException(
                    "A reversal transaction must reference its original transaction.");

            return;
        }

        ValidateId(
            reversalOfTransactionId.Value,
            nameof(reversalOfTransactionId));

        if (currentTransactionId == reversalOfTransactionId.Value)
            throw new InvalidOperationException(
                "A transaction cannot reverse itself.");

        if (!transactionType.IsReversible)
            throw new InvalidOperationException(
                "This transaction type does not permit reversals.");

        var originalTransaction = await _unitOfWork.Transaction
            .GetByIdAsync(reversalOfTransactionId.Value);

        if (originalTransaction == null)
            throw new InvalidOperationException(
                "Original transaction does not exist.");

        if (originalTransaction.Status != "Posted")
            throw new InvalidOperationException(
                "Only posted transactions can be reversed.");

        var originalType = await _unitOfWork.TransactionType
            .GetByIdAsync(originalTransaction.TransactionTypeId);

        if (originalType == null || !originalType.IsReversible)
            throw new InvalidOperationException(
                "The original transaction type does not permit reversals.");

        var existingReversals = await _unitOfWork.Transaction
            .GetReversalTransactionsAsync(originalTransaction.TransactionId);

        if (existingReversals.Any(x =>
                x.Status != "Failed" && x.Status != "Cancelled"))
        {
            throw new InvalidOperationException(
                "The original transaction already has a non-cancelled reversal.");
        }
    }

    private static void ValidateStatusTransition(
        string currentStatus,
        string newStatus)
    {
        var allowedStatuses = currentStatus switch
        {
            "Initiated" => new[]
            {
                "Pending", "Processing", "OnHold", "Failed", "Cancelled"
            },
            "Pending" => new[]
            {
                "Processing", "OnHold", "Failed", "Cancelled"
            },
            "Processing" => new[]
            {
                "Pending", "OnHold", "Failed", "Cancelled"
            },
            "OnHold" => new[]
            {
                "Pending", "Processing", "Failed", "Cancelled"
            },
            _ => Array.Empty<string>()
        };

        if (currentStatus != newStatus &&
            !allowedStatuses.Contains(
                newStatus,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Transaction status transition from '{currentStatus}' to '{newStatus}' is not permitted.");
        }
    }

    private static string NormalizeValue(
        string value,
        IEnumerable<string> allowedValues,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);

        var normalized = allowedValues.FirstOrDefault(
            x => string.Equals(
                x,
                value.Trim(),
                StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid {parameterName}. Allowed values: {string.Join(", ", allowedValues)}.",
            parameterName);
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "ID must be greater than zero.");
    }

    private static void ValidateRequiredString(
        string? value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
    }

    private static void ValidateDate(DateTime date, string parameterName)
    {
        if (date == default)
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
    }

    private static void ValidateDateRange(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDate(startDate, nameof(startDate));
        ValidateDate(endDate, nameof(endDate));

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot precede start date.");
    }
}