
using FinVault.Application.DTOs.DocumentDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    // ==================== DOCUMENT CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _documentService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("{documentId:int}")]
    public async Task<IActionResult> GetById(int documentId)
    {
        var data = await _documentService.GetByIdAsync(documentId);

        return data is null
            ? NotFound(ApiResponse<DocumentDto>.FailureResponse("Document not found.", 404))
            : Ok(ApiResponse<DocumentDto>.SuccessResponse(data));
    }

    [HttpGet("{documentId:int}/details")]
    public async Task<IActionResult> GetDocumentDetails(int documentId)
    {
        var data = await _documentService.GetDocumentDetailsAsync(documentId);

        return data is null
            ? NotFound(ApiResponse<DocumentDto>.FailureResponse("Document details not found.", 404))
            : Ok(ApiResponse<DocumentDto>.SuccessResponse(data));
    }

    [HttpGet("content-hash")]
    public async Task<IActionResult> GetByContentHash(
        [FromQuery] string contentHash)
    {
        if (string.IsNullOrWhiteSpace(contentHash))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Content hash is required.", 400));

        var data = await _documentService.GetByContentHashAsync(contentHash);

        return data is null
            ? NotFound(ApiResponse<DocumentDto>.FailureResponse("Document not found.", 404))
            : Ok(ApiResponse<DocumentDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocumentDto request)
    {
        var data = await _documentService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { documentId = data.DocumentId },
            ApiResponse<DocumentDto>.SuccessResponse(
                data, "Document created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateDocumentDto request)
    {
        var data = await _documentService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<DocumentDto>.FailureResponse("Document not found.", 404))
            : Ok(ApiResponse<DocumentDto>.SuccessResponse(
                data, "Document updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteDocumentDto request)
    {
        var result = await _documentService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Document deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Document could not be deleted.", 409));
    }

    // ==================== CUSTOMER DOCUMENT MANAGEMENT ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var data = await _documentService.GetByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/type/{documentType}")]
    public async Task<IActionResult> GetByCustomerAndDocumentType(
        int customerId,
        string documentType)
    {
        var data = await _documentService.GetByCustomerAndDocumentTypeAsync(
            customerId, documentType);

        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/verification-details")]
    public async Task<IActionResult> GetCustomerDocumentsWithVerificationDetails(
        int customerId)
    {
        var data = await _documentService
            .GetCustomerDocumentsWithVerificationDetailsAsync(customerId);

        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/count")]
    public async Task<IActionResult> CountDocumentsByCustomerId(int customerId)
    {
        var count = await _documentService.CountDocumentsByCustomerIdAsync(customerId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    // ==================== KYC AND LOAN DOCUMENT MANAGEMENT ====================

    [HttpGet("kyc/{kycDetailsId:int}")]
    public async Task<IActionResult> GetByKYCDetailsId(int kycDetailsId)
    {
        var data = await _documentService.GetByKYCDetailsIdAsync(kycDetailsId);
        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("loan/{loanId:int}")]
    public async Task<IActionResult> GetByLoanId(int loanId)
    {
        var data = await _documentService.GetByLoanIdAsync(loanId);
        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("verified-by-employee/{employeeId:int}")]
    public async Task<IActionResult> GetByVerifiedByEmployeeId(int employeeId)
    {
        var data = await _documentService.GetByVerifiedByEmployeeIdAsync(employeeId);
        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    // ==================== DOCUMENT VERIFICATION ====================

    [HttpGet("verification-status/{verificationStatus}")]
    public async Task<IActionResult> GetByVerificationStatus(string verificationStatus)
    {
        var data = await _documentService.GetByVerificationStatusAsync(
            verificationStatus);

        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("pending-verification")]
    public async Task<IActionResult> GetPendingVerificationDocuments()
    {
        var data = await _documentService.GetPendingVerificationDocumentsAsync();
        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("rejected")]
    public async Task<IActionResult> GetRejectedDocuments()
    {
        var data = await _documentService.GetRejectedDocumentsAsync();
        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/verified")]
    public async Task<IActionResult> GetVerifiedDocumentsByCustomerId(int customerId)
    {
        var data = await _documentService.GetVerifiedDocumentsByCustomerIdAsync(
            customerId);

        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    // ==================== DOCUMENT EXPIRY MANAGEMENT ====================

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredDocuments(
        [FromQuery] DateTime currentDate)
    {
        var data = await _documentService.GetExpiredDocumentsAsync(currentDate);
        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("expiring-within-range")]
    public async Task<IActionResult> GetDocumentsExpiringWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        var data = await _documentService.GetDocumentsExpiringWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    // ==================== FILE STORAGE AND SECURITY ====================

    [HttpGet("unencrypted")]
    public async Task<IActionResult> GetUnencryptedDocuments()
    {
        var data = await _documentService.GetUnencryptedDocumentsAsync();
        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }

    [HttpGet("exists/content-hash")]
    public async Task<IActionResult> ExistsByContentHash(
        [FromQuery] string contentHash)
    {
        if (string.IsNullOrWhiteSpace(contentHash))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Content hash is required.", 400));

        var result = await _documentService.ExistsByContentHashAsync(contentHash);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== SEARCH AND REPORTING ====================

    [HttpGet("search")]
    public async Task<IActionResult> SearchDocuments(
        [FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Search term is required.", 400));

        var data = await _documentService.SearchDocumentsAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<DocumentDto>>.SuccessResponse(data));
    }
}
