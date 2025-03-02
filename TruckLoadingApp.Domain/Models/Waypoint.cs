using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a waypoint along a route.
    /// </summary>
    public class Waypoint
    {
        /// <summary>
        /// Gets or sets the unique identifier for the waypoint.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated route.
        /// </summary>
        [Required]
        public long RouteId { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the waypoint.
        /// </summary>
        [Required]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the waypoint.
        /// </summary>
        [Required]
        public decimal Longitude { get; set; }

        /// <summary>
        /// Gets or sets the location of the waypoint as a geographic point.
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// Gets or sets the sequence number of the stop.
        /// </summary>
        [Required]
        public int StopOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of arrival at the waypoint.
        /// </summary>
        public DateTime ArrivalDate { get; set; }

        /// <summary>
        /// Navigation property to the associated Route.
        /// </summary>
        public Route Route { get; set; } = null!;
    }
}
