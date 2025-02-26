using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Truck> Trucks { get; set; }
        public DbSet<TruckType> TruckTypes { get; set; }
        public DbSet<Load> Loads { get; set; }
        public DbSet<LoadStop> LoadStops { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Waypoint> Waypoints { get; set; }
        public DbSet<TruckLocation> TruckLocations { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<LoadType> LoadTypes { get; set; }
        public DbSet<PriceFactors> PriceFactors { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<TruckLocationHistory> TruckLocationHistories { get; set; }
        public DbSet<Ratings> Ratings { get; set; }
        public DbSet<LoadDimensions> LoadDimensions { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<ContactDetails> ContactDetails { get; set; }
        public DbSet<UserLocation> UserLocations { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new TruckConfiguration());
            builder.ApplyConfiguration(new LoadConfiguration());
            builder.ApplyConfiguration(new DriverConfiguration());
            builder.ApplyConfiguration(new BookingConfiguration());
            builder.ApplyConfiguration(new TruckTypeConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RouteConfiguration());
            builder.ApplyConfiguration(new WaypointConfiguration());
            builder.ApplyConfiguration(new TruckLocationConfiguration());
            builder.ApplyConfiguration(new DocumentsConfiguration());
            builder.ApplyConfiguration(new LoadTypeConfiguration());
            builder.ApplyConfiguration(new PriceFactorsConfiguration());
            builder.ApplyConfiguration(new PaymentDetailsConfiguration());
            builder.ApplyConfiguration(new TruckLocationHistoryConfiguration());
            builder.ApplyConfiguration(new RatingsConfiguration());
            builder.ApplyConfiguration(new LoadDimensionsConfiguration());
            builder.ApplyConfiguration(new RegionConfiguration());
            builder.ApplyConfiguration(new ContactDetailsConfiguration());
            builder.ApplyConfiguration(new UserLocationConfiguration());
            builder.ApplyConfiguration(new LoadStopConfiguration());
        }
    }
}