using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EVChargingStation.CARC.Domain.TruongNN.Enums;

namespace EVChargingStation.CARC.Domain.TruongNN.DTOs.InvoiceDTOs
{
    public class UpdateInvoiceDto
    {
        public DateTime? PeriodStart { get; set; }

        public DateTime? PeriodEnd { get; set; }

        [DefaultValue(InvoiceStatus.Outstanding)]
        public InvoiceStatus? Status { get; set; }

        [Range(0, 1000000, ErrorMessage = "Subtotal amount must be between 0 and 1,000,000")]
        public decimal? SubtotalAmount { get; set; }

        [Range(0, 1000000, ErrorMessage = "Tax amount must be between 0 and 1,000,000")]
        public decimal? TaxAmount { get; set; }

        [Range(0, 1000000, ErrorMessage = "Amount paid must be between 0 and 1,000,000")]
        public decimal? AmountPaid { get; set; }

        public DateTime? DueDate { get; set; }
    }
}