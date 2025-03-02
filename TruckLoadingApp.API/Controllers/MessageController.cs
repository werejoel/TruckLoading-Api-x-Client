using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers
{
   // [EnableRateLimiting("default")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("direct")]
        [Authorize(Policy = "SendMessages")]
        public async Task<ActionResult<UserMessage>> SendDirectMessage([FromBody] SendDirectMessageRequest request)
        {
            try
            {
                var message = await _messageService.SendDirectMessageAsync(
                    request.SenderId,
                    request.ReceiverId,
                    request.Content,
                    request.RelatedEntityType,
                    request.RelatedEntityId);

                return CreatedAtAction(
                    nameof(GetDirectMessage),
                    new { messageId = message.Id },
                    message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("direct/{messageId}")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<UserMessage>> GetDirectMessage(long messageId)
        {
            var message = await _messageService.GetDirectMessageByIdAsync(messageId);
            if (message == null)
                return NotFound();

            return Ok(message);
        }

        [HttpGet("direct/thread")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<IEnumerable<UserMessage>>> GetDirectMessageThread(
            [FromQuery] string userId1,
            [FromQuery] string userId2,
            [FromQuery] int? maxMessages)
        {
            var messages = await _messageService.GetDirectMessageThreadAsync(
                userId1,
                userId2,
                maxMessages);

            return Ok(messages);
        }

        [HttpGet("direct/user/{userId}")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<IEnumerable<UserMessage>>> GetUserDirectMessages(
            string userId,
            [FromQuery] bool sent = true,
            [FromQuery] bool received = true,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? maxMessages = null)
        {
            var messages = await _messageService.GetUserDirectMessagesAsync(
                userId,
                sent,
                received,
                startDate,
                endDate,
                maxMessages);

            return Ok(messages);
        }

        [HttpPost("direct/{messageId}/read")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<IActionResult> MarkDirectMessageAsRead(long messageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "User ID could not be determined." });
            }
            
            var result = await _messageService.MarkDirectMessageAsReadAsync(messageId, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("direct/{messageId}")]
        [Authorize(Policy = "ManageMessages")]
        public async Task<IActionResult> DeleteDirectMessage(long messageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "User ID could not be determined." });
            }
            
            var result = await _messageService.DeleteDirectMessageAsync(messageId, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("group")]
        [Authorize(Policy = "SendMessages")]
        public async Task<ActionResult<GroupMessage>> SendGroupMessage([FromBody] SendGroupMessageRequest request)
        {
            try
            {
                var message = await _messageService.SendGroupMessageAsync(
                    request.SenderId,
                    request.TeamId,
                    request.Content,
                    request.RelatedEntityType,
                    request.RelatedEntityId);

                return CreatedAtAction(
                    nameof(GetGroupMessage),
                    new { messageId = message.Id },
                    message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("group/{messageId}")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<GroupMessage>> GetGroupMessage(long messageId)
        {
            var message = await _messageService.GetGroupMessageByIdAsync(messageId);
            if (message == null)
                return NotFound();

            return Ok(message);
        }

        [HttpGet("group/team/{teamId}")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<IEnumerable<GroupMessage>>> GetTeamMessages(
            int teamId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? maxMessages = null)
        {
            var messages = await _messageService.GetTeamMessagesAsync(
                teamId,
                startDate,
                endDate,
                maxMessages);

            return Ok(messages);
        }

        [HttpGet("group/user/{userId}")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<IEnumerable<GroupMessage>>> GetUserGroupMessages(
            string userId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? maxMessages = null)
        {
            var messages = await _messageService.GetUserGroupMessagesAsync(
                userId,
                startDate,
                endDate,
                maxMessages);

            return Ok(messages);
        }

        [HttpPost("group/{messageId}/read")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<IActionResult> MarkGroupMessageAsRead(long messageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "User ID could not be determined." });
            }
            
            var result = await _messageService.MarkGroupMessageAsReadAsync(messageId, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("group/{messageId}")]
        [Authorize(Policy = "ManageMessages")]
        public async Task<IActionResult> DeleteGroupMessage(long messageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "User ID could not be determined." });
            }
            
            var result = await _messageService.DeleteGroupMessageAsync(messageId, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("statistics/user/{userId}")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<MessageStatistics>> GetUserMessageStatistics(
            string userId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var statistics = await _messageService.GetUserMessageStatisticsAsync(
                userId,
                startDate,
                endDate);

            return Ok(statistics);
        }

        [HttpGet("statistics/team/{teamId}")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<MessageStatistics>> GetTeamMessageStatistics(
            int teamId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var statistics = await _messageService.GetTeamMessageStatisticsAsync(
                teamId,
                startDate,
                endDate);

            return Ok(statistics);
        }

        [HttpGet("unread/direct/{userId}")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<int>> GetUnreadDirectMessageCount(string userId)
        {
            var count = await _messageService.GetUnreadDirectMessageCountAsync(userId);
            return Ok(count);
        }

        [HttpGet("unread/group/{userId}")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<int>> GetUnreadGroupMessageCount(string userId)
        {
            var count = await _messageService.GetUnreadGroupMessageCountAsync(userId);
            return Ok(count);
        }

        [HttpGet("unread/team/{userId}")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<Dictionary<int, int>>> GetUnreadMessageCountByTeam(string userId)
        {
            var counts = await _messageService.GetUnreadMessageCountByTeamAsync(userId);
            return Ok(counts);
        }

        [HttpGet("search/direct")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<IEnumerable<UserMessage>>> SearchDirectMessages(
            [FromQuery] string searchTerm,
            [FromQuery] string? userId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? maxResults = null)
        {
            var messages = await _messageService.SearchDirectMessagesAsync(
                searchTerm,
                userId,
                startDate,
                endDate,
                maxResults);

            return Ok(messages);
        }

        [HttpGet("search/group")]
        [Authorize(Policy = "ViewMessages")]
        public async Task<ActionResult<IEnumerable<GroupMessage>>> SearchGroupMessages(
            [FromQuery] string searchTerm,
            [FromQuery] int? teamId = null,
            [FromQuery] string? userId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? maxResults = null)
        {
            var messages = await _messageService.SearchGroupMessagesAsync(
                searchTerm,
                teamId,
                userId,
                startDate,
                endDate,
                maxResults);

            return Ok(messages);
        }
    }

    public class SendDirectMessageRequest
    {
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? RelatedEntityType { get; set; }
        public string? RelatedEntityId { get; set; }
    }

    public class SendGroupMessageRequest
    {
        public string SenderId { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? RelatedEntityType { get; set; }
        public string? RelatedEntityId { get; set; }
    }
}
