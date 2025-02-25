using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Domain.Enums;

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


            builder.Entity<Truck>()
                .HasOne(t => t.AssignedDriver)
                .WithOne(d => d.Truck)
                .HasForeignKey<Driver>(d => d.TruckId)
                .IsRequired(false)  // Make this explicit
                .OnDelete(DeleteBehavior.SetNull);

            // Spatial Data TruckLocationHistory
            builder.Entity<TruckLocationHistory>()
               .Property(t => t.Location)
               .HasColumnType("geography");

            builder.Entity<Load>()
    .HasOne(l => l.Shipper)
    .WithMany()
    .HasForeignKey(l => l.ShipperId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Waypoint>()
                .Property(w => w.Location)
                .HasColumnType("geography");

            builder.Entity<User>()
            .HasOne(u => u.ContactDetails)
            .WithOne(cd => cd.User)
            .HasForeignKey<ContactDetails>(cd => cd.UserId);

            builder.Entity<Waypoint>()
                .HasOne(w => w.Route)
                .WithMany(r => r.Waypoints)
                .HasForeignKey(w => w.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Truck>()
                .HasOne(t => t.TruckType)
                .WithMany()
                .HasForeignKey(t => t.TruckTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LoadStop>()
            .HasOne(ls => ls.Booking)
            .WithMany()
            .HasForeignKey(ls => ls.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PriceFactors>()
                .HasOne(pf => pf.LoadType)
                .WithMany()
                .HasForeignKey(pf => pf.LoadTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PriceFactors>()
                .HasOne(pf => pf.Region)
                .WithMany()
                .HasForeignKey(pf => pf.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TruckLocationHistory>()
                .Property(t => t.Latitude)
                .HasColumnType("decimal(18,15)");

            builder.Entity<TruckLocationHistory>()
                .Property(t => t.Longitude)
                .HasColumnType("decimal(18,15)");

            // Existing relationships configuration
            builder.Entity<Truck>()
                .HasOne(t => t.TruckType)
                .WithMany()
                .HasForeignKey(t => t.TruckTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Truck>()
                .HasOne(t => t.Owner)
                .WithMany(u => u.Trucks)
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

            builder.Entity<Driver>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Documents>()
                .Property(d => d.EntityType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<TruckType>()
               .HasIndex(tt => tt.Name)
               .IsUnique();

            // Enum Conversions
            builder.Entity<User>()
                .Property(u => u.UserType)
                .HasConversion<int>();

            builder.Entity<User>()
               .Property(u => u.TruckOwnerType)
               .HasConversion<int>();

            builder.Entity<User>().ToTable("AspNetUsers");

            builder.Entity<Load>()
               .Property(l => l.GoodsType)
               .HasConversion<int>();

            builder.Entity<Truck>()
               .Property(t => t.OperationalStatus)
               .HasConversion<int>();

            builder.Entity<Booking>()
               .Property(b => b.Status)
               .HasConversion<int>();

            builder.Entity<Documents>()
                .Property(d => d.DocumentType)
                .HasConversion<int>();

            builder.Entity<PaymentDetails>()
                .Property(p => p.PaymentStatus)
                .HasConversion<int>();

            builder.Entity<PaymentDetails>()
                .Property(p => p.PaymentMethod)
                .HasConversion<int>();

            builder.Entity<Load>()
               .HasOne(l => l.LoadDimensions)
               .WithOne(ld => ld.Load)
               .HasForeignKey<LoadDimensions>(ld => ld.LoadId);

            builder.Entity<Documents>()
                .Property(d => d.EntityType)
                .HasConversion<string>();

            builder.Entity<TruckLocationHistory>()
                .HasOne(tl => tl.Truck)
                .WithMany()
                .HasForeignKey(tl => tl.TruckId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Load>()
                .Property(l => l.Price)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Ratings>()
                .HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ratings>()
                .HasOne(r => r.RatedByUser)
                .WithMany()
                .HasForeignKey(r => r.RatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ratings>()
                .HasOne(r => r.RatedUser)
                .WithMany()
                .HasForeignKey(r => r.RatedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LoadDimensions>()
                .Property(ld => ld.Height)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<LoadDimensions>()
                .Property(ld => ld.Width)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<LoadDimensions>()
                .Property(ld => ld.Length)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<LoadDimensions>()
                .Property(ld => ld.Volume)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<LoadDimensions>()
                .Property(ld => ld.Weight)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<PriceFactors>()
                .Property(p => p.DistanceRate)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<PriceFactors>()
                .Property(p => p.WeightRate)
                .HasColumnType("decimal(18, 2)");

            // Explicitly configure decimal column types
            builder.Entity<Load>()
                .Property(l => l.Height)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Load>()
                .Property(l => l.Length)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Load>()
                .Property(l => l.Weight)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Load>()
                .Property(l => l.Width)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Truck>()
                .Property(t => t.Height)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Truck>()
                .Property(t => t.Length)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Truck>()
                .Property(t => t.Width)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<Driver>()
                .Property(d => d.SafetyRating)
                .HasColumnType("decimal(3, 2)");

            builder.Entity<PaymentDetails>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18, 2)");

            builder.Entity<TruckLocation>()
                .Property(tl => tl.CurrentLatitude)
                .HasColumnType("decimal(9, 6)");

            builder.Entity<TruckLocation>()
                .Property(tl => tl.CurrentLongitude)
                .HasColumnType("decimal(9, 6)");

            builder.Entity<TruckLocationHistory>()
                .Property(tl => tl.Speed)
                .HasColumnType("decimal(10, 2)");

            builder.Entity<Waypoint>()
                .Property(w => w.Latitude)
                .HasColumnType("decimal(9, 6)");

            builder.Entity<Waypoint>()
                .Property(w => w.Longitude)
                .HasColumnType("decimal(9, 6)");
            builder.Entity<UserLocation>()
           .Property(ul => ul.Latitude)
           .HasColumnType("decimal(9, 6)");

            builder.Entity<UserLocation>()
                .Property(ul => ul.Longitude)
                .HasColumnType("decimal(9, 6)");
        }
    }
}
