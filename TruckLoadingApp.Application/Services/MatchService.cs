using TruckLoadingApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Microsoft.Extensions.Logging;

namespace TruckLoadingApp.Application.Services
{
    public class MatchService : IMatchService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MatchService> _logger;

        public MatchService(ApplicationDbContext context, ILogger<MatchService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Truck>> FindMatchingTrucks(
            decimal originLatitude, decimal originLongitude,
            decimal destinationLatitude, decimal destinationLongitude,
            decimal weight, decimal? height, decimal? width, decimal? length,
            DateTime pickupDate, DateTime deliveryDate)
        {
            const double maxDistanceKm = 200;
            var maxDistanceInMeters = maxDistanceKm * 1000;

            try
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                var originPoint = geometryFactory.CreatePoint(new Coordinate((double)originLongitude, (double)originLatitude));
                var destinationPoint = geometryFactory.CreatePoint(new Coordinate((double)destinationLongitude, (double)destinationLatitude));

                _logger.LogInformation($"FindMatchingTrucks - Origin: ({originLongitude},{originLatitude}), " +
                    $"Destination: ({destinationLongitude},{destinationLatitude}), Weight: {weight}");

                // Filter trucks by capacity and availability
                var trucksQuery = _context.Trucks
                    .Where(t => t.LoadCapacityWeight >= weight &&
                               t.AvailabilityStartDate <= pickupDate &&
                               t.AvailabilityEndDate >= deliveryDate);

                // Ensure current location is near the origin point
                trucksQuery = trucksQuery.Where(t =>
                    _context.TruckLocations.Any(tl =>
                        tl.TruckId == t.Id &&
                        tl.CurrentLocation.Distance(originPoint) <= maxDistanceInMeters));

                // Ensure at least one route has waypoints near both the origin and destination
                trucksQuery = trucksQuery.Where(t =>
                    t.Routes.Any(r => r.Waypoints.Any(w =>
                        w.Location.Distance(originPoint) <= maxDistanceInMeters)) &&
                    t.Routes.Any(r => r.Waypoints.Any(w =>
                        w.Location.Distance(destinationPoint) <= maxDistanceInMeters))
                );

                var matchingTrucks = await trucksQuery.ToListAsync();
                return matchingTrucks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding matching trucks: {Message}", ex.Message);
                return Enumerable.Empty<Truck>();
            }
        }
    }
}