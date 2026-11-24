using AutoMapper;
using FinVault.Application.DTOs.KYCDetailsDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public sealed class KYCDetailsService : IKYCDetailsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public KYCDetailsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Return the customer's current KYC record, if one exists.
    public async Task<KYCDetailsDto?> GetByCustomerIdAsync(int customerId)
    {
        var record = await _unitOfWork.KYCDetails.GetCurrentKYCByCustomerIdAsync(customerId);
        return record == null ? null : _mapper.Map<KYCDetailsDto>(record);
    }

    // 2. Submit a KYC record. Raw PAN must be hashed/encrypted by a dedicated secure component before persistence.
    public async Task<KYCDetailsDto> SubmitAsync(CreateKYCDetailsDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _unitOfWork.KYCDetails.HasCurrentKYCRecordAsync(dto.CustomerId))
            throw new InvalidOperationException("The customer already has a current KYC record.");

        var record = _mapper.Map<KYCDetails>(dto);
        record.IsCurrent = true;
        record.VerificationStatus = "Pending";
        record.PanHash = null;
        record.PanEncrypted = null;
        record.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.KYCDetails.AddAsync(record);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<KYCDetailsDto>(record);
    }

    // 3. Update an existing KYC record without accepting a client-supplied primary key mismatch.
    public async Task<KYCDetailsDto?> UpdateAsync(int kycDetailsId, UpdateKYCDetailsDto dto)
    {
        var record = await _unitOfWork.KYCDetails.GetKYCDetailsAsync(kycDetailsId);
        if (record == null) return null;
        if (dto.KYCDetailsId != kycDetailsId) throw new ArgumentException("Route KYC ID and DTO KYC ID do not match.");
        _mapper.Map(dto, record);
        record.PanHash = null; // Never derive a hash from a raw identifier without the approved security service.
        record.PanEncrypted = null;
        record.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.KYCDetails.Update(record);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<KYCDetailsDto>(record);
    }

    // 4. Record an authorized review decision.
    public async Task<KYCDetailsDto?> ReviewAsync(int kycDetailsId, string decision, string? remarks)
    {
        var record = await _unitOfWork.KYCDetails.GetKYCDetailsAsync(kycDetailsId);
        if (record == null) return null;
        var normalized = decision?.Trim().ToLowerInvariant();
        if (normalized is not ("verified" or "rejected" or "inprogress"))
            throw new ArgumentException("Decision must be Verified, Rejected, or InProgress.", nameof(decision));
        record.VerificationStatus = normalized == "inprogress" ? "InProgress" : normalized == "verified" ? "Verified" : "Rejected";
        record.VerifiedAt = normalized == "verified" ? DateTime.UtcNow : null;
        record.RejectionReason = normalized == "rejected" ? remarks : null;
        record.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.KYCDetails.Update(record);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<KYCDetailsDto>(record);
    }
}
