using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Models.Responses
{
    public class LoadResponse
    {
        public long Id { get; set; }
        public string ShipperId { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public string? Description { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? SpecialRequirements { get; set; }
        public GoodsTypeEnum GoodsType { get; set; }
        public int LoadTypeId { get; set; }
        public string? LoadTypeName { get; set; }
        public decimal? Price { get; set; }
        public string? Currency { get; set; }
        public string? Region { get; set; }
        public int? RequiredTruckTypeId { get; set; }
        public string? RequiredTruckTypeName { get; set; }
        public bool IsStackable { get; set; }
        public bool RequiresTemperatureControl { get; set; }
        public HazardousMaterialClass HazardousMaterialClass { get; set; }
        public string? HandlingInstructions { get; set; }
        public bool IsFragile { get; set; }
        public bool RequiresStackingControl { get; set; }
        public string? StackingInstructions { get; set; }
        public string? UNNumber { get; set; }
        public bool RequiresCustomsDeclaration { get; set; }
        public string? CustomsDeclarationNumber { get; set; }
        public string? PickupAddress { get; set; }
        public decimal? PickupLatitude { get; set; }
        public decimal? PickupLongitude { get; set; }
        public string? DeliveryAddress { get; set; }
        public decimal? DeliveryLatitude { get; set; }
        public decimal? DeliveryLongitude { get; set; }
        public LoadStatusEnum Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public BookingStatusEnum? CurrentBookingStatus { get; set; }
        public long? CurrentBookingId { get; set; }
    }
} 