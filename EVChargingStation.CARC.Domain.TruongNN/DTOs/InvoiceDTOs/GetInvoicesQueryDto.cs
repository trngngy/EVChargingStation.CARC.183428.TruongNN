using System.ComponentModel;

namespace EVChargingStation.CARC.Domain.TruongNN.DTOs.InvoiceDTOs
{
    public class GetInvoicesQueryDto
    {
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;

        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; } // Search by user email or name

        public int? Status { get; set; } // Filter by status (1=Outstanding, 2=Paid, 3=Canceled)

        public Guid? UserId { get; set; } // Filter by user ID

        public bool? IsOverdue { get; set; } // Filter overdue invoices

        [DefaultValue("CreatedAt")]
        public string SortBy { get; set; } = "CreatedAt"; // Sort field

        [DefaultValue("desc")]
        public string SortOrder { get; set; } = "desc"; // asc or desc
    }
}