using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TruckLoadingApp.Domain.Models
{
    public class UserMessage
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public string ReceiverId { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime SentTime { get; set; } = DateTime.UtcNow;

        public DateTime? ReadTime { get; set; }

        public MessageStatus Status { get; set; } = MessageStatus.Sent;

        public MessageType Type { get; set; } = MessageType.Direct;

        public string? RelatedEntityType { get; set; }

        public string? RelatedEntityId { get; set; }

        // Navigation properties
        [ForeignKey("SenderId")]
        public User Sender { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; } = null!;
    }

    public class GroupMessage
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;

        [Required]
        public int TeamId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime SentTime { get; set; } = DateTime.UtcNow;

        public MessageType Type { get; set; } = MessageType.Group;

        public string? RelatedEntityType { get; set; }

        public string? RelatedEntityId { get; set; }

        // Navigation properties
        [ForeignKey("SenderId")]
        public User Sender { get; set; } = null!;

        [ForeignKey("TeamId")]
        public Team Team { get; set; } = null!;

        public ICollection<GroupMessageReceipt> MessageReceipts { get; set; } = new List<GroupMessageReceipt>();
    }

    public class GroupMessageReceipt
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public long GroupMessageId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public DateTime? ReadTime { get; set; }

        // Navigation properties
        [ForeignKey("GroupMessageId")]
        public GroupMessage GroupMessage { get; set; } = null!;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }

    public enum MessageStatus
    {
        Sent,
        Delivered,
        Read,
        Failed
    }

    public enum MessageType
    {
        Direct,
        Group,
        System,
        Alert
    }
}
