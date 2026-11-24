using AutoMapper;
using FinVault.Application.DTOs.TransactionDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Creates and tracks transaction records. Posting money movements requires a separately verified ledger workflow.</summary>
public sealed class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieves a transaction with related accounting and payment information.
    public async Task<TransactionDto?> GetByIdAsync(int transactionId)
    {
        if (transactionId <= 0) throw new ArgumentOutOfRangeException(nameof(transactionId));
        var transaction = await _unitOfWork.Transaction.GetTransactionDetailsAsync(transactionId);
        return transaction is null ? null : _mapper.Map<TransactionDto>(transaction);
    }

    // 2. Retrieves transaction history for an account, including incoming and outgoing activity.
    public async Task<IReadOnlyList<TransactionDto>> GetByAccountIdAsync(int accountId)
    {
        if (accountId <= 0) throw new ArgumentOutOfRangeException(nameof(accountId));
        var transactions = await _unitOfWork.Transaction.GetAccountTransactionHistoryAsync(accountId);
        return _mapper.Map<IReadOnlyList<TransactionDto>>(transactions);
    }

    // 3. Validates and records a transfer instruction. This method deliberately does not debit or credit balances.
    public async Task<TransactionDto> TransferAsync(CreateTransactionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.Amount <= 0) throw new ArgumentException("Transfer amount must be greater than zero.");
        if (!dto.SourceAccountId.HasValue || !dto.DestinationAccountId.HasValue) throw new ArgumentException("Both source and destination accounts are required.");
        if (dto.SourceAccountId == dto.DestinationAccountId) throw new ArgumentException("Source and destination accounts must be different.");
        if (string.IsNullOrWhiteSpace(dto.TransactionReference)) throw new ArgumentException("Transaction reference is required.");
        if (dto.IdempotencyKey is not null && await _unitOfWork.Transaction.ExistsByIdempotencyKeyAsync(dto.IdempotencyKey))
            throw new InvalidOperationException("A transaction with this idempotency key already exists.");

        var source = await _unitOfWork.Account.GetAccountDetailsAsync(dto.SourceAccountId.Value);
        var destination = await _unitOfWork.Account.GetAccountDetailsAsync(dto.DestinationAccountId.Value);
        if (source is null || destination is null) throw new InvalidOperationException("Source or destination account was not found.");
        if (source.Status != "Active" || destination.Status != "Active") throw new InvalidOperationException("Both accounts must be active.");
        if (source.CurrencyId != destination.CurrencyId || source.CurrencyId != dto.CurrencyId) throw new InvalidOperationException("Both accounts and the transaction must use the same currency.");
        if (source.AvailableBalance < dto.Amount + dto.FeeAmount + dto.TaxAmount) throw new InvalidOperationException("Insufficient available balance.");

        var transaction = _mapper.Map<Transaction>(dto);
        transaction.TransactionReference = dto.TransactionReference.Trim();
        transaction.Status = "Initiated";
        transaction.TransactionAt = DateTime.UtcNow;
        transaction.ValueDate = dto.ValueDate == default ? DateTime.UtcNow.Date : dto.ValueDate;
        transaction.PostedAt = null;
        transaction.SourceBalanceAfter = null;
        transaction.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Transaction.AddAsync(transaction);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TransactionDto>(transaction);
    }

    // 4. Creates a reversal instruction linked to an eligible original transaction; actual ledger reversal must be posted by the ledger workflow.
    public async Task<TransactionDto?> ReverseAsync(int transactionId, string reason)
    {
        if (transactionId <= 0) throw new ArgumentOutOfRangeException(nameof(transactionId));
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("A reversal reason is required.", nameof(reason));
        var original = await _unitOfWork.Transaction.GetTransactionDetailsAsync(transactionId);
        if (original is null) return null;
        if (original.Status != "Posted") throw new InvalidOperationException("Only a posted transaction can be reversed.");
        if (original.ReversalOfTransactionId.HasValue) throw new InvalidOperationException("A reversal transaction cannot itself be reversed.");
        var existingReversals = await _unitOfWork.Transaction.GetReversalTransactionsAsync(transactionId);
        if (existingReversals.Any()) throw new InvalidOperationException("This transaction has already been reversed.");

        var reversal = new Transaction
        {
            TransactionReference = $"REV-{Guid.NewGuid():N}",
            Amount = original.Amount,
            FeeAmount = 0,
            TaxAmount = 0,
            Status = "Initiated",
            Channel = "Internal",
            TransactionAt = DateTime.UtcNow,
            ValueDate = DateTime.UtcNow.Date,
            Description = $"Reversal of {original.TransactionReference}",
            Remarks = reason.Trim(),
            TransactionTypeId = original.TransactionTypeId,
            SourceAccountId = original.DestinationAccountId,
            DestinationAccountId = original.SourceAccountId,
            CurrencyId = original.CurrencyId,
            ReversalOfTransactionId = original.TransactionId,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Transaction.AddAsync(reversal);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TransactionDto>(reversal);
    }
}
