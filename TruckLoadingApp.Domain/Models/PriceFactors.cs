using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLoadingApp.Domain.Models
{
    public class PriceFactors
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal DistanceRate { get; set; }

        [Required]
        public decimal WeightRate { get; set; }

        [Required]
        public int LoadTypeId { get; set; }

        [Required]
        public int RegionId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        //Navigation Properties
        [Required]
        public LoadType LoadType { get; set; } = null!;
        [Required]
        public Region Region { get; set; } = null!;
    }
}
