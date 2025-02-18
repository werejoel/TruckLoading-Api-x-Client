using NetTopologySuite.Geometries;

using System.ComponentModel.DataAnnotations;


namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents the location of a truck.
    /// </summary>
    public class TruckLocation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the truck location record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the truck.
        /// </summary>
        [Required]
        public int TruckId { get; set; }

        /// <summary>
        /// Gets or sets the current latitude of the truck.
        /// </summary>
        [Required]
        public decimal CurrentLatitude { get; set; }

        /// <summary>
        /// Gets or sets the current longitude of the truck.
        /// </summary>
        [Required]
        public decimal CurrentLongitude { get; set; }

        /// <summary>
        /// Gets or sets the current location of the truck as a geographic point.
        /// </summary>
        public Point CurrentLocation { get; set; } = null!;

        /// <summary>
        /// Gets or sets the timestamp of when the location was recorded.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Navigation property to the associated Truck.
        /// </summary>
        public Truck Truck { get; set; } = null!;
    }
}
