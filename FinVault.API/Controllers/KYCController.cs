
using FinVault.Application.DTOs.KYCDetailsDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class KYCController : ControllerBase
{
    private readonly IKYCService _kycService;

    public KYCController(IKYCService kycService)
    {
        _kycService = kycService;
    }

    // ==================== KYC CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _kycService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("{kycDetailsId:int}")]
    public async Task<IActionResult> GetById(int kycDetailsId)
    {
        var data = await _kycService.GetByIdAsync(kycDetailsId);

        return data is null
            ? NotFound(ApiResponse<KYCDetailsDto>.FailureResponse("KYC record not found.", 404))
            : Ok(ApiResponse<KYCDetailsDto>.SuccessResponse(data));
    }

    [HttpGet("{kycDetailsId:int}/details")]
    public async Task<IActionResult> GetKYCDetails(int kycDetailsId)
    {
        var data = await _kycService.GetKYCDetailsAsync(kycDetailsId);

        return data is null
            ? NotFound(ApiResponse<KYCDetailsDto>.FailureResponse("KYC details not found.", 404))
            : Ok(ApiResponse<KYCDetailsDto>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/current")]
    public async Task<IActionResult> GetCurrentKYCByCustomerId(int customerId)
    {
        var data = await _kycService.GetCurrentKYCByCustomerIdAsync(customerId);

        return data is null
            ? NotFound(ApiResponse<KYCDetailsDto>.FailureResponse("Current KYC record not found.", 404))
            : Ok(ApiResponse<KYCDetailsDto>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/history")]
    public async Task<IActionResult> GetKYCHistoryByCustomerId(int customerId)
    {
        var data = await _kycService.GetKYCHistoryByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKYCDetailsDto request)
    {
        var data = await _kycService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { kycDetailsId = data.KYCDetailsId },
            ApiResponse<KYCDetailsDto>.SuccessResponse(
                data, "KYC record created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateKYCDetailsDto request)
    {
        var data = await _kycService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<KYCDetailsDto>.FailureResponse("KYC record not found.", 404))
            : Ok(ApiResponse<KYCDetailsDto>.SuccessResponse(
                data, "KYC record updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteKYCDetailsDto request)
    {
        var result = await _kycService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "KYC record deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "KYC record could not be deleted.", 409));
    }

    [HttpGet("customer/{customerId:int}/has-current")]
    public async Task<IActionResult> HasCurrentKYCRecord(int customerId)
    {
        var result = await _kycService.HasCurrentKYCRecordAsync(customerId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== KYC LEVEL AND VERIFICATION STATUS ====================

    [HttpGet("level/{kycLevel}")]
    public async Task<IActionResult> GetByKycLevel(string kycLevel)
    {
        var data = await _kycService.GetByKycLevelAsync(kycLevel);
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("verification-status/{verificationStatus}")]
    public async Task<IActionResult> GetByVerificationStatus(string verificationStatus)
    {
        var data = await _kycService.GetByVerificationStatusAsync(verificationStatus);
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingKYCRecords()
    {
        var data = await _kycService.GetPendingKYCRecordsAsync();
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("in-progress")]
    public async Task<IActionResult> GetInProgressKYCRecords()
    {
        var data = await _kycService.GetInProgressKYCRecordsAsync();
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("verified")]
    public async Task<IActionResult> GetVerifiedKYCRecords()
    {
        var data = await _kycService.GetVerifiedKYCRecordsAsync();
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("rejected")]
    public async Task<IActionResult> GetRejectedKYCRecords()
    {
        var data = await _kycService.GetRejectedKYCRecordsAsync();
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredKYCRecords()
    {
        var data = await _kycService.GetExpiredKYCRecordsAsync();
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    // ==================== PAN VERIFICATION ====================

    [HttpGet("pan/current")]
    public async Task<IActionResult> GetCurrentByPan([FromQuery] string pan)
    {
        if (string.IsNullOrWhiteSpace(pan))
            return BadRequest(ApiResponse<object>.FailureResponse("PAN is required.", 400));

        var data = await _kycService.GetCurrentByPanAsync(pan);

        return data is null
            ? NotFound(ApiResponse<KYCDetailsDto>.FailureResponse("Current KYC record not found for PAN.", 404))
            : Ok(ApiResponse<KYCDetailsDto>.SuccessResponse(data));
    }

    [HttpGet("pan/exists-current")]
    public async Task<IActionResult> ExistsCurrentByPan([FromQuery] string pan)
    {
        if (string.IsNullOrWhiteSpace(pan))
            return BadRequest(ApiResponse<object>.FailureResponse("PAN is required.", 400));

        var result = await _kycService.ExistsCurrentByPanAsync(pan);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("pan/verified")]
    public async Task<IActionResult> GetPanVerifiedRecords()
    {
        var data = await _kycService.GetPanVerifiedRecordsAsync();
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("pan/pending-verification")]
    public async Task<IActionResult> GetPanPendingVerificationRecords()
    {
        var data = await _kycService.GetPanPendingVerificationRecordsAsync();
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpPatch("{kycDetailsId:int}/pan/verify")]
    public async Task<IActionResult> VerifyPan(int kycDetailsId)
    {
        var result = await _kycService.VerifyPanAsync(kycDetailsId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "PAN verified successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse("PAN verification failed.", 400));
    }

    // ==================== AADHAAR AND CKYC MANAGEMENT ====================

    [HttpGet("aadhaar/verified")]
    public async Task<IActionResult> GetAadhaarVerifiedRecords()
    {
        var data = await _kycService.GetAadhaarVerifiedRecordsAsync();
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("aadhaar/verification-method/{verificationMethod}")]
    public async Task<IActionResult> GetByAadhaarVerificationMethod(string verificationMethod)
    {
        var data = await _kycService.GetByAadhaarVerificationMethodAsync(verificationMethod);
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("ckyc/number")]
    public async Task<IActionResult> GetByCkycNumber([FromQuery] string ckycNumber)
    {
        var data = await _kycService.GetByCkycNumberAsync(ckycNumber);

        return data is null
            ? NotFound(ApiResponse<KYCDetailsDto>.FailureResponse("CKYC record not found.", 404))
            : Ok(ApiResponse<KYCDetailsDto>.SuccessResponse(data));
    }

    [HttpGet("ckyc/status/{ckycStatus}")]
    public async Task<IActionResult> GetByCkycStatus(string ckycStatus)
    {
        var data = await _kycService.GetByCkycStatusAsync(ckycStatus);
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpPatch("{kycDetailsId:int}/ckyc/status")]
    public async Task<IActionResult> UpdateCkycStatus(
        int kycDetailsId,
        [FromQuery] string ckycStatus)
    {
        var result = await _kycService.UpdateCkycStatusAsync(
            kycDetailsId, ckycStatus);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "CKYC status updated successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse("CKYC status update failed.", 400));
    }

    // ==================== RE-KYC AND VERIFICATION DEADLINES ====================

    [HttpGet("rekyc/overdue")]
    public async Task<IActionResult> GetOverdueReKYCRecords(
        [FromQuery] DateTime currentDate)
    {
        var data = await _kycService.GetOverdueReKYCRecordsAsync(currentDate);
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("rekyc/due-within-range")]
    public async Task<IActionResult> GetReKYCRecordsDueWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        var data = await _kycService.GetReKYCRecordsDueWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpPatch("{kycDetailsId:int}/rekyc/due-date")]
    public async Task<IActionResult> UpdateReKycDueDate(
        int kycDetailsId,
        [FromQuery] DateTime nextReKycDueDate)
    {
        var result = await _kycService.UpdateReKycDueDateAsync(
            kycDetailsId, nextReKycDueDate);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Re-KYC due date updated successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse("Re-KYC due date update failed.", 400));
    }

    // ==================== EMPLOYEE VERIFICATION AND DOCUMENTS ====================

    [HttpGet("verified-by-employee/{employeeId:int}")]
    public async Task<IActionResult> GetByVerifiedByEmployeeId(int employeeId)
    {
        var data = await _kycService.GetByVerifiedByEmployeeIdAsync(employeeId);
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/documents")]
    public async Task<IActionResult> GetKYCRecordsWithDocuments(int customerId)
    {
        var data = await _kycService.GetKYCRecordsWithDocumentsAsync(customerId);
        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpPatch("{kycDetailsId:int}/verify")]
    public async Task<IActionResult> VerifyKYC(
        int kycDetailsId,
        [FromQuery] int employeeId)
    {
        var result = await _kycService.VerifyKYCAsync(
            kycDetailsId, employeeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "KYC verified successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse("KYC verification failed.", 400));
    }

    [HttpPatch("{kycDetailsId:int}/reject")]
    public async Task<IActionResult> RejectKYC(
        int kycDetailsId,
        [FromQuery] int employeeId,
        [FromQuery] string rejectionReason)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Rejection reason is required.", 400));

        var result = await _kycService.RejectKYCAsync(
            kycDetailsId, employeeId, rejectionReason);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "KYC rejected successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse("KYC rejection failed.", 400));
    }

    // ==================== DATE AND REPORTING ====================

    [HttpGet("verified/date-range")]
    public async Task<IActionResult> GetVerifiedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        var data = await _kycService.GetVerifiedWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<KYCDetailsDto>>.SuccessResponse(data));
    }

    [HttpGet("count/current-by-status/{verificationStatus}")]
    public async Task<IActionResult> CountCurrentRecordsByStatus(string verificationStatus)
    {
        var count = await _kycService.CountCurrentRecordsByStatusAsync(verificationStatus);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }
}
