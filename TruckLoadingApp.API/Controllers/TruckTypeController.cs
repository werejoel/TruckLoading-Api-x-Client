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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TruckTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TruckTypeController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public TruckTypeController(ApplicationDbContext context)
        {
            _context = context;
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
            if (ModelState.IsValid)
            {
                truckType.CreatedDate = DateTime.UtcNow;
                _context.TruckTypes.Add(truckType);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTruckType", new { id = truckType.Id }, truckType);
            }

            return BadRequest(ModelState);
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
            var truckTypes = await _context.TruckTypes
                .Where(t => t.IsActive)  // Only return active truck types
                .Select(t => new { t.Id, t.Name })
                .ToListAsync();

            if (!truckTypes.Any())
            {
                return NoContent();
            }

            return Ok(truckTypes);
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
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                _context.Entry(truckType).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
                {
                    if (!TruckTypeExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }

            return BadRequest(ModelState);
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
            var truckType = await _context.TruckTypes.FindAsync(id);
            if (truckType == null)
            {
                return NotFound();
            }

            truckType.IsActive = false;  // ✅ Soft delete
            truckType.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool TruckTypeExists(int id)
        {
            return _context.TruckTypes.Any(e => e.Id == id);
        }
    }
}