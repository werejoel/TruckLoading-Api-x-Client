using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class PaymentDetailsConfiguration : IEntityTypeConfiguration<PaymentDetails>
    {
        public void Configure(EntityTypeBuilder<PaymentDetails> builder)
        {
            builder.HasKey(pd => pd.Id);

            builder.Property(pd => pd.Amount)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            builder.Property(pd => pd.TransactionId)
                .HasMaxLength(100);
        }
    }
}