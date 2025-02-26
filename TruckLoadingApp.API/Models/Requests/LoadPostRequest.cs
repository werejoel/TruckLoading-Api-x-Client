using System.ComponentModel.DataAnnotations;
using TruckLoadingApp.Domain.Enums;

public class LoadPostRequest
{
    [Required]
    public decimal Weight { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public decimal? Height { get; set; }
    public decimal? Width { get; set; }
    public decimal? Length { get; set; }

    public string? SpecialRequirements { get; set; }

    [Required]
    public DateTime PickupDate { get; set; }

    [Required]
    public DateTime DeliveryDate { get; set; }

    [Required]
    public GoodsTypeEnum GoodsType { get; set; }
    public int? RequiredTruckTypeId { get; internal set; }
}
