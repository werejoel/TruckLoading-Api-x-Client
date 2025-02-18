using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents the type of a truck.
    /// </summary>
    public class TruckType
    {
        /// <summary>
        /// Gets or sets the unique identifier for the truck type.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the truck type.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time the truck type was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time the truck type was last updated (nullable).
        /// </summary>
        public DateTime? UpdatedDate { get; set; }
    }
}
