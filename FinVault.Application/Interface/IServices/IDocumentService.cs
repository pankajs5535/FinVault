using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.DocumentDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Document banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IDocumentService
{
        Task<DocumentDto?> GetByIdAsync(int documentId);
        Task<IReadOnlyList<DocumentDto>> GetByCustomerIdAsync(int customerId);
        Task<DocumentDto> RegisterAsync(CreateDocumentDto dto);
        Task<bool> DeleteAsync(int documentId);
}
