using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;

/// <summary>
/// Represents a truck.
/// </summary>
/// <summary>
/// Represents a truck.
/// </summary>
public class Truck
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TruckTypeId { get; set; }

    // Generic owner (Can be a Trucker or a Company)
    [Required]
    public string OwnerId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string NumberPlate { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal LoadCapacityWeight { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal LoadCapacityVolume { get; set; }

    public decimal? Height { get; set; }
    public decimal? Width { get; set; }
    public decimal? Length { get; set; }

    [Required]
    public DateTime AvailabilityStartDate { get; set; }

    [Required]
    public DateTime AvailabilityEndDate { get; set; }

    public bool IsApproved { get; set; } = false; // 🚀 Trucks must be approved before use

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }

    // Navigation properties
    public TruckType? TruckType { get; set; }
    public User? Owner { get; set; } // Can be either a Trucker or a Company

    public TruckOperationalStatusEnum OperationalStatus { get; set; }

    // 🚀 New Relationship: Driver Assigned to This Truck
    public Driver? AssignedDriver { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AvailableCapacityWeight { get; set; }
}

