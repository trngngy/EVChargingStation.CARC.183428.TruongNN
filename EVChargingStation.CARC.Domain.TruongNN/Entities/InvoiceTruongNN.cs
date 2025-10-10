﻿using EVChargingStation.CARC.Domain.TruongNN.Enums;
using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.TruongNN.Entities
{
    public class InvoiceTruongNN : BaseEntity
    {
        [Required]
        public Guid UserId { get; set; }

        public Guid? SessionId { get; set; }

        [Required]
        public DateTime PeriodStart { get; set; }

        [Required]
        public DateTime PeriodEnd { get; set; }

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        [Range(0, 1000000)]
        public decimal SubtotalAmount { get; set; } = 0;

        [Range(0, 1000000)]
        public decimal TaxAmount { get; set; } = 0;

        [Range(0, 1000000)]
        public decimal TotalAmount { get; set; } = 0;

        [Range(0, 1000000)]
        public decimal AmountPaid { get; set; } = 0;

        public DateTime? DueDate { get; set; }

        public DateTime? IssuedAt { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}