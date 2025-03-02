using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.LoadManagement
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LoadTagController : ControllerBase
    {
        private readonly ILoadTagService _loadTagService;
        private readonly ILogger<LoadTagController> _logger;

        public LoadTagController(
            ILoadTagService loadTagService,
            ILogger<LoadTagController> logger)
        {
            _loadTagService = loadTagService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoadTag>>> GetAllTags()
        {
            try
            {
                var tags = await _loadTagService.GetAllTagsAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tags");
                return StatusCode(500, "An error occurred while retrieving tags");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoadTag>> GetTagById(int id)
        {
            try
            {
                var tag = await _loadTagService.GetTagByIdAsync(id);
                if (tag == null)
                {
                    return NotFound();
                }
                return Ok(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tag with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the tag");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<LoadTag>> CreateTag([FromBody] LoadTag tag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _loadTagService.CreateTagAsync(tag.Name, tag.Description);
                return CreatedAtAction(nameof(GetTagById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error creating tag");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tag");
                return StatusCode(500, "An error occurred while creating the tag");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] LoadTag tag)
        {
            try
            {
                if (id != tag.Id)
                {
                    return BadRequest("Tag ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _loadTagService.UpdateTagAsync(id, tag.Name, tag.Description);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error updating tag with ID {Id}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tag with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the tag");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                var result = await _loadTagService.DeleteTagAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tag with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the tag");
            }
        }

        [HttpGet("load/{loadId}")]
        public async Task<ActionResult<IEnumerable<LoadTag>>> GetLoadTags(long loadId)
        {
            try
            {
                var tags = await _loadTagService.GetTagsByLoadIdAsync(loadId);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tags for load with ID {LoadId}", loadId);
                return StatusCode(500, "An error occurred while retrieving load tags");
            }
        }

        [HttpPost("load/{loadId}/tag/{tagId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignTagToLoad(long loadId, int tagId)
        {
            try
            {
                var result = await _loadTagService.AssignTagToLoadAsync(tagId, loadId);
                if (!result)
                {
                    return BadRequest("Failed to assign tag to load");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning tag {TagId} to load {LoadId}", tagId, loadId);
                return StatusCode(500, "An error occurred while assigning the tag to the load");
            }
        }

        [HttpDelete("load/{loadId}/tag/{tagId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveTagFromLoad(long loadId, int tagId)
        {
            try
            {
                var result = await _loadTagService.RemoveTagFromLoadAsync(tagId, loadId);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing tag {TagId} from load {LoadId}", tagId, loadId);
                return StatusCode(500, "An error occurred while removing the tag from the load");
            }
        }
    }
} 