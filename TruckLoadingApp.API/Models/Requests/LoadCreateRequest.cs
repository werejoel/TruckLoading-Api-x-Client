using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.API.Models.Requests
{
    public class LoadCreateRequest
    {
        [Required]
        public decimal Weight { get; set; }

        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }

        [MaxLength(200)]
        public string? PickupAddress { get; set; }
        
        public decimal? PickupLatitude { get; set; }
        
        public decimal? PickupLongitude { get; set; }
        
        [MaxLength(200)]
        public string? DeliveryAddress { get; set; }
        
        public decimal? DeliveryLatitude { get; set; }
        
        public decimal? DeliveryLongitude { get; set; }

        [MaxLength(1000)]
        public string? SpecialRequirements { get; set; }

        [Required]
        public GoodsTypeEnum GoodsType { get; set; }

        [Required]
        public int LoadTypeId { get; set; }

        public decimal? Price { get; set; }

        [MaxLength(3)]
        public string? Currency { get; set; } // ISO Currency Code

        public string? Region { get; set; }

        public int? RequiredTruckTypeId { get; set; }

        public bool IsStackable { get; set; }

        public bool RequiresTemperatureControl { get; set; }

        public HazardousMaterialClass HazardousMaterialClass { get; set; }

        [MaxLength(500)]
        public string? HandlingInstructions { get; set; }

        public bool IsFragile { get; set; }

        public bool RequiresStackingControl { get; set; }

        [MaxLength(200)]
        public string? StackingInstructions { get; set; }

        [MaxLength(100)]
        public string? UNNumber { get; set; } // For hazardous materials

        public bool RequiresCustomsDeclaration { get; set; }

        [MaxLength(200)]
        public string? CustomsDeclarationNumber { get; set; }
    }
} 