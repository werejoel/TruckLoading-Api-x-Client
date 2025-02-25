using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Domain.Models
{
    public class Documents
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long EntityId { get; set; }

        [Required]
        [MaxLength(50)]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        public DocumentTypeEnum DocumentType { get; set; }

        [Required]
        [MaxLength(500)]
        public string DocumentUrl { get; set; } = string.Empty;

        public DateTime? ExpiryDate { get; set; }

        public bool IsVerified { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }
    }
}
