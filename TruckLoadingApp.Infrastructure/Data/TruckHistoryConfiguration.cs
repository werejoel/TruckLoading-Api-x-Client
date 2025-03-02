using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class TruckHistoryConfiguration : IEntityTypeConfiguration<TruckHistory>
    {
        public void Configure(EntityTypeBuilder<TruckHistory> builder)
        {
            builder.HasKey(th => th.Id);

            builder.Property(th => th.Action)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(th => th.Details)
                .HasMaxLength(500);

            // Configure the relationship with Truck
            // This is the key part that fixes the type mismatch issue
            builder.HasOne(th => th.Truck)
                .WithMany()
                .HasForeignKey(th => th.TruckId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Convert TruckId to int to match Truck.Id
            builder.Property(th => th.TruckId)
                .HasColumnType("int");
        }
    }
} 