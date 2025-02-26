using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class LoadConfiguration : IEntityTypeConfiguration<Load>
    {
        public void Configure(EntityTypeBuilder<Load> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Weight)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            builder.Property(l => l.Height)
                .HasColumnType("decimal(18, 2)");

            builder.Property(l => l.Width)
                .HasColumnType("decimal(18, 2)");

            builder.Property(l => l.Length)
                .HasColumnType("decimal(18, 2)");

            builder.Property(l => l.Description)
                .HasMaxLength(500);

            builder.Property(l => l.Price)
                 .HasColumnType("decimal(18, 2)");

            // Relationships
            builder.HasOne(l => l.Shipper)
                   .WithMany()
                   .HasForeignKey(l => l.ShipperId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}