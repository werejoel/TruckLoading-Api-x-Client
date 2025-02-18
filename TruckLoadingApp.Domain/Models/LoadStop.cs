using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

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
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated load.
        /// </summary>
        public long LoadId { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the stop.
        /// </summary>
        [Required]
        public decimal StopLatitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the stop.
        /// </summary>
        [Required]
        public decimal StopLongitude { get; set; }

        /// <summary>
        /// Gets or sets the order of the stop in the route sequence.
        /// </summary>
        [Required]
        public int Order { get; set; }

        /// <summary>
        /// Gets or sets the date and time the truck is expected to arrive at the stop.
        /// </summary>
        [Required]
        public DateTime StopDate { get; set; }

        /// <summary>
        /// Gets or sets the location of the stop as a geographic point.
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// Gets or sets the associated Load.
        /// </summary>
        public Load Load { get; set; } = null!;
    }
}
