using EVChargingStation.CARC.Application.TruongNN.Interfaces;
using EVChargingStation.CARC.Application.TruongNN.Interfaces.Commons;
using EVChargingStation.CARC.Application.TruongNN.Utils;
using EVChargingStation.CARC.Domain.TruongNN.DTOs.InvoiceDTOs;
using EVChargingStation.CARC.Domain.TruongNN.Entities;
using EVChargingStation.CARC.Domain.TruongNN.Enums;
using EVChargingStation.CARC.Infrastructure.TruongNN.Commons;
using EVChargingStation.CARC.Infrastructure.TruongNN.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVChargingStation.CARC.Application.TruongNN.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerService _logger;

        public InvoiceService(IUnitOfWork unitOfWork, ILoggerService logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<InvoiceResponseDto> CreateInvoiceFromSessionAsync(CreateInvoiceFromSessionDto dto)
        {
            try
            {
                var session = await _unitOfWork.Sessions.FirstOrDefaultAsync(
                     s => s.TruongNNID == dto.SessionId,
                     s => s.User
                 );

                if (session == null)
                    throw ErrorHelper.NotFound("Session not found.");

                if (session.Status != SessionStatus.Stopped)
                    throw ErrorHelper.BadRequest("Session is not complete.");

                if (!session.Cost.HasValue)
                    throw ErrorHelper.BadRequest("Session cost is not calculated yet.");

                // Kiểm tra xem session đã được gán vào invoice chưa
                if (session.InvoiceTruongNNId.HasValue)
                    throw ErrorHelper.Conflict("This session has already been invoiced.");

                // Calculate amounts
                var subtotal = session.Cost.Value;
                var taxAmount = subtotal * (dto.TaxRate / 100);
                var totalAmount = subtotal + taxAmount;

                // Create invoice
                var invoice = new InvoiceTruongNN
                {
                    UserId = session.UserId,
                    SessionId = session.TruongNNID,
                    PeriodStart = session.StartTime,
                    PeriodEnd = session.EndTime ?? DateTime.UtcNow,
                    Status = InvoiceStatus.Outstanding,
                    SubtotalAmount = subtotal,
                    TaxAmount = taxAmount,
                    TotalAmount = totalAmount,
                    AmountPaid = 0,
                    DueDate = DateTime.UtcNow.AddDays(dto.DueDays),
                    IssuedAt = DateTime.UtcNow
                };

                await _unitOfWork.Invoices.AddAsync(invoice);
                await _unitOfWork.SaveChangesAsync();

                // Cập nhật InvoiceTruongNNId cho session
                session.InvoiceTruongNNId = invoice.TruongNNID;
                await _unitOfWork.Sessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.Success($"Invoice created from session {dto.SessionId}");

                // Reload with user
                invoice.User = session.User;
                return MapToResponseDto(invoice);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error creating invoice from session: {ex.Message}");
                throw;
            }
        }

        public async Task<InvoiceDetailResponseDto> GetInvoiceByIdAsync(Guid id)
        {
            try
            {
                var invoice = await _unitOfWork.Invoices.FirstOrDefaultAsync(
                    i => i.TruongNNID == id && !i.IsDeleted,
                    i => i.User,
                    i => i.Payments
                );

                if (invoice == null)
                    throw ErrorHelper.NotFound("Invoice not found.");

                // Load session if exists
                Session? session = null;
                if (invoice.SessionId.HasValue)
                {
                    session = await _unitOfWork.Sessions
                        .GetQueryable()
                        .Include(s => s.Connector)
                            .ThenInclude(c => c.StationAnhDHV)
                        .FirstOrDefaultAsync(s => s.TruongNNID == invoice.SessionId.Value);
                }

                return MapToDetailResponseDto(invoice, session);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting invoice by ID: {ex.Message}");
                throw;
            }
        }

        // Sửa hàm ban đầu, giữ nguyên chữ ký và logic lọc/sắp xếp
        public async Task<Pagination<InvoiceResponseDto>> GetAllInvoicesAsync(
            string? search,
            string? sortBy,
            bool isDescending,
            int page,
            int pageSize)
        {
            try
            {
                var invoicesQuery = _unitOfWork.Invoices
                    .GetQueryable()
                    .Include(i => i.User)
                    .Where(i => !i.IsDeleted);


                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchLower = search.ToLower();
                    invoicesQuery = invoicesQuery.Where(i =>
                        i.User.Email.ToLower().Contains(searchLower) ||
                        (i.User.FirstName + " " + i.User.LastName).ToLower().Contains(searchLower));
                }

                // 3. Sắp xếp (Ánh xạ isDescending)
                var sortOrder = isDescending ? "desc" : "asc";
                var sortKey = (sortBy ?? string.Empty).ToLower();

                invoicesQuery = sortKey switch
                {
                    "periodstart" => isDescending
                        ? invoicesQuery.OrderByDescending(i => i.PeriodStart)
                        : invoicesQuery.OrderBy(i => i.PeriodStart),
                    "periodend" => isDescending
                        ? invoicesQuery.OrderByDescending(i => i.PeriodEnd)
                        : invoicesQuery.OrderBy(i => i.PeriodEnd),
                    "totalamount" => isDescending
                        ? invoicesQuery.OrderByDescending(i => i.TotalAmount)
                        : invoicesQuery.OrderBy(i => i.TotalAmount),
                    "status" => isDescending
                        ? invoicesQuery.OrderByDescending(i => i.Status)
                        : invoicesQuery.OrderBy(i => i.Status),
                    "duedate" => isDescending
                        ? invoicesQuery.OrderByDescending(i => i.DueDate)
                        : invoicesQuery.OrderBy(i => i.DueDate),
                    _ => isDescending
                        ? invoicesQuery.OrderByDescending(i => i.CreatedAt)
                        : invoicesQuery.OrderBy(i => i.CreatedAt)
                };

                var totalInvoices = await invoicesQuery.CountAsync();

                var pagedInvoices = await invoicesQuery
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

                var invoiceDtos = pagedInvoices.Select(MapToResponseDto).ToList();

                return new Pagination<InvoiceResponseDto>(
                    invoiceDtos,
                    totalInvoices,
                    page,
                    pageSize);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting all invoices: {ex.Message}");
                throw new Exception("An unexpected error occurred while fetching invoices.");
            }
        }

        public async Task<InvoiceResponseDto> UpdateInvoiceAsync(Guid id, UpdateInvoiceDto dto)
        {
            try
            {
                var invoice = await _unitOfWork.Invoices.FirstOrDefaultAsync(
                   i => i.TruongNNID == id && !i.IsDeleted,
                   i => i.User
                );

                if (invoice == null)
                    throw ErrorHelper.NotFound("Invoice not found.");

                // Update fields if provided
                if (dto.PeriodStart.HasValue)
                    invoice.PeriodStart = dto.PeriodStart.Value;

                if (dto.PeriodEnd.HasValue)
                {
                    if (dto.PeriodEnd.Value < invoice.PeriodStart)
                        throw ErrorHelper.BadRequest("Invoice periodend is wrong.");
                    invoice.PeriodEnd = dto.PeriodEnd.Value;
                }

                if (dto.Status.HasValue)
                    invoice.Status = dto.Status.Value;

                if (dto.SubtotalAmount.HasValue)
                    invoice.SubtotalAmount = dto.SubtotalAmount.Value;

                if (dto.TaxAmount.HasValue)
                    invoice.TaxAmount = dto.TaxAmount.Value;

                if (dto.AmountPaid.HasValue)
                    invoice.AmountPaid = dto.AmountPaid.Value;

                if (dto.DueDate.HasValue)
                    invoice.DueDate = dto.DueDate.Value;

                // Recalculate total amount
                invoice.TotalAmount = invoice.SubtotalAmount + invoice.TaxAmount;

                // Update status based on amount paid
                if (invoice.AmountPaid >= invoice.TotalAmount)
                    invoice.Status = InvoiceStatus.Paid;

                await _unitOfWork.Invoices.Update(invoice);
                await _unitOfWork.SaveChangesAsync();

                _logger.Success($"Invoice {id} updated successfully");

                return MapToResponseDto(invoice);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating invoice: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteInvoiceAsync(Guid id)
        {
            try
            {
                var invoice = await _unitOfWork.Invoices.FirstOrDefaultAsync(
                    i => i.TruongNNID == id && !i.IsDeleted,
                    i => i.User
                 );

                if (invoice == null)
                    throw ErrorHelper.NotFound("Invoice not found.");

                if (invoice.Status == InvoiceStatus.Paid)
                    throw ErrorHelper.BadRequest("Invoice has paid.");

                await _unitOfWork.Invoices.SoftRemove(invoice);
                await _unitOfWork.SaveChangesAsync();

                _logger.Success($"Invoice {id} deleted successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting invoice: {ex.Message}");
                throw;
            }
        }

        public async Task<InvoiceResponseDto> PayInvoiceAsync(Guid id, decimal amountPaid)
        {
            try
            {
                if (amountPaid <= 0)
                    throw ErrorHelper.BadRequest("Payment amount must be greater than 0.");

                var invoice = await _unitOfWork.Invoices.FirstOrDefaultAsync(
                    i => i.TruongNNID == id && !i.IsDeleted,
                    i => i.User
                 );

                if (invoice == null)
                    throw ErrorHelper.NotFound("Invoice not found.");

                if (invoice.Status == InvoiceStatus.Canceled)
                    throw ErrorHelper.BadRequest("Invoice has canceled.");

                if (invoice.Status == InvoiceStatus.Paid)
                    throw ErrorHelper.BadRequest("Invoice has paid.");

                // Update amount paid
                invoice.AmountPaid += amountPaid;

                // Check if fully paid
                if (invoice.AmountPaid >= invoice.TotalAmount)
                {
                    invoice.Status = InvoiceStatus.Paid;
                    invoice.AmountPaid = invoice.TotalAmount; // Cap at total amount
                }

                await _unitOfWork.Invoices.Update(invoice);
                await _unitOfWork.SaveChangesAsync();

                _logger.Success($"Invoice {id} payment recorded: {amountPaid:N0} VND");

                return MapToResponseDto(invoice);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error processing payment for invoice: {ex.Message}");
                throw;
            }
        }

        public async Task<InvoiceResponseDto> CancelInvoiceAsync(Guid id)
        {
            try
            {
                var invoice = await _unitOfWork.Invoices.FirstOrDefaultAsync(
                    i => i.TruongNNID == id && !i.IsDeleted,
                    i => i.User
                 );

                if (invoice == null)
                    throw ErrorHelper.NotFound("Invoice not found.");

                if (invoice.Status == InvoiceStatus.Paid)
                    throw ErrorHelper.BadRequest("Cannot cancel a paid invoice.");

                if (invoice.Status == InvoiceStatus.Canceled)
                    throw ErrorHelper.BadRequest("Invoice has be canceled");

                invoice.Status = InvoiceStatus.Canceled;

                if (invoice.SessionId.HasValue)
                {
                    var session = await _unitOfWork.Sessions.FirstOrDefaultAsync(
                        s => s.TruongNNID == invoice.SessionId.Value
                    );

                    if (session != null)
                    {
                        session.InvoiceTruongNNId = null;
                        await _unitOfWork.Sessions.Update(session);
                    }
                }

                await _unitOfWork.Invoices.Update(invoice);
                await _unitOfWork.SaveChangesAsync();

                _logger.Success($"Invoice {id} canceled successfully");

                return MapToResponseDto(invoice);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error canceling invoice: {ex.Message}");
                throw;
            }
        }

        #region Helper Methods

        private InvoiceResponseDto MapToResponseDto(InvoiceTruongNN invoice)
        {
            var isOverdue = invoice.Status == InvoiceStatus.Outstanding &&
                           invoice.DueDate.HasValue &&
                           invoice.DueDate.Value < DateTime.UtcNow;

            return new InvoiceResponseDto
            {
                Id = invoice.TruongNNID,
                UserId = invoice.UserId,
                UserEmail = invoice.User?.Email ?? "N/A",
                UserFullName = invoice.User != null ? $"{invoice.User.FirstName} {invoice.User.LastName}" : "N/A",
                SessionId = invoice.SessionId,
                PeriodStart = invoice.PeriodStart,
                PeriodEnd = invoice.PeriodEnd,
                Status = invoice.Status,
                StatusDisplay = GetStatusDisplay(invoice.Status),
                SubtotalAmount = invoice.SubtotalAmount,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,
                AmountPaid = invoice.AmountPaid,
                AmountDue = invoice.TotalAmount - invoice.AmountPaid,
                DueDate = invoice.DueDate,
                IssuedAt = invoice.IssuedAt,
                IsOverdue = isOverdue,
                CreatedAt = invoice.CreatedAt,
                UpdatedAt = invoice.UpdatedAt
            };
        }

        private InvoiceDetailResponseDto MapToDetailResponseDto(InvoiceTruongNN invoice, Session? session)
        {
            var isOverdue = invoice.Status == InvoiceStatus.Outstanding &&
                           invoice.DueDate.HasValue &&
                           invoice.DueDate.Value < DateTime.UtcNow;

            SessionSummaryDto? sessionDto = null;
            if (session != null)
            {
                sessionDto = new SessionSummaryDto
                {
                    Id = session.TruongNNID,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime,
                    EnergyKwh = session.EnergyKwh,
                    Cost = session.Cost,
                    ConnectorType = session.Connector?.ConnectorType.ToString() ?? "N/A",
                    StationName = session.Connector?.StationAnhDHV?.Name ?? "N/A"
                };
            }

            return new InvoiceDetailResponseDto
            {
                Id = invoice.TruongNNID,
                UserId = invoice.UserId,
                UserEmail = invoice.User?.Email ?? "N/A",
                UserFullName = invoice.User != null ? $"{invoice.User.FirstName} {invoice.User.LastName}" : "N/A",
                UserPhone = invoice.User?.Phone ?? "N/A",
                SessionId = invoice.SessionId,
                PeriodStart = invoice.PeriodStart,
                PeriodEnd = invoice.PeriodEnd,
                Status = invoice.Status,
                StatusDisplay = GetStatusDisplay(invoice.Status),
                SubtotalAmount = invoice.SubtotalAmount,
                TaxAmount = invoice.TaxAmount,
                TotalAmount = invoice.TotalAmount,
                AmountPaid = invoice.AmountPaid,
                AmountDue = invoice.TotalAmount - invoice.AmountPaid,
                DueDate = invoice.DueDate,
                IssuedAt = invoice.IssuedAt,
                IsOverdue = isOverdue,
                Session = sessionDto,
                Payments = invoice.Payments?.Select(p => new PaymentSummaryDto
                {
                    Id = p.TruongNNID,
                    Amount = p.Amount,
                    Status = p.Status.ToString(),
                    CreatedAt = p.CreatedAt
                }).ToList() ?? new List<PaymentSummaryDto>(),
                CreatedAt = invoice.CreatedAt,
                UpdatedAt = invoice.UpdatedAt
            };
        }

        private string GetStatusDisplay(InvoiceStatus status)
        {
            return status switch
            {
                InvoiceStatus.Outstanding => "Chưa thanh toán",
                InvoiceStatus.Paid => "Đã thanh toán",
                InvoiceStatus.Canceled => "Đã hủy",
                _ => "Không xác định"
            };
        }

        #endregion
    }
}