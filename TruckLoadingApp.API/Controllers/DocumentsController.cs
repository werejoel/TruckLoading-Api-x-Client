using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.API.Configuration;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(ApplicationDbContext context, ILogger<DocumentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{entityType}/{entityId}")]
        public async Task<ActionResult<IEnumerable<Documents>>> GetDocuments(string entityType, long entityId)
        {
            try
            {
                var documents = await _context.Documents
                    .Where(d => d.EntityType == entityType && d.EntityId == entityId)
                    .ToListAsync();

                if (documents == null || documents.Count == 0)
                {
                    _logger.LogWarning($"No documents found for EntityType: {entityType}, EntityId: {entityId}");
                    return NotFound(new { Message = "No documents found." });
                }

                return documents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting documents for EntityType: {entityType}, EntityId: {entityId}");
                return StatusCode(500, new { Message = "Failed to retrieve documents. Please try again later." });
            }
        }
    }
}
