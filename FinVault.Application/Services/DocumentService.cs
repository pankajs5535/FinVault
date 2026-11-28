
using AutoMapper;
using FinVault.Application.DTOs.DocumentDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public sealed class DocumentService : IDocumentService
{
    private static readonly string[] ValidDocumentTypes =
    {
        "Pan", "Aadhaar", "Passport", "VoterId", "DrivingLicence",
        "Photo", "Signature", "AddressProof", "IncomeProof",
        "Form60", "LoanAgreement", "Other"
    };

    private static readonly string[] ValidStatuses =
        { "Pending", "Verified", "Rejected", "Expired" };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public DocumentService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    // 1. DOCUMENT IDENTIFICATION AND CRUD

    public async Task<IEnumerable<DocumentDto>> GetAllAsync()
        => MapMany(await _unitOfWork.Document.GetAllAsync());

    public async Task<DocumentDto?> GetByIdAsync(int documentId)
    {
        ValidateId(documentId, nameof(documentId));
        return Map(await _unitOfWork.Document.GetByIdAsync(documentId));
    }

    public async Task<DocumentDto?> GetDocumentDetailsAsync(int documentId)
    {
        ValidateId(documentId, nameof(documentId));
        return Map(await _unitOfWork.Document.GetDocumentDetailsAsync(documentId));
    }

    public async Task<DocumentDto?> GetByContentHashAsync(string contentHash)
    {
        RequireText(contentHash, nameof(contentHash), 64);
        return Map(await _unitOfWork.Document.GetByContentHashAsync(contentHash.Trim()));
    }

    public Task<DocumentDto> CreateAsync(CreateDocumentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        throw new NotSupportedException(
            "Document creation requires an encrypted file-storage service to generate StoragePath and SHA-256 ContentHash. The current CreateDocumentDto does not provide file content or storage metadata.");
    }

    public async Task<DocumentDto?> UpdateAsync(UpdateDocumentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.DocumentId, nameof(dto.DocumentId));
        ValidateDocumentType(dto.DocumentType);
        ValidateStatus(dto.VerificationStatus);
        ValidateDocumentDates(dto.IssueDate, dto.ExpiryDate);
        ValidateFileMetadata(dto.FileName, dto.ContentType, dto.FileSizeBytes);

        var document = await _unitOfWork.Document.GetByIdAsync(dto.DocumentId);
        if (document is null)
            return null;

        if (dto.VerificationStatus == "Verified" &&
            document.VerificationStatus != "Verified")
        {
            throw new InvalidOperationException(
                "Use an authorized document-verification workflow to verify a document.");
        }

        if (dto.VerifiedAt != document.VerifiedAt ||
            dto.RejectionReason != document.RejectionReason)
        {
            throw new InvalidOperationException(
                "Verification audit fields cannot be changed through UpdateAsync.");
        }

        await ValidateReferencesAsync(
            dto.CustomerId,
            dto.KYCDetailsId,
            dto.LoanId,
            dto.VerifiedByEmployeeId);

        // Preserve storage metadata and verification audit fields.
        var storagePath = document.StoragePath;
        var contentHash = document.ContentHash;
        var verificationStatus = document.VerificationStatus;
        var verifiedAt = document.VerifiedAt;
        var rejectionReason = document.RejectionReason;
        var verifiedByEmployeeId = document.VerifiedByEmployeeId;

        _mapper.Map(dto, document);

        document.StoragePath = storagePath;
        document.ContentHash = contentHash;
        document.VerificationStatus = verificationStatus;
        document.VerifiedAt = verifiedAt;
        document.RejectionReason = rejectionReason;
        document.VerifiedByEmployeeId = verifiedByEmployeeId;
        document.UpdatedAt = DateTime.UtcNow;
        document.UpdatedBy = _currentUser.UserId;

        _unitOfWork.Document.Update(document);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DocumentDto>(document);
    }

    public Task<bool> DeleteAsync(DeleteDocumentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.DocumentId, nameof(dto.DocumentId));

        // Banking and KYC documents are compliance evidence.
        // Physical deletion is not supported by the current entity model.
        return Task.FromResult(false);
    }

    // 2. CUSTOMER DOCUMENT MANAGEMENT

    public async Task<IEnumerable<DocumentDto>> GetByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));
        return MapMany(await _unitOfWork.Document.GetByCustomerIdAsync(customerId));
    }

    public async Task<IEnumerable<DocumentDto>> GetByCustomerAndDocumentTypeAsync(
        int customerId, string documentType)
    {
        ValidateId(customerId, nameof(customerId));
        ValidateDocumentType(documentType);

        return MapMany(await _unitOfWork.Document.GetByCustomerAndDocumentTypeAsync(
            customerId, documentType.Trim()));
    }

    public async Task<IEnumerable<DocumentDto>> GetCustomerDocumentsWithVerificationDetailsAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));
        return MapMany(await _unitOfWork.Document
            .GetCustomerDocumentsWithVerificationDetailsAsync(customerId));
    }

    public async Task<int> CountDocumentsByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));
        return await _unitOfWork.Document.CountDocumentsByCustomerIdAsync(customerId);
    }

    // 3. KYC AND LOAN DOCUMENT MANAGEMENT

    public async Task<IEnumerable<DocumentDto>> GetByKYCDetailsIdAsync(int kycDetailsId)
    {
        ValidateId(kycDetailsId, nameof(kycDetailsId));
        return MapMany(await _unitOfWork.Document.GetByKYCDetailsIdAsync(kycDetailsId));
    }

    public async Task<IEnumerable<DocumentDto>> GetByLoanIdAsync(int loanId)
    {
        ValidateId(loanId, nameof(loanId));
        return MapMany(await _unitOfWork.Document.GetByLoanIdAsync(loanId));
    }

    public async Task<IEnumerable<DocumentDto>> GetByVerifiedByEmployeeIdAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        return MapMany(await _unitOfWork.Document.GetByVerifiedByEmployeeIdAsync(employeeId));
    }

    // 4. DOCUMENT VERIFICATION

    public async Task<IEnumerable<DocumentDto>> GetByVerificationStatusAsync(
        string verificationStatus)
    {
        ValidateStatus(verificationStatus);
        return MapMany(await _unitOfWork.Document
            .GetByVerificationStatusAsync(verificationStatus.Trim()));
    }

    public async Task<IEnumerable<DocumentDto>> GetPendingVerificationDocumentsAsync()
        => MapMany(await _unitOfWork.Document.GetPendingVerificationDocumentsAsync());

    public async Task<IEnumerable<DocumentDto>> GetRejectedDocumentsAsync()
        => MapMany(await _unitOfWork.Document.GetRejectedDocumentsAsync());

    public async Task<IEnumerable<DocumentDto>> GetVerifiedDocumentsByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));
        return MapMany(await _unitOfWork.Document.GetVerifiedDocumentsByCustomerIdAsync(customerId));
    }

    // 5. DOCUMENT EXPIRY MANAGEMENT

    public async Task<IEnumerable<DocumentDto>> GetExpiredDocumentsAsync(DateTime currentDate)
    {
        if (currentDate == default)
            throw new ArgumentException("A valid current date is required.", nameof(currentDate));

        return MapMany(await _unitOfWork.Document.GetExpiredDocumentsAsync(currentDate));
    }

    public async Task<IEnumerable<DocumentDto>> GetDocumentsExpiringWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);
        return MapMany(await _unitOfWork.Document
            .GetDocumentsExpiringWithinRangeAsync(startDate, endDate));
    }

    // 6. FILE STORAGE AND SECURITY

    public async Task<IEnumerable<DocumentDto>> GetUnencryptedDocumentsAsync()
        => MapMany(await _unitOfWork.Document.GetUnencryptedDocumentsAsync());

    public async Task<bool> ExistsByContentHashAsync(string contentHash)
    {
        RequireText(contentHash, nameof(contentHash), 64);
        return await _unitOfWork.Document.ExistsByContentHashAsync(contentHash.Trim());
    }

    // 7. SEARCH AND REPORTING

    public async Task<IEnumerable<DocumentDto>> SearchDocumentsAsync(string searchTerm)
    {
        RequireText(searchTerm, nameof(searchTerm), 200);
        return MapMany(await _unitOfWork.Document.SearchDocumentsAsync(searchTerm.Trim()));
    }

    // PRIVATE HELPERS

    private async Task ValidateReferencesAsync(
        int customerId,
        int? kycDetailsId,
        int? loanId,
        int? verifiedByEmployeeId)
    {
        ValidateId(customerId, nameof(customerId));

        if (await _unitOfWork.Customer.GetByIdAsync(customerId) is null)
            throw new InvalidOperationException($"Customer with ID {customerId} does not exist.");

        if (kycDetailsId.HasValue)
        {
            ValidateId(kycDetailsId.Value, nameof(kycDetailsId));
            if (await _unitOfWork.KYCDetails.GetByIdAsync(kycDetailsId.Value) is null)
                throw new InvalidOperationException(
                    $"KYC record with ID {kycDetailsId.Value} does not exist.");
        }

        if (loanId.HasValue)
        {
            ValidateId(loanId.Value, nameof(loanId));
            if (await _unitOfWork.Loan.GetByIdAsync(loanId.Value) is null)
                throw new InvalidOperationException(
                    $"Loan with ID {loanId.Value} does not exist.");
        }

        if (verifiedByEmployeeId.HasValue)
        {
            ValidateId(verifiedByEmployeeId.Value, nameof(verifiedByEmployeeId));
            if (await _unitOfWork.Employee.GetByIdAsync(verifiedByEmployeeId.Value) is null)
                throw new InvalidOperationException(
                    $"Employee with ID {verifiedByEmployeeId.Value} does not exist.");
        }
    }

    private DocumentDto? Map(Document? document)
        => document is null ? null : _mapper.Map<DocumentDto>(document);

    private IEnumerable<DocumentDto> MapMany(IEnumerable<Document> documents)
        => _mapper.Map<IEnumerable<DocumentDto>>(documents);

    private static void ValidateDocumentType(string documentType)
    {
        RequireText(documentType, nameof(documentType), 30);
        if (!ValidDocumentTypes.Contains(documentType.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid document type.", nameof(documentType));
    }

    private static void ValidateStatus(string status)
    {
        RequireText(status, nameof(status), 20);
        if (!ValidStatuses.Contains(status.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid document verification status.", nameof(status));
    }

    private static void ValidateDocumentDates(DateTime? issueDate, DateTime? expiryDate)
    {
        if (issueDate.HasValue && expiryDate.HasValue &&
            expiryDate.Value.Date < issueDate.Value.Date)
        {
            throw new ArgumentException("Document expiry date cannot be earlier than issue date.");
        }
    }

    private static void ValidateFileMetadata(string fileName, string contentType, long fileSizeBytes)
    {
        RequireText(fileName, nameof(fileName), 260);
        RequireText(contentType, nameof(contentType), 100);

        if (fileSizeBytes < 0)
            throw new ArgumentOutOfRangeException(nameof(fileSizeBytes));
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(parameterName);
    }

    private static void RequireText(string? value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A value is required.", parameterName);

        if (value.Trim().Length > maxLength)
            throw new ArgumentException($"Value cannot exceed {maxLength} characters.", parameterName);
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default || startDate > endDate)
            throw new ArgumentException(
                "A valid date range is required; start date must be earlier than or equal to end date.");
    }
}
