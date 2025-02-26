using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class ContactDetailsConfiguration : IEntityTypeConfiguration<ContactDetails>
    {
        public void Configure(EntityTypeBuilder<ContactDetails> builder)
        {
            builder.HasKey(cd => cd.UserId);

            builder.Property(cd => cd.Phone)
                .HasMaxLength(100);

            builder.Property(cd => cd.Address)
                .HasMaxLength(100);

            // Relationship
            builder.HasOne(cd => cd.User)
                .WithOne(u => u.ContactDetails)
                .HasForeignKey<ContactDetails>(cd => cd.UserId);
        }
    }
}