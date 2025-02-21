using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruckLoadingApp.Domain.Models;

/// <summary>
/// Represents a truck.
/// </summary>
public class Truck
{
    public int Id { get; set; }

    [Required]
    public int TruckTypeId { get; set; }

    // Generic owner (Can be a Trucker or a Company)
    [Required]
    public string OwnerId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string NumberPlate { get; set; } = string.Empty;

    [Required]
    public decimal LoadCapacityWeight { get; set; }

    public decimal? Height { get; set; }
    public decimal? Width { get; set; }
    public decimal? Length { get; set; }

    /// <summary>
    /// Gets or sets the name of the driver (nullable).
    /// </summary>
    [MaxLength(256)]
    public string? DriverName { get; set; }


    /// <summary>
    /// Gets or sets the driver's contact information (nullable).
    /// </summary>
    [MaxLength(256)]
    public string? DriverContactInformation { get; set; }

    /// <summary>
    /// Gets or sets the truck's insurance information (nullable).
    /// </summary>
    [MaxLength(256)]
    public string? InsuranceInformation { get; set; }


    [Required]
    public decimal LoadCapacityVolume { get; set; }

    [Required]
    public DateTime AvailabilityStartDate { get; set; }

    [Required]
    public DateTime AvailabilityEndDate { get; set; }

    [MaxLength(256)]
    public string? PreferredRoute { get; set; }

    public bool IsApproved { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    // Navigation properties
    public TruckType? TruckType { get; set; }
    public User? Owner { get; set; } // Can be either a Trucker or a Company
    public List<Route> Routes { get; set; } = new List<Route>();
}


