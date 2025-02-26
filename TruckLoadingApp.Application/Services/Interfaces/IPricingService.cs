using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TruckLoadingApp.Domain.Enums;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface IPricingService
    {
        decimal CalculatePrice(decimal distance, decimal weight, GoodsTypeEnum goodsType);
    }
}
