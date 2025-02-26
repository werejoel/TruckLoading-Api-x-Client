using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.CompanyName)
                .HasMaxLength(256);

            builder.Property(u => u.CompanyAddress)
                .HasMaxLength(500);

            builder.Property(u => u.CompanyRegistrationNumber)
                .HasMaxLength(50);

            builder.Property(u => u.CompanyContact)
                .HasMaxLength(100);

            builder.ToTable("AspNetUsers"); // Explicitly map to AspNetUsers table

            // Enum Conversions
            builder.Property(u => u.UserType)
                .HasConversion<int>();

            builder.Property(u => u.TruckOwnerType)
                .HasConversion<int>();
        }
    }
}