
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Application.Services
{
    public class PricingService : IPricingService
    {
        public decimal CalculatePrice(decimal distance, decimal weight, GoodsTypeEnum goodsType)
        {
            // Placeholder for pricing logic
            // Replace with your actual pricing calculations based on distance, weight, and goods type
            decimal basePrice = 50;
            decimal distanceFactor = 0.1m;
            decimal weightFactor = 0.05m;

            decimal price = basePrice + (distance * distanceFactor) + (weight * weightFactor);

            // Adjust price based on goods type (example)
            switch (goodsType)
            {
                case GoodsTypeEnum.Hazardous:
                    price *= 1.5m; // Charge 50% more for hazardous goods
                    break;
                case GoodsTypeEnum.Refrigerated:
                    price *= 1.2m; // Charge 20% more for refrigerated goods
                    break;
                default:
                    break;
            }

            return price;
        }
    }
}
