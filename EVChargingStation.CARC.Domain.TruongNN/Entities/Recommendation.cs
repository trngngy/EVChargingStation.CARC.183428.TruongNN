﻿using System.ComponentModel.DataAnnotations;

namespace EVChargingStation.CARC.Domain.TruongNN.Entities;

public class Recommendation : BaseEntity
{
    [Required] public Guid UserId { get; set; }

    [Required] public Guid StationAnhDHVId { get; set; }

    public Guid? ConnectorId { get; set; }

    public DateTime SuggestedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    ///     ConfidenceScore base on:
    ///     - The vehicle's compatibility with the connector type at the station.
    ///     - The distance between the user and the station.
    ///     - Historical data or user behavior patterns that suggest certain stations or connectors are frequently used
    ///     successfully.
    /// </summary>
    [Range(0, 1)]
    public decimal? ConfidenceScore { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public StationAnhDHV StationAnhDHV { get; set; } = null!;
    public Connector? Connector { get; set; }
}