using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
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


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TruckType>()
    .HasIndex(tt => tt.Name)
    .IsUnique();  // ✅ Ensures TruckType names are unique


            // Configure spatial properties for Load
            builder.Entity<Load>()
                .Property(l => l.OriginLocation)
                .HasColumnType("geography");

            builder.Entity<Load>()
                .Property(l => l.DestinationLocation)
                .HasColumnType("geography");

            // Keep the original latitude/longitude columns for backward compatibility
            builder.Entity<Load>()
                .Property(l => l.OriginLatitude)
                .HasColumnType("decimal(9,6)");

            builder.Entity<Load>()
                .Property(l => l.OriginLongitude)
                .HasColumnType("decimal(9,6)");

            builder.Entity<Load>()
                .Property(l => l.DestinationLatitude)
                .HasColumnType("decimal(9,6)");
            builder.Entity<Waypoint>()
                .Property(l => l.Latitude)
                .HasColumnType("decimal(9,6)");

            builder.Entity<Waypoint>()
               .Property(l => l.Longitude)
               .HasColumnType("decimal(9,6)");

            builder.Entity<Load>()
                .Property(l => l.DestinationLongitude)
                .HasColumnType("decimal(9,6)");

            builder.Entity<Route>()
    .HasOne(r => r.Truck)
    .WithMany(t => t.Routes)
    .HasForeignKey(r => r.TruckId)
    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Waypoint>()
                .Property(w => w.Location)
                .HasColumnType("geography");

            builder.Entity<Waypoint>()
                .HasOne(w => w.Route)
                .WithMany(r => r.Waypoints)
                .HasForeignKey(w => w.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure spatial properties for LoadStop
            builder.Entity<LoadStop>()
                .Property(ls => ls.Location)
                .HasColumnType("geography");
            builder.Entity<LoadStop>()
    .Property(ls => ls.StopLatitude)
    .HasColumnType("decimal(9,6)"); // ✅ Precision: 9 digits total, 6 after the decimal

            builder.Entity<LoadStop>()
                .Property(ls => ls.StopLongitude)
                .HasColumnType("decimal(9,6)");
            // Configure other decimal properties
            builder.Entity<Load>()
                .Property(l => l.Weight)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Load>()
                .Property(l => l.Height)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Load>()
                .Property(l => l.Width)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Load>()
                .Property(l => l.Length)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Truck>()
                .Property(t => t.LoadCapacityWeight)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Truck>()
                .Property(t => t.LoadCapacityVolume)
                .HasColumnType("decimal(18,2)");
            builder.Entity<Truck>()
    .Property(t => t.Height)
    .HasColumnType("decimal(18,2)"); // Specify precision and scale

            builder.Entity<Truck>()
                .Property(t => t.Length)
                .HasColumnType("decimal(18,2)"); // Specify precision and scale

            builder.Entity<Truck>()
                .Property(t => t.Width)
                .HasColumnType("decimal(18,2)");

            // Configure spatial properties for Truck
            builder.Entity<TruckLocation>()
            .Property(t => t.CurrentLocation)
            .HasColumnType("geography");
            builder.Entity<TruckLocation>()
                .HasOne(tl => tl.Truck)
                .WithMany()
                .HasForeignKey(tl => tl.TruckId)
                .OnDelete(DeleteBehavior.Cascade);

            // Keep the original latitude/longitude columns for backward compatibility
            builder.Entity<TruckLocation>()
                .Property(t => t.CurrentLatitude)
                .HasColumnType("decimal(9,6)");

            builder.Entity<TruckLocation>()
                .Property(t => t.CurrentLongitude)
                .HasColumnType("decimal(9,6)");

            // Existing relationships configuration
            builder.Entity<LoadStop>()
                .HasOne(ls => ls.Load)
                .WithMany(l => l.Stops)
                .HasForeignKey(ls => ls.LoadId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Truck>()
                .HasOne(t => t.TruckType)
                .WithMany()
                .HasForeignKey(t => t.TruckTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Truck>()
    .HasOne(t => t.Owner)
    .WithMany(u => u.Trucks) // Allow multiple trucks per user
    .HasForeignKey(t => t.OwnerId)
    .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Load>()
                .HasOne(l => l.Shipper)
                .WithMany()
                .HasForeignKey(l => l.ShipperId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Load)
                .WithMany()
                .HasForeignKey(b => b.LoadId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Truck)
                .WithMany()
                .HasForeignKey(b => b.TruckId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>()
                .Property(u => u.UserType)
                .HasConversion<int>();

            builder.Entity<User>().ToTable("AspNetUsers");
        }

    }
}