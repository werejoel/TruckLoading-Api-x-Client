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
    /// <summary>
    /// Gets or sets the unique identifier for the truck.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the truck type.
    /// </summary>
    [Required]
    public int TruckTypeId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the owner (User) of the truck.
    /// </summary>
    [Required]
    public string OwnerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the truck's number plate.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string NumberPlate { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the truck's load capacity in weight.
    /// </summary>
    [Required]
    public decimal LoadCapacityWeight { get; set; }

    /// <summary>
    /// Gets or sets the height of the truck (nullable).
    /// </summary>
    public decimal? Height { get; set; }

    /// <summary>
    /// Gets or sets the width of the truck (nullable).
    /// </summary>
    public decimal? Width { get; set; }

    /// <summary>
    /// Gets or sets the length of the truck (nullable).
    /// </summary>
    public decimal? Length { get; set; }

    /// <summary>
    /// Gets or sets the truck's load capacity in volume.
    /// </summary>
    [Required]
    public decimal LoadCapacityVolume { get; set; }

    /// <summary>
    /// Gets or sets the date and time the truck becomes available.
    /// </summary>
    [Required]
    public DateTime AvailabilityStartDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time the truck is no longer available.
    /// </summary>
    [Required]
    public DateTime AvailabilityEndDate { get; set; }

    /// <summary>
    /// Gets or sets the preferred route of the truck (nullable).
    /// </summary>
    [MaxLength(256)]
    public string? PreferredRoute { get; set; }

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

    /// <summary>
    /// Gets or sets a value indicating whether the truck has been approved by an administrator.
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Gets or sets the date and time the truck was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time the truck was last updated (nullable).
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    /// Navigation property to the associated TruckType.
    /// </summary>
    public TruckType? TruckType { get; set; }

    /// <summary>
    /// Navigation property to the associated Owner (User).
    /// </summary>
    public User? Owner { get; set; }

    /// <summary>
    /// Navigation property to the list of routes assigned to the truck.
    /// </summary>
    public List<Route> Routes { get; set; } = new List<Route>();
}

