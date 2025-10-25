using EVChargingStation.CARC.Application.TruongNN.Utils;
using EVChargingStation.CARC.Domain.TruongNN.DTOs.InvoiceDTOs;

namespace EVChargingStation.CARC.Application.TruongNN.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponseDto> CreateInvoiceFromSessionAsync(CreateInvoiceFromSessionDto dto);
        Task<InvoiceDetailResponseDto> GetInvoiceByIdAsync(Guid id);
        Task<PaginatedList<InvoiceResponseDto>> GetAllInvoicesAsync(GetInvoicesQueryDto query);
        Task<InvoiceResponseDto> UpdateInvoiceAsync(Guid id, UpdateInvoiceDto dto);
        Task<bool> DeleteInvoiceAsync(Guid id);
        Task<InvoiceResponseDto> PayInvoiceAsync(Guid id, decimal amountPaid);
        Task<InvoiceResponseDto> CancelInvoiceAsync(Guid id);
    }
}