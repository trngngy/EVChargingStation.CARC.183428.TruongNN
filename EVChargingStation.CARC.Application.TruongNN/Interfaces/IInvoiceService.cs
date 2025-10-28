using EVChargingStation.CARC.Infrastructure.TruongNN.Commons;
using EVChargingStation.CARC.Domain.TruongNN.DTOs.InvoiceDTOs;

namespace EVChargingStation.CARC.Application.TruongNN.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponseDto> CreateInvoiceFromSessionAsync(CreateInvoiceFromSessionDto dto);
        Task<InvoiceDetailResponseDto> GetInvoiceByIdAsync(Guid id);
        Task<Pagination<InvoiceResponseDto>> GetAllInvoicesAsync(string? search, string? sortBy, bool isDescending, int page, int pageSize);
        Task<InvoiceResponseDto> UpdateInvoiceAsync(Guid id, UpdateInvoiceDto dto);
        Task<bool> DeleteInvoiceAsync(Guid id);
        Task<InvoiceResponseDto> PayInvoiceAsync(Guid id, decimal amountPaid);
        Task<InvoiceResponseDto> CancelInvoiceAsync(Guid id);
    }
}