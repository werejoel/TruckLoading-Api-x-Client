using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
    {
        public void Configure(EntityTypeBuilder<UserActivity> builder)
        {
            builder.HasKey(ua => ua.Id);

            builder.HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(ua => ua.ActivityType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ua => ua.Description)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(ua => ua.EntityType)
                .HasMaxLength(50);

            builder.Property(ua => ua.Status)
                .HasMaxLength(50);

            // Create an index on Timestamp for faster querying of logs
            builder.HasIndex(ua => ua.Timestamp);

            // Create a composite index on UserId and ActivityType
            builder.HasIndex(ua => new { ua.UserId, ua.ActivityType });
        }
    }
}
