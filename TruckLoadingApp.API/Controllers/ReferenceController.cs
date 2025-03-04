using Microsoft.AspNetCore.Mvc;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Api.Controllers
{
    [ApiController]
    [Route("api/reference")]
    public class ReferenceController : ControllerBase
    {
        private readonly ITruckTypeService _truckTypeService;
        private readonly ILoadTypeService _loadTypeService;

        public ReferenceController(
            ITruckTypeService truckTypeService,
            ILoadTypeService loadTypeService)
        {
            _truckTypeService = truckTypeService;
            _loadTypeService = loadTypeService;
        }

        [HttpGet("truck-types")]
        public async Task<ActionResult<IEnumerable<TruckType>>> GetTruckTypes()
        {
            var types = await _truckTypeService.GetAllTruckTypesAsync();
            return Ok(types);
        }

        [HttpGet("truck-types/{id}")]
        public async Task<ActionResult<TruckType>> GetTruckTypeById(int id)
        {
            var truckType = await _truckTypeService.GetTruckTypeByIdAsync(id);
            if (truckType == null)
            {
                return NotFound();
            }
            return Ok(truckType);
        }

        [HttpGet("truck-categories")]
        public async Task<ActionResult<IEnumerable<TruckCategory>>> GetTruckCategories()
        {
            try
            {
                var categories = await _truckTypeService.GetAllTruckCategoriesAsync();
                
                // Add debug information
                var categoriesWithTypes = await Task.WhenAll(categories.Select(async c =>
                {
                    var types = await _truckTypeService.GetTruckTypesByCategoryIdAsync(c.Id);
                    return new
                    {
                        CategoryId = c.Id,
                        CategoryName = c.CategoryName,
                        IsActive = c.IsActive,
                        TruckTypes = types.Select(t => new { t.Id, t.Name, t.CategoryId }).ToList()
                    };
                }));
                
                return Ok(categoriesWithTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving categories", Error = ex.Message });
            }
        }

        [HttpGet("truck-categories/{id}")]
        public async Task<ActionResult<TruckCategory>> GetTruckCategoryById(int id)
        {
            var category = await _truckTypeService.GetTruckCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpGet("truck-categories/{categoryId}/types")]
        public async Task<ActionResult<IEnumerable<TruckType>>> GetTruckTypesByCategory(int categoryId)
        {
            try
            {
                var category = await _truckTypeService.GetTruckCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    return NotFound(new { Message = $"Category with ID {categoryId} not found" });
                }

                var types = await _truckTypeService.GetTruckTypesByCategoryIdAsync(categoryId);
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving truck types", Error = ex.Message });
            }
        }

        [HttpGet("load-types")]
        public async Task<ActionResult<IEnumerable<LoadType>>> GetLoadTypes()
        {
            try
            {
                var loadTypes = await _loadTypeService.GetAllLoadTypesAsync();
                return Ok(loadTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving load types", Error = ex.Message });
            }
        }

        [HttpGet("load-types/{id}")]
        public async Task<ActionResult<LoadType>> GetLoadTypeById(int id)
        {
            try
            {
                var loadType = await _loadTypeService.GetLoadTypeByIdAsync(id);
                if (loadType == null)
                {
                    return NotFound(new { Message = $"Load type with ID {id} not found" });
                }
                return Ok(loadType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving the load type", Error = ex.Message });
            }
        }
    }
} 