using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        /// Gets or sets the description of the truck type.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the category ID this truck type belongs to.
        /// </summary>
        [Required]
        public int CategoryId { get; set; }

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

        /// <summary>
        /// Navigation property for the category this truck type belongs to.
        /// </summary>
        [ForeignKey("CategoryId")]
        [JsonIgnore]
        public TruckCategory? Category { get; set; }
    }
}
