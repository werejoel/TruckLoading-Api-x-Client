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

            builder.Property(l => l.HandlingInstructions)
                .HasMaxLength(500);

            builder.Property(l => l.StackingInstructions)
                .HasMaxLength(200);

            builder.Property(l => l.UNNumber)
                .HasMaxLength(100);

            builder.Property(l => l.CustomsDeclarationNumber)
                .HasMaxLength(200);

            // Relationships
            builder.HasOne(l => l.Shipper)
                   .WithMany()
                   .HasForeignKey(l => l.ShipperId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.LoadType)
                   .WithMany()
                   .HasForeignKey(l => l.LoadTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.RequiredTruckType)
                   .WithMany()
                   .HasForeignKey(l => l.RequiredTruckTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.TemperatureRequirement)
                   .WithOne(t => t.Load)
                   .HasForeignKey<LoadTemperatureRequirement>(t => t.LoadId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(l => l.LoadTags)
                   .WithOne(lt => lt.Load)
                   .HasForeignKey(lt => lt.LoadId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}