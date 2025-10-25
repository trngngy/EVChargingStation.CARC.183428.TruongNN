using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.TruongNN.DTOs.InvoiceDTOs
{
    public class CreateInvoiceFromSessionDto
    {
        [Required(ErrorMessage = "Session ID is required")]
        public Guid SessionId { get; set; }

        [Range(0, 100, ErrorMessage = "Tax rate must be between 0 and 100")]
        [DefaultValue(10)]
        public decimal TaxRate { get; set; }

        [DefaultValue(7)]
        public int DueDays { get; set; }
    }
}