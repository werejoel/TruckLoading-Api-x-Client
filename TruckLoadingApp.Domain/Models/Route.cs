using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a defined route for a truck.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Gets or sets the unique identifier for the route.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the truck assigned to the route.
        /// </summary>
        [Required]
        public int TruckId { get; set; }

        /// <summary>
        /// Gets or sets a descriptive name for the route.
        /// </summary>
        [Required]
        public string RouteName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time the route was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time the route was last updated (nullable).
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Navigation property to the associated Truck.
        /// </summary>
        public Truck Truck { get; set; } = null!;

        /// <summary>
        /// Navigation property to the list of waypoints along the route.
        /// </summary>
        public List<Waypoint> Waypoints { get; set; } = new List<Waypoint>();
    }
}
