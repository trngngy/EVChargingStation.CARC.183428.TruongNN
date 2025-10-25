using EVChargingStation.CARC.Application.TruongNN.Interfaces;
using EVChargingStation.CARC.Application.TruongNN.Interfaces.Commons;
using EVChargingStation.CARC.Application.TruongNN.Utils;
using EVChargingStation.CARC.Domain.TruongNN.DTOs.InvoiceDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVChargingStation.CARC.WebAPI.TruongNN.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILoggerService _logger;

        public InvoiceController(IInvoiceService invoiceService, ILoggerService logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        /// <summary>
        /// Create invoice from a completed charging session
        /// </summary>
        /// <remarks>
        /// Creates an invoice for a completed charging session with automatic calculation of subtotal, tax, and total amount.
        /// Only Admin can create invoices.
        /// </remarks>
        [HttpPost("from-session")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CreateInvoiceFromSession([FromBody] CreateInvoiceFromSessionDto dto)
        {
            try
            {
                var result = await _invoiceService.CreateInvoiceFromSessionAsync(dto);
                return Ok(ApiResult<InvoiceResponseDto>.Success(
                    result,
                    "200",
                    "Invoice created from session successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating invoice from session: {ex.Message}");
                var statusCode = ex.Data["StatusCode"] as int? ?? 500;
                return StatusCode(statusCode,
                    ApiResult<InvoiceResponseDto>.Failure(statusCode.ToString(), ex.Message));
            }
        }

        /// <summary>
        /// Get invoice details by ID
        /// </summary>
        /// <remarks>
        /// Returns detailed information about an invoice including session and payment details.
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceById(Guid id)
        {
            try
            {
                var result = await _invoiceService.GetInvoiceByIdAsync(id);
                return Ok(ApiResult<InvoiceDetailResponseDto>.Success(
                    result,
                    "200",
                    "Invoice retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting invoice: {ex.Message}");
                var statusCode = ex.Data["StatusCode"] as int? ?? 500;
                return StatusCode(statusCode,
                    ApiResult<InvoiceDetailResponseDto>.Failure(statusCode.ToString(), ex.Message));
            }
        }

        /// <summary>
        /// Get all invoices with pagination, filtering, and search
        /// </summary>
        /// <remarks>
        /// Returns paginated list of invoices with support for:
        /// - Search by user email or name
        /// - Filter by status (1=Outstanding, 2=Paid, 3=Canceled)
        /// - Filter by user ID
        /// - Filter overdue invoices
        /// - Sort by various fields (CreatedAt, PeriodStart, PeriodEnd, TotalAmount, Status, DueDate)
        /// - Sort order (asc/desc)
        /// 
        /// Example: GET /api/invoices?pageNumber=1&amp;pageSize=10&amp;status=1&amp;sortBy=DueDate&amp;sortOrder=asc
        /// </remarks>
        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> GetAllInvoices([FromQuery] GetInvoicesQueryDto query)
        {
            try
            {
                var result = await _invoiceService.GetAllInvoicesAsync(query);
                return Ok(ApiResult<PaginatedList<InvoiceResponseDto>>.Success(
                    result,
                    "200",
                    $"Retrieved {result.Items.Count} of {result.TotalCount} invoices (Page {result.PageNumber}/{result.TotalPages})."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting invoices: {ex.Message}");
                var statusCode = ex.Data["StatusCode"] as int? ?? 500;
                return StatusCode(statusCode,
                    ApiResult<PaginatedList<InvoiceResponseDto>>.Failure(statusCode.ToString(), ex.Message));
            }
        }

        /// <summary>
        /// Update invoice information
        /// </summary>
        /// <remarks>
        /// Updates invoice fields. Can update period dates, status, amounts, and due date.
        /// Total amount is automatically recalculated when subtotal or tax changes.
        /// Only Admin can update invoices.
        /// </remarks>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UpdateInvoice(Guid id, [FromBody] UpdateInvoiceDto dto)
        {
            try
            {
                var result = await _invoiceService.UpdateInvoiceAsync(id, dto);
                return Ok(ApiResult<InvoiceResponseDto>.Success(
                    result,
                    "200",
                    "Invoice updated successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating invoice: {ex.Message}");
                var statusCode = ex.Data["StatusCode"] as int? ?? 500;
                return StatusCode(statusCode,
                    ApiResult<InvoiceResponseDto>.Failure(statusCode.ToString(), ex.Message));
            }
        }

        /// <summary>
        /// Delete invoice (soft delete)
        /// </summary>
        /// <remarks>
        /// Soft deletes an invoice. Cannot delete paid invoices.
        /// Only Admin can delete invoices.
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteInvoice(Guid id)
        {
            try
            {
                var result = await _invoiceService.DeleteInvoiceAsync(id);
                return Ok(ApiResult<bool>.Success(
                    result,
                    "200",
                    "Invoice deleted successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting invoice: {ex.Message}");
                var statusCode = ex.Data["StatusCode"] as int? ?? 500;
                return StatusCode(statusCode,
                    ApiResult<bool>.Failure(statusCode.ToString(), ex.Message));
            }
        }

        /// <summary>
        /// Pay invoice (record payment)
        /// </summary>
        /// <remarks>
        /// Records a payment for an invoice. Supports partial and full payments.
        /// If payment amount equals or exceeds total amount, invoice status changes to Paid.
        /// Cannot pay canceled invoices.
        /// 
        /// Example: POST /api/invoices/{id}/pay?amountPaid=222750
        /// </remarks>
        [HttpPost("{id}/pay")]
        public async Task<IActionResult> PayInvoice(Guid id, [FromQuery] decimal amountPaid)
        {
            try
            {
                var result = await _invoiceService.PayInvoiceAsync(id, amountPaid);
                return Ok(ApiResult<InvoiceResponseDto>.Success(
                    result,
                    "200",
                    $"Payment of {amountPaid:N0} VND recorded successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error paying invoice: {ex.Message}");
                var statusCode = ex.Data["StatusCode"] as int? ?? 500;
                return StatusCode(statusCode,
                    ApiResult<InvoiceResponseDto>.Failure(statusCode.ToString(), ex.Message));
            }
        }

        /// <summary>
        /// Cancel invoice
        /// </summary>
        /// <remarks>
        /// Cancels an outstanding invoice. Cannot cancel paid invoices.
        /// Only Admin can cancel invoices.
        /// </remarks>
        [HttpPost("{id}/cancel")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> CancelInvoice(Guid id)
        {
            try
            {
                var result = await _invoiceService.CancelInvoiceAsync(id);
                return Ok(ApiResult<InvoiceResponseDto>.Success(
                    result,
                    "200",
                    "Invoice canceled successfully."));
            }
            catch (Exception ex)
            {
                _logger.Error($"Error canceling invoice: {ex.Message}");
                var statusCode = ex.Data["StatusCode"] as int? ?? 500;
                return StatusCode(statusCode,
                    ApiResult<InvoiceResponseDto>.Failure(statusCode.ToString(), ex.Message));
            }
        }
    }
}