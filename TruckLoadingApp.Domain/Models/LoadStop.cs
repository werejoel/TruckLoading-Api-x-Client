using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a stop along the route of a load.
    /// </summary>
    public class LoadStop
    {
        /// <summary>
        /// Gets or sets the unique identifier for the load stop.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated load.
        /// </summary>
        [Required]
        public long BookingId { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the stop.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(9,6)")]
        public decimal StopLatitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the stop.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(9,6)")]
        public decimal StopLongitude { get; set; }

        /// <summary>
        /// Gets or sets the order of the stop in the route sequence.
        /// </summary>
        [Required]
        public int StopOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time the truck is expected to arrive at the stop.
        /// </summary>
        [Required]
        public DateTime StopDate { get; set; }

        /// <summary>
        /// Gets or sets the location of the stop as a geographic point.
        /// </summary>
        public Point Location { get; set; } = null!;

        /// <summary>
        /// Gets or sets the associated Booking
        /// </summary>
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!;
        public DateTime? ActualArrivalDate { get; set; }
        public DateTime? ActualDepartureDate { get; set; }
        public int StopSequence { get; set; }
    }
}
