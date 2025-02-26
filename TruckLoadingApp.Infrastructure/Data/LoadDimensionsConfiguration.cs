using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class LoadDimensionsConfiguration : IEntityTypeConfiguration<LoadDimensions>
    {
        public void Configure(EntityTypeBuilder<LoadDimensions> builder)
        {
            builder.HasKey(ld => ld.LoadId);

            builder.Property(ld => ld.Height)
                .HasColumnType("decimal(18, 2)");

            builder.Property(ld => ld.Width)
                .HasColumnType("decimal(18, 2)");

            builder.Property(ld => ld.Length)
                .HasColumnType("decimal(18, 2)");

            builder.Property(ld => ld.Volume)
                .HasColumnType("decimal(18, 2)");

            builder.Property(ld => ld.Weight)
                .HasColumnType("decimal(18, 2)")
                .IsRequired(); //Weight is required

            // Relationship
            builder.HasOne(ld => ld.Load)
                .WithOne(l => l.LoadDimensions)
                .HasForeignKey<LoadDimensions>(ld => ld.LoadId);
        }
    }
}