using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface IPricingService
    {
        decimal CalculatePrice(decimal distance, decimal weight, GoodsTypeEnum goodsType);
    }
}
