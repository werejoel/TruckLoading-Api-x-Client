﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Column(TypeName = "VARCHAR(100)")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time the truck type was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;  // Auto-set on creation

        /// <summary>
        /// Gets or sets the date and time the truck type was last updated (nullable).
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Indicates whether the truck type is active (for soft deletion).
        /// </summary>
        public bool IsActive { get; set; } = true;  // ✅ Soft delete mechanism
    }
}
