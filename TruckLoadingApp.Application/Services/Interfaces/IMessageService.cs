using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface IMessageService
    {
        // Direct Messages
        Task<UserMessage> SendDirectMessageAsync(
            string senderId,
            string receiverId,
            string content,
            string? relatedEntityType = null,
            string? relatedEntityId = null);

        Task<UserMessage?> GetDirectMessageByIdAsync(long messageId);

        Task<IEnumerable<UserMessage>> GetDirectMessageThreadAsync(
            string userId1,
            string userId2,
            int? maxMessages = null);

        Task<IEnumerable<UserMessage>> GetUserDirectMessagesAsync(
            string userId,
            bool sent = true,
            bool received = true,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxMessages = null);

        Task<bool> MarkDirectMessageAsReadAsync(long messageId, string userId);

        Task<bool> DeleteDirectMessageAsync(long messageId, string userId);

        // Group Messages
        Task<GroupMessage> SendGroupMessageAsync(
            string senderId,
            int teamId,
            string content,
            string? relatedEntityType = null,
            string? relatedEntityId = null);

        Task<GroupMessage?> GetGroupMessageByIdAsync(long messageId);

        Task<IEnumerable<GroupMessage>> GetTeamMessagesAsync(
            int teamId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxMessages = null);

        Task<IEnumerable<GroupMessage>> GetUserGroupMessagesAsync(
            string userId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxMessages = null);

        Task<bool> MarkGroupMessageAsReadAsync(long messageId, string userId);

        Task<bool> DeleteGroupMessageAsync(long messageId, string userId);

        // Message Statistics
        Task<MessageStatistics> GetUserMessageStatisticsAsync(
            string userId,
            DateTime startDate,
            DateTime endDate);

        Task<MessageStatistics> GetTeamMessageStatisticsAsync(
            int teamId,
            DateTime startDate,
            DateTime endDate);

        // Unread Messages
        Task<int> GetUnreadDirectMessageCountAsync(string userId);

        Task<int> GetUnreadGroupMessageCountAsync(string userId);

        Task<Dictionary<int, int>> GetUnreadMessageCountByTeamAsync(string userId);

        // Search
        Task<IEnumerable<UserMessage>> SearchDirectMessagesAsync(
            string searchTerm,
            string? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxResults = null);

        Task<IEnumerable<GroupMessage>> SearchGroupMessagesAsync(
            string searchTerm,
            int? teamId = null,
            string? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxResults = null);
    }

    public class MessageStatistics
    {
        public int TotalMessagesSent { get; set; }
        public int TotalMessagesReceived { get; set; }
        public int UnreadMessages { get; set; }
        public Dictionary<MessageType, int> MessagesByType { get; set; } = new();
        public Dictionary<string, int> TopSenders { get; set; } = new();
        public Dictionary<string, int> TopReceivers { get; set; } = new();
    }
}
