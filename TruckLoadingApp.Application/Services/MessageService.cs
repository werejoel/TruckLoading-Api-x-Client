using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserActivityService _userActivityService;
        private readonly ITeamService _teamService;

        public MessageService(
            ApplicationDbContext context,
            IUserActivityService userActivityService,
            ITeamService teamService)
        {
            _context = context;
            _userActivityService = userActivityService;
            _teamService = teamService;
        }

        public async Task<UserMessage> SendDirectMessageAsync(
            string senderId,
            string receiverId,
            string content,
            string? relatedEntityType = null,
            string? relatedEntityId = null)
        {
            var sender = await _context.Users.FindAsync(senderId);
            if (sender == null)
                throw new KeyNotFoundException($"Sender with ID {senderId} not found.");

            var receiver = await _context.Users.FindAsync(receiverId);
            if (receiver == null)
                throw new KeyNotFoundException($"Receiver with ID {receiverId} not found.");

            var message = new UserMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId,
                Type = MessageType.Direct
            };

            _context.UserMessages.Add(message);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                senderId,
                ActivityTypes.SendMessage,
                $"Sent direct message to {receiverId}",
                "Message",
                message.Id.ToString());

            return message;
        }

        public async Task<UserMessage?> GetDirectMessageByIdAsync(long messageId)
        {
            return await _context.UserMessages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task<IEnumerable<UserMessage>> GetDirectMessageThreadAsync(
            string userId1,
            string userId2,
            int? maxMessages = null)
        {
            var query = _context.UserMessages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                           (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderByDescending(m => m.SentTime)
                .AsQueryable();

            if (maxMessages.HasValue)
                query = query.Take(maxMessages.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<UserMessage>> GetUserDirectMessagesAsync(
            string userId,
            bool sent = true,
            bool received = true,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxMessages = null)
        {
            var query = _context.UserMessages.AsQueryable();

            if (sent && received)
                query = query.Where(m => m.SenderId == userId || m.ReceiverId == userId);
            else if (sent)
                query = query.Where(m => m.SenderId == userId);
            else if (received)
                query = query.Where(m => m.ReceiverId == userId);

            if (startDate.HasValue)
                query = query.Where(m => m.SentTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.SentTime <= endDate.Value);

            query = query.OrderByDescending(m => m.SentTime).AsQueryable();

            if (maxMessages.HasValue)
                query = query.Take(maxMessages.Value);

            return await query.ToListAsync();
        }

        public async Task<bool> MarkDirectMessageAsReadAsync(long messageId, string userId)
        {
            var message = await _context.UserMessages.FindAsync(messageId);
            if (message == null || message.ReceiverId != userId)
                return false;

            if (message.ReadTime.HasValue)
                return true;

            message.ReadTime = DateTime.UtcNow;
            message.Status = MessageStatus.Read;
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                userId,
                ActivityTypes.ReadMessage,
                "Read direct message",
                "Message",
                messageId.ToString());

            return true;
        }

        public async Task<bool> DeleteDirectMessageAsync(long messageId, string userId)
        {
            var message = await _context.UserMessages.FindAsync(messageId);
            if (message == null || (message.SenderId != userId && message.ReceiverId != userId))
                return false;

            _context.UserMessages.Remove(message);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                userId,
                ActivityTypes.DeleteMessage,
                "Deleted direct message",
                "Message",
                messageId.ToString());

            return true;
        }

        public async Task<GroupMessage> SendGroupMessageAsync(
            string senderId,
            int teamId,
            string content,
            string? relatedEntityType = null,
            string? relatedEntityId = null)
        {
            var sender = await _context.Users.FindAsync(senderId);
            if (sender == null)
                throw new KeyNotFoundException($"Sender with ID {senderId} not found.");

            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found.");

            if (!await _teamService.IsUserInTeamAsync(teamId, senderId))
                throw new InvalidOperationException($"User {senderId} is not a member of team {teamId}.");

            var message = new GroupMessage
            {
                SenderId = senderId,
                TeamId = teamId,
                Content = content,
                RelatedEntityType = relatedEntityType,
                RelatedEntityId = relatedEntityId,
                Type = MessageType.Group
            };

            _context.GroupMessages.Add(message);

            // Create message receipts for all team members
            var teamMembers = await _teamService.GetTeamMembersAsync(teamId);
            foreach (var member in teamMembers)
            {
                if (member.UserId != senderId) // Don't create receipt for sender
                {
                    var receipt = new GroupMessageReceipt
                    {
                        GroupMessage = message,
                        UserId = member.UserId
                    };
                    _context.GroupMessageReceipts.Add(receipt);
                }
            }

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                senderId,
                ActivityTypes.SendMessage,
                $"Sent group message to team {teamId}",
                "Message",
                message.Id.ToString());

            return message;
        }

        public async Task<GroupMessage?> GetGroupMessageByIdAsync(long messageId)
        {
            return await _context.GroupMessages
                .Include(m => m.Sender)
                .Include(m => m.Team)
                .Include(m => m.MessageReceipts)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task<IEnumerable<GroupMessage>> GetTeamMessagesAsync(
            int teamId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxMessages = null)
        {
            var query = _context.GroupMessages
                .Where(m => m.TeamId == teamId)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(m => m.SentTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.SentTime <= endDate.Value);

            query = query.OrderByDescending(m => m.SentTime);

            if (maxMessages.HasValue)
                query = query.Take(maxMessages.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<GroupMessage>> GetUserGroupMessagesAsync(
            string userId,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxMessages = null)
        {
            var userTeams = await _teamService.GetTeamsByMemberAsync(userId);
            var teamIds = userTeams.Select(t => t.Id).ToList();

            var query = _context.GroupMessages
                .Where(m => teamIds.Contains(m.TeamId))
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(m => m.SentTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.SentTime <= endDate.Value);

            query = query.OrderByDescending(m => m.SentTime);

            if (maxMessages.HasValue)
                query = query.Take(maxMessages.Value);

            return await query.ToListAsync();
        }

        public async Task<bool> MarkGroupMessageAsReadAsync(long messageId, string userId)
        {
            var receipt = await _context.GroupMessageReceipts
                .FirstOrDefaultAsync(r => r.GroupMessageId == messageId && r.UserId == userId);

            if (receipt == null)
                return false;

            if (receipt.ReadTime.HasValue)
                return true;

            receipt.ReadTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                userId,
                ActivityTypes.ReadMessage,
                "Read group message",
                "Message",
                messageId.ToString());

            return true;
        }

        public async Task<bool> DeleteGroupMessageAsync(long messageId, string userId)
        {
            var message = await _context.GroupMessages
                .Include(m => m.Team)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
                return false;

            // Only allow deletion by sender or team leader
            if (message.SenderId != userId && message.Team.TeamLeaderId != userId)
                return false;

            _context.GroupMessageReceipts.RemoveRange(message.MessageReceipts);
            _context.GroupMessages.Remove(message);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                userId,
                ActivityTypes.DeleteMessage,
                "Deleted group message",
                "Message",
                messageId.ToString());

            return true;
        }

        public async Task<MessageStatistics> GetUserMessageStatisticsAsync(
            string userId,
            DateTime startDate,
            DateTime endDate)
        {
            var stats = new MessageStatistics();

            // Direct messages
            var directMessages = await _context.UserMessages
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) &&
                           m.SentTime >= startDate &&
                           m.SentTime <= endDate)
                .ToListAsync();

            stats.TotalMessagesSent = directMessages.Count(m => m.SenderId == userId);
            stats.TotalMessagesReceived = directMessages.Count(m => m.ReceiverId == userId);
            stats.UnreadMessages = directMessages.Count(m => m.ReceiverId == userId && !m.ReadTime.HasValue);

            // Group messages
            var userTeams = await _teamService.GetTeamsByMemberAsync(userId);
            var teamIds = userTeams.Select(t => t.Id).ToList();

            var groupMessages = await _context.GroupMessages
                .Where(m => teamIds.Contains(m.TeamId) &&
                           m.SentTime >= startDate &&
                           m.SentTime <= endDate)
                .ToListAsync();

            stats.TotalMessagesSent += groupMessages.Count(m => m.SenderId == userId);
            stats.TotalMessagesReceived += groupMessages.Count(m => m.SenderId != userId);

            // Message types
            stats.MessagesByType = directMessages
                .GroupBy(m => m.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            // Top senders and receivers (for direct messages)
            stats.TopSenders = directMessages
                .Where(m => m.ReceiverId == userId)
                .GroupBy(m => m.SenderId)
                .ToDictionary(g => g.Key, g => g.Count());

            stats.TopReceivers = directMessages
                .Where(m => m.SenderId == userId)
                .GroupBy(m => m.ReceiverId)
                .ToDictionary(g => g.Key, g => g.Count());

            return stats;
        }

        public async Task<MessageStatistics> GetTeamMessageStatisticsAsync(
            int teamId,
            DateTime startDate,
            DateTime endDate)
        {
            var stats = new MessageStatistics();

            var messages = await _context.GroupMessages
                .Where(m => m.TeamId == teamId &&
                           m.SentTime >= startDate &&
                           m.SentTime <= endDate)
                .ToListAsync();

            stats.TotalMessagesSent = messages.Count;

            // Message types
            stats.MessagesByType = messages
                .GroupBy(m => m.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            // Top senders
            stats.TopSenders = messages
                .GroupBy(m => m.SenderId)
                .ToDictionary(g => g.Key, g => g.Count());

            return stats;
        }

        public async Task<int> GetUnreadDirectMessageCountAsync(string userId)
        {
            return await _context.UserMessages
                .CountAsync(m => m.ReceiverId == userId && !m.ReadTime.HasValue);
        }

        public async Task<int> GetUnreadGroupMessageCountAsync(string userId)
        {
            return await _context.GroupMessageReceipts
                .CountAsync(r => r.UserId == userId && !r.ReadTime.HasValue);
        }

        public async Task<Dictionary<int, int>> GetUnreadMessageCountByTeamAsync(string userId)
        {
            var receipts = await _context.GroupMessageReceipts
                .Include(r => r.GroupMessage)
                .Where(r => r.UserId == userId && !r.ReadTime.HasValue)
                .GroupBy(r => r.GroupMessage.TeamId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            return receipts;
        }

        public async Task<IEnumerable<UserMessage>> SearchDirectMessagesAsync(
            string searchTerm,
            string? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxResults = null)
        {
            var query = _context.UserMessages
                .Where(m => m.Content.Contains(searchTerm))
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(m => m.SenderId == userId || m.ReceiverId == userId);

            if (startDate.HasValue)
                query = query.Where(m => m.SentTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.SentTime <= endDate.Value);

            query = query.OrderByDescending(m => m.SentTime);

            if (maxResults.HasValue)
                query = query.Take(maxResults.Value);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<GroupMessage>> SearchGroupMessagesAsync(
            string searchTerm,
            int? teamId = null,
            string? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? maxResults = null)
        {
            var query = _context.GroupMessages
                .Where(m => m.Content.Contains(searchTerm))
                .AsQueryable();

            if (teamId.HasValue)
                query = query.Where(m => m.TeamId == teamId.Value);

            if (!string.IsNullOrEmpty(userId))
            {
                var userTeams = await _teamService.GetTeamsByMemberAsync(userId);
                var teamIds = userTeams.Select(t => t.Id).ToList();
                query = query.Where(m => teamIds.Contains(m.TeamId));
            }

            if (startDate.HasValue)
                query = query.Where(m => m.SentTime >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.SentTime <= endDate.Value);

            query = query.OrderByDescending(m => m.SentTime);

            if (maxResults.HasValue)
                query = query.Take(maxResults.Value);

            return await query.ToListAsync();
        }
    }
}
