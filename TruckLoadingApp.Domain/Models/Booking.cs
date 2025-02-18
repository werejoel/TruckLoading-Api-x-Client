using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a booking for a load with a specific truck.
    /// </summary>
    public class Booking
    {
        /// <summary>
        /// Gets or sets the unique identifier for the booking.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the load being booked.
        /// </summary>
        [Required]
        public long LoadId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the truck assigned to the booking.
        /// </summary>
        [Required]
        public int TruckId { get; set; }

        /// <summary>
        /// Gets or sets the date and time the booking was made.
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets the status of the booking.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the date and time the booking was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time the booking was last updated (nullable).
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Navigation property to the associated Load.
        /// </summary>
        public Load? Load { get; set; }
        /// <summary>
        /// Navigation property to the associated Truck.
        /// </summary>
        public Truck? Truck { get; set; }
    }
}
