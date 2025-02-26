using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace TruckLoadingApp.Domain.Models
{
    public class TruckLocationHistory
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public int TruckId { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        public decimal? Speed { get; set; }
        public int? Heading { get; set; }

        [Required]
        public DateTime LocationTimestamp { get; set; }

        public Point Location { get; set; } = null!;

        [ForeignKey("TruckId")]
        public Truck Truck { get; set; } = null!;
    }
}
