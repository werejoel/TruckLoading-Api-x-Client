using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.TruckManagement
{
    [ApiController]
    [Route("api/truck-types")]
    [Authorize]
    public class TruckTypeController : ControllerBase
    {
        private readonly ITruckTypeService _truckTypeService;
        private readonly ILogger<TruckTypeController> _logger;

        public TruckTypeController(
            ITruckTypeService truckTypeService,
            ILogger<TruckTypeController> logger)
        {
            _truckTypeService = truckTypeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TruckType>>> GetAllTruckTypes()
        {
            try
            {
                var truckTypes = await _truckTypeService.GetAllTruckTypesAsync();
                return Ok(truckTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving truck types");
                return StatusCode(500, new { Message = "An error occurred while retrieving truck types", Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TruckType>> GetTruckTypeById(int id)
        {
            try
            {
                var truckType = await _truckTypeService.GetTruckTypeByIdAsync(id);
                if (truckType == null)
                {
                    return NotFound();
                }
                return Ok(truckType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving truck type with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the truck type");
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<TruckCategory>>> GetAllCategories()
        {
            try
            {
                var categories = await _truckTypeService.GetAllTruckCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving truck categories");
                return StatusCode(500, new { Message = "An error occurred while retrieving truck categories", Error = ex.Message });
            }
        }

        [HttpGet("categories/{id}")]
        public async Task<ActionResult<TruckCategory>> GetCategoryById(int id)
        {
            try
            {
                var category = await _truckTypeService.GetTruckCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving truck category with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the truck category");
            }
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<TruckType>>> GetTruckTypesByCategory(int categoryId)
        {
            try
            {
                var truckTypes = await _truckTypeService.GetTruckTypesByCategoryIdAsync(categoryId);
                return Ok(truckTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving truck types for category ID {CategoryId}", categoryId);
                return StatusCode(500, "An error occurred while retrieving the truck types");
            }
        }
    }
} 