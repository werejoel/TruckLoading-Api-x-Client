using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class RecurringScheduleInstanceConfiguration : IEntityTypeConfiguration<RecurringScheduleInstance>
    {
        public void Configure(EntityTypeBuilder<RecurringScheduleInstance> builder)
        {
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.InstanceNumber)
                .IsRequired();
                
            builder.Property(r => r.IsModified)
                .HasDefaultValue(false);
                
            builder.Property(r => r.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");
                
            // Set up relationship with DriverSchedule
            builder.HasOne(r => r.ParentSchedule)
                .WithMany()
                .HasForeignKey(r => r.ParentScheduleId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Set up relationship with Driver
            builder.HasOne(r => r.Driver)
                .WithMany()
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
                
            // Optional relationship with Load
            builder.HasOne(r => r.Load)
                .WithMany()
                .HasForeignKey(r => r.LoadId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
} 