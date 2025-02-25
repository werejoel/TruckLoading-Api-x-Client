using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Domain.Models
{
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

        public string? Description { get; set; }

        public DateTime PickupDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public string? SpecialRequirements { get; set; }
        public User? Shipper { get; set; }

        public GoodsTypeEnum GoodsType { get; set; }

        [ForeignKey("LoadTypeId")]
        public int LoadTypeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        [MaxLength(3)]
        public string? Currency { get; set; } // ISO Currency Code
        public LoadStatusEnum Status { get; set; }
        public int? RequiredTruckTypeId { get; set; }
        public LoadDimensions? LoadDimensions { get; set; }
    }
}
