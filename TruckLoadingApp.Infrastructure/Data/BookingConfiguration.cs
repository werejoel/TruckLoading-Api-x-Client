using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.AgreedPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.PriceCalculationMethod)
                .HasMaxLength(50);

            builder.Property(b => b.Currency)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(b => b.CancellationReason)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(b => b.Load)
                .WithMany()
                .HasForeignKey(b => b.LoadId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Truck)
                .WithMany()
                .HasForeignKey(b => b.TruckId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
