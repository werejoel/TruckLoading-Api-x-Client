using System;

namespace TruckLoadingApp.Domain.Models
{
    public class PayrollPeriod
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsClosed { get; set; }
        public string Status { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string ProcessedBy { get; set; }
    }
} 