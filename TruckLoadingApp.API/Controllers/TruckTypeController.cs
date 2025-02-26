using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.API.Controllers
{
    /// <summary>
    /// Controller for managing truck types.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TruckTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TruckTypeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TruckTypeController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public TruckTypeController(ApplicationDbContext context, ILogger<TruckTypeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new truck type.
        /// </summary>
        /// <param name="truckType">The truck type to create.</param>
        /// <returns>An IActionResult indicating the result of the truck type creation.</returns>
        /// <response code="201">Truck type created successfully.</response>
        /// <response code="400">Invalid model state.</response>
        [HttpPost("create")]
        public async Task<IActionResult> CreateTruckType([FromBody] TruckType truckType)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                truckType.CreatedDate = DateTime.UtcNow;
                _context.TruckTypes.Add(truckType);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Truck type created successfully with ID: {truckType.Id}");
                return CreatedAtAction(nameof(GetTruckTypes), new { id = truckType.Id }, truckType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating truck type.");
                return StatusCode(500, new { Message = "Failed to create truck type. Please try again later." });
            }
        }

        /// <summary>
        /// Retrieves a truck type by its ID.
        /// </summary>
        /// <param name="id">The ID of the truck type to retrieve.</param>
        /// <returns>An IActionResult containing the truck type.</returns>
        /// <response code="200">Returns the truck type.</response>
        /// <response code="404">Truck type not found.</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTruckTypes()
        {
            try
            {
                var truckTypes = await _context.TruckTypes
                    .Where(t => t.IsActive)  // Only return active truck types
                    .Select(t => new { t.Id, t.Name })
                    .ToListAsync();

                if (!truckTypes.Any())
                {
                    _logger.LogInformation("No active truck types found.");
                    return NoContent();
                }

                return Ok(truckTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting truck types.");
                return StatusCode(500, new { Message = "Failed to retrieve truck types. Please try again later." });
            }
        }

        /// <summary>
        /// Updates an existing truck type.
        /// </summary>
        /// <param name="id">The ID of the truck type to update.</param>
        /// <param name="truckType">The updated truck type.</param>
        /// <returns>An IActionResult indicating the result of the truck type update.</returns>
        /// <response code="204">Truck type updated successfully.</response>
        /// <response code="400">Invalid model state or ID mismatch.</response>
        /// <response code="404">Truck type not found.</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTruckType(int id, [FromBody] TruckType truckType)
        {
            if (id != truckType.Id)
            {
                _logger.LogWarning($"ID mismatch: Request ID {id} does not match truck type ID {truckType.Id}.");
                return BadRequest(new { Message = "ID mismatch between request and entity." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                _context.Entry(truckType).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Truck type with ID {id} updated successfully.");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TruckTypeExists(id))
                {
                    _logger.LogWarning($"Truck type with ID {id} not found during concurrency check.");
                    return NotFound(new { Message = "Truck type not found." });
                }
                else
                {
                    _logger.LogError(ex, $"Concurrency error updating truck type with ID {id}.");
                    return StatusCode(500, new { Message = "Failed to update truck type due to a concurrency issue. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating truck type with ID {id}.");
                return StatusCode(500, new { Message = "Failed to update truck type. Please try again later." });
            }
        }

        /// <summary>
        /// Deletes a truck type.
        /// </summary>
        /// <param name="id">The ID of the truck type to delete.</param>
        /// <returns>An IActionResult indicating the result of the truck type deletion.</returns>
        /// <response code="204">Truck type deleted successfully.</response>
        /// <response code="404">Truck type not found.</response>

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTruckType(int id)
        {
            try
            {
                var truckType = await _context.TruckTypes.FindAsync(id);
                if (truckType == null)
                {
                    _logger.LogWarning($"Truck type with ID {id} not found.");
                    return NotFound(new { Message = "Truck type not found." });
                }

                truckType.IsActive = false;  // ✅ Soft delete
                truckType.UpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Truck type with ID {id} soft deleted successfully.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting truck type with ID {id}.");
                return StatusCode(500, new { Message = "Failed to delete truck type. Please try again later." });
            }
        }


        private bool TruckTypeExists(int id)
        {
            return _context.TruckTypes.Any(e => e.Id == id);
        }
    }
}