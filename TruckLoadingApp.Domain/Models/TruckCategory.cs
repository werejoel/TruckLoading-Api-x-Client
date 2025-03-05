using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a category of trucks (e.g., Light-Duty, Medium-Duty, Heavy-Duty).
    /// </summary>
    public class TruckCategory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the truck category.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the truck category.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time the category was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time the category was last updated.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets whether the category is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Navigation property for truck types in this category.
        /// </summary>
        [JsonIgnore]
        public ICollection<TruckType> TruckTypes { get; set; } = new List<TruckType>();
    }
} 