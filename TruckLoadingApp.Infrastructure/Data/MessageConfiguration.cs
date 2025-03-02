using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Infrastructure.Data
{
    public class UserMessageConfiguration : IEntityTypeConfiguration<UserMessage>
    {
        public void Configure(EntityTypeBuilder<UserMessage> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(1000);

            // Create indexes for faster message retrieval
            builder.HasIndex(m => new { m.SenderId, m.SentTime });
            builder.HasIndex(m => new { m.ReceiverId, m.SentTime });
            builder.HasIndex(m => m.Status);
        }
    }

    public class GroupMessageConfiguration : IEntityTypeConfiguration<GroupMessage>
    {
        public void Configure(EntityTypeBuilder<GroupMessage> builder)
        {
            builder.HasKey(m => m.Id);

            builder.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Team)
                .WithMany()
                .HasForeignKey(m => m.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(1000);

            // Create indexes for faster message retrieval
            builder.HasIndex(m => new { m.TeamId, m.SentTime });
            builder.HasIndex(m => new { m.SenderId, m.SentTime });
        }
    }

    public class GroupMessageReceiptConfiguration : IEntityTypeConfiguration<GroupMessageReceipt>
    {
        public void Configure(EntityTypeBuilder<GroupMessageReceipt> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.GroupMessage)
                .WithMany(m => m.MessageReceipts)
                .HasForeignKey(r => r.GroupMessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create a unique index to prevent duplicate receipts
            builder.HasIndex(r => new { r.GroupMessageId, r.UserId }).IsUnique();
        }
    }
}
