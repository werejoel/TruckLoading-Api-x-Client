﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class LoadDimensions
    {
        [Key]
        [ForeignKey("Load")]
        public long LoadId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Height { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Width { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Length { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Volume { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; }

        public Load Load { get; set; } = null!;
    }
}
