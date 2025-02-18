using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a load to be transported.
    /// </summary>
    public class Load
    {
        /// <summary>
        /// Gets or sets the unique identifier for the load.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the shipper posting the load.
        /// </summary>
        [Required]
        public string ShipperId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the latitude of the load's origin.
        /// </summary>
        [Required]
        public decimal OriginLatitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the load's origin.
        /// </summary>
        [Required]
        public decimal OriginLongitude { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the load's destination.
        /// </summary>
        [Required]
        public decimal DestinationLatitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the load's destination.
        /// </summary>
        [Required]
        public decimal DestinationLongitude { get; set; }

        /// <summary>
        /// Gets or sets the origin location as a geographic point.
        /// </summary>
        public Point OriginLocation { get; set; }

        /// <summary>
        /// Gets or sets the destination location as a geographic point.
        /// </summary>
        public Point DestinationLocation { get; set; }

        /// <summary>
        /// Gets or sets a list of stops along the route.
        /// </summary>
        public List<LoadStop>? Stops { get; set; }

        /// <summary>
        /// Gets or sets the weight of the load.
        /// </summary>
        [Required]
        public decimal Weight { get; set; }

        /// <summary>
        /// Gets or sets the height of the load (nullable).
        /// </summary>
        public decimal? Height { get; set; }

        /// <summary>
        /// Gets or sets the width of the load (nullable).
        /// </summary>
        public decimal? Width { get; set; }

        /// <summary>
        /// Gets or sets the length of the load (nullable).
        /// </summary>
        public decimal? Length { get; set; }

        /// <summary>
        /// Gets or sets a description of the load.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the date and time the load is available for pickup.
        /// </summary>
        public DateTime PickupDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time the load must be delivered.
        /// </summary>
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time the load was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time the load was last updated (nullable).
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Navigation property to the associated Shipper (User).
        /// </summary>
        public User? Shipper { get; set; }
    }
}
