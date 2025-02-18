using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLoadingApp.Application.Services
{
    public interface IMatchService
    {
        Task<IEnumerable<Truck>> FindMatchingTrucks(
            decimal originLatitude,
            decimal originLongitude,
            decimal destinationLatitude,
            decimal destinationLongitude,
            decimal weight,
            decimal? height,
            decimal? width,
            decimal? length,
            DateTime pickupDate,
            DateTime deliveryDate
        );
    }
}
