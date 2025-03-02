using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class PayrollEntryConfiguration : IEntityTypeConfiguration<PayrollEntry>
    {
        public void Configure(EntityTypeBuilder<PayrollEntry> builder)
        {
            builder.HasKey(pe => pe.Id);

            // Configure decimal properties with appropriate precision and scale
            builder.Property(pe => pe.RegularHours)
                .HasColumnType("decimal(8, 2)");

            builder.Property(pe => pe.OvertimeHours)
                .HasColumnType("decimal(8, 2)");

            builder.Property(pe => pe.RegularRate)
                .HasColumnType("decimal(10, 2)");

            builder.Property(pe => pe.OvertimeRate)
                .HasColumnType("decimal(10, 2)");

            builder.Property(pe => pe.RegularPay)
                .HasColumnType("decimal(12, 2)");

            builder.Property(pe => pe.OvertimePay)
                .HasColumnType("decimal(12, 2)");

            builder.Property(pe => pe.PerformanceBonus)
                .HasColumnType("decimal(12, 2)");

            builder.Property(pe => pe.SafetyBonus)
                .HasColumnType("decimal(12, 2)");

            builder.Property(pe => pe.OtherBonuses)
                .HasColumnType("decimal(12, 2)");

            builder.Property(pe => pe.Deductions)
                .HasColumnType("decimal(12, 2)");

            builder.Property(pe => pe.TotalCompensation)
                .HasColumnType("decimal(12, 2)");

            builder.Property(pe => pe.TotalPay)
                .HasColumnType("decimal(12, 2)");

            // Configure relationships
            builder.HasOne(pe => pe.Driver)
                .WithMany()
                .HasForeignKey(pe => pe.DriverId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PayrollEntry_Drivers_DriverId");
        }
    }
}
