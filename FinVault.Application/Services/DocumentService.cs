using AutoMapper;
using FinVault.Application.DTOs.DocumentDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public sealed class DocumentService : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DocumentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieve a document record by its ID.
    public async Task<DocumentDto?> GetByIdAsync(int documentId)
    {
        var document = await _unitOfWork.Document.GetDocumentDetailsAsync(documentId);
        return document == null ? null : _mapper.Map<DocumentDto>(document);
    }

    // 2. Retrieve all document records for a customer.
    public async Task<IReadOnlyList<DocumentDto>> GetByCustomerIdAsync(int customerId)
    {
        var documents = await _unitOfWork.Document.GetByCustomerIdAsync(customerId);
        return _mapper.Map<IReadOnlyList<DocumentDto>>(documents);
    }

    // 3. Register metadata for a file that has already been securely uploaded to storage.
    public async Task<DocumentDto> RegisterAsync(CreateDocumentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.FileSizeBytes <= 0) throw new ArgumentOutOfRangeException(nameof(dto.FileSizeBytes));
        if (string.IsNullOrWhiteSpace(dto.FileName) || string.IsNullOrWhiteSpace(dto.ContentType))
            throw new ArgumentException("File name and content type are required.");
        // StoragePath and ContentHash are required by the entity but are not present in CreateDocumentDto.
        // The current DTO contract therefore cannot safely register a document until these are supplied by a storage service.
        throw new NotSupportedException("Document registration requires a storage service to provide StoragePath and ContentHash. Extend the application contract before enabling this operation.");
    }

    // 4. Delete the document record. Physical file deletion must be coordinated with secure storage.
    public async Task<bool> DeleteAsync(int documentId)
    {
        var document = await _unitOfWork.Document.GetDocumentDetailsAsync(documentId);
        if (document == null) return false;
        _unitOfWork.Document.Delete(document);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
