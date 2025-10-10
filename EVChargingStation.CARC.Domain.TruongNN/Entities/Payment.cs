using System.ComponentModel.DataAnnotations;
using EVChargingStation.CARC.Domain.TruongNN.Enums;

namespace EVChargingStation.CARC.Domain.TruongNN.Entities;

public class Payment : BaseEntity
{
    [Required] [Range(0, 1000000)] public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? Note { get; set; }

    public Guid? InvoiceId { get; set; }

    public Guid? SessionId { get; set; }

    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public InvoiceTruongNN? Invoice { get; set; }
    public Session? Session { get; set; }
}