using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class TemperatureReadingConfiguration : IEntityTypeConfiguration<TemperatureReading>
    {
        public void Configure(EntityTypeBuilder<TemperatureReading> builder)
        {
            builder.HasKey(tr => tr.Id);

            builder.Property(tr => tr.Temperature)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(tr => tr.Timestamp)
                .IsRequired();

            builder.Property(tr => tr.DeviceId)
                .HasMaxLength(50);

            builder.HasOne(tr => tr.Load)
                .WithMany()
                .HasForeignKey(tr => tr.LoadId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("TemperatureReadings");
        }
    }
} 