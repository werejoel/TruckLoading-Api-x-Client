using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Domain.Models
{
    /// <summary>
    /// Represents a load to be transported
    /// </summary>
    public class Load
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string ShipperId { get; set; } = string.Empty;

        [Required]
        public decimal Weight { get; set; }

        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime PickupDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        // Add pickup location details
        [MaxLength(200)]
        public string? PickupAddress { get; set; }
        
        public decimal? PickupLatitude { get; set; }
        
        public decimal? PickupLongitude { get; set; }
        
        // Add delivery location details
        [MaxLength(200)]
        public string? DeliveryAddress { get; set; }
        
        public decimal? DeliveryLatitude { get; set; }
        
        public decimal? DeliveryLongitude { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        [MaxLength(1000)]
        public string? SpecialRequirements { get; set; }

        [ForeignKey("ShipperId")]
        public User? Shipper { get; set; }

        public GoodsTypeEnum GoodsType { get; set; }

        [ForeignKey("LoadTypeId")]
        public int LoadTypeId { get; set; }
        public LoadType LoadType { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [MaxLength(3)]
        public string? Currency { get; set; } // ISO Currency Code

        public string? Region { get; set; }
        public LoadStatusEnum Status { get; set; }

        public int? RequiredTruckTypeId { get; set; }
        [ForeignKey("RequiredTruckTypeId")]
        public TruckType? RequiredTruckType { get; set; }

        public LoadDimensions? LoadDimensions { get; set; }

        // New properties for load management improvements
        public bool IsStackable { get; set; }

        public bool RequiresTemperatureControl { get; set; }

        public HazardousMaterialClass HazardousMaterialClass { get; set; }

        [MaxLength(500)]
        public string? HandlingInstructions { get; set; }

        public LoadTemperatureRequirement? TemperatureRequirement { get; set; }

        public ICollection<LoadLoadTag> LoadTags { get; set; } = new List<LoadLoadTag>();

        public bool IsFragile { get; set; }

        public bool RequiresStackingControl { get; set; }

        [MaxLength(200)]
        public string? StackingInstructions { get; set; }

        [MaxLength(100)]
        public string? UNNumber { get; set; } // For hazardous materials

        public bool RequiresCustomsDeclaration { get; set; }

        [MaxLength(200)]
        public string? CustomsDeclarationNumber { get; set; }
        
        /// <summary>
        /// Determines if the load is late for delivery.
        /// A load is considered late if the current date is past the delivery date
        /// and the load is not in a delivered or cancelled state.
        /// </summary>
        [NotMapped]
        public bool IsLate => DateTime.UtcNow > DeliveryDate && 
                              Status != LoadStatusEnum.Delivered && 
                              Status != LoadStatusEnum.Cancelled;
    }
}
