using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Models.Responses
{
    public class BookingResponse
    {
        public long Id { get; set; }
        public long LoadId { get; set; }
        public long TruckId { get; set; }
        public string? TruckRegistrationNumber { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPhoneNumber { get; set; }
        public string? TruckCompanyName { get; set; }
        public decimal AgreedPrice { get; set; }
        public string PricingModel { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public BookingStatusEnum Status { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? PickupDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ActualPickupDate { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public string? Notes { get; set; }
        public bool IsExpressBooking { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        // Load summary information
        public decimal LoadWeight { get; set; }
        public string? LoadDescription { get; set; }
        public GoodsTypeEnum LoadGoodsType { get; set; }
        
        // Current location information
        public decimal? CurrentLatitude { get; set; }
        public decimal? CurrentLongitude { get; set; }
        public DateTime? LastLocationUpdate { get; set; }
        public string? EstimatedTimeOfArrival { get; set; }
    }
} 