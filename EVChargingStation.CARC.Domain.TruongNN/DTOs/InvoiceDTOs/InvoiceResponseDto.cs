using EVChargingStation.CARC.Domain.TruongNN.Enums;

namespace EVChargingStation.CARC.Domain.TruongNN.DTOs.InvoiceDTOs
{
    public class InvoiceResponseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public Guid? SessionId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public InvoiceStatus Status { get; set; }
        public string StatusDisplay { get; set; } = string.Empty;
        public decimal SubtotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? IssuedAt { get; set; }
        public bool IsOverdue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}