using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers
{
    //[EnableRateLimiting("default")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyHierarchyController : ControllerBase
    {
        private readonly ICompanyHierarchyService _companyHierarchyService;

        public CompanyHierarchyController(ICompanyHierarchyService companyHierarchyService)
        {
            _companyHierarchyService = companyHierarchyService;
        }

        [HttpPost("departments")]
        [Authorize(Policy = "ManageCompanyHierarchy")]
        public async Task<ActionResult<CompanyHierarchy>> CreateDepartment([FromBody] CreateDepartmentRequest request)
        {
            try
            {
                var department = await _companyHierarchyService.CreateDepartmentAsync(
                    request.CompanyId,
                    request.DepartmentName,
                    request.Description,
                    request.ParentDepartmentId);

                return CreatedAtAction(
                    nameof(GetDepartment),
                    new { id = department.Id },
                    department);
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

        [HttpGet("departments/{id}")]
        [Authorize(Policy = "ViewCompanyHierarchy")]
        public async Task<ActionResult<CompanyHierarchy>> GetDepartment(int id)
        {
            var department = await _companyHierarchyService.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound();

            return Ok(department);
        }

        [HttpGet("companies/{companyId}/departments")]
        [Authorize(Policy = "ViewCompanyHierarchy")]
        public async Task<ActionResult<IEnumerable<CompanyHierarchy>>> GetCompanyDepartments(
            string companyId,
            [FromQuery] bool includeSubDepartments = true)
        {
            var departments = await _companyHierarchyService.GetDepartmentsByCompanyAsync(
                companyId,
                includeSubDepartments);

            return Ok(departments);
        }

        [HttpGet("departments/{id}/subdepartments")]
        [Authorize(Policy = "ViewCompanyHierarchy")]
        public async Task<ActionResult<IEnumerable<CompanyHierarchy>>> GetSubDepartments(int id)
        {
            var departments = await _companyHierarchyService.GetSubDepartmentsAsync(id);
            return Ok(departments);
        }

        [HttpPut("departments/{id}")]
        [Authorize(Policy = "ManageCompanyHierarchy")]
        public async Task<ActionResult<CompanyHierarchy>> UpdateDepartment(
            int id,
            [FromBody] UpdateDepartmentRequest request)
        {
            try
            {
                var department = await _companyHierarchyService.UpdateDepartmentAsync(
                    id,
                    request.DepartmentName,
                    request.Description);

                return Ok(department);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("departments/{id}")]
        [Authorize(Policy = "ManageCompanyHierarchy")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var result = await _companyHierarchyService.DeleteDepartmentAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("departments/{departmentId}/members")]
        [Authorize(Policy = "ManageCompanyHierarchy")]
        public async Task<ActionResult<DepartmentMember>> AddDepartmentMember(
            int departmentId,
            [FromBody] AddDepartmentMemberRequest request)
        {
            try
            {
                var member = await _companyHierarchyService.AddDepartmentMemberAsync(
                    departmentId,
                    request.UserId,
                    request.Role);

                return CreatedAtAction(
                    nameof(GetDepartmentMembers),
                    new { departmentId },
                    member);
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

        [HttpDelete("departments/{departmentId}/members/{userId}")]
        [Authorize(Policy = "ManageCompanyHierarchy")]
        public async Task<IActionResult> RemoveDepartmentMember(int departmentId, string userId)
        {
            var result = await _companyHierarchyService.RemoveDepartmentMemberAsync(departmentId, userId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPut("departments/{departmentId}/members/{userId}/role")]
        [Authorize(Policy = "ManageCompanyHierarchy")]
        public async Task<IActionResult> UpdateDepartmentMemberRole(
            int departmentId,
            string userId,
            [FromBody] UpdateDepartmentMemberRoleRequest request)
        {
            var result = await _companyHierarchyService.UpdateDepartmentMemberRoleAsync(
                departmentId,
                userId,
                request.NewRole);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("departments/{departmentId}/members")]
        [Authorize(Policy = "ViewCompanyHierarchy")]
        public async Task<ActionResult<IEnumerable<DepartmentMember>>> GetDepartmentMembers(
            int departmentId,
            [FromQuery] bool includeSubDepartments = false)
        {
            var members = await _companyHierarchyService.GetDepartmentMembersAsync(
                departmentId,
                includeSubDepartments);

            return Ok(members);
        }

        [HttpPost("departments/{departmentId}/move/{newParentId}")]
        [Authorize(Policy = "ManageCompanyHierarchy")]
        public async Task<IActionResult> MoveDepartment(int departmentId, int newParentId)
        {
            try
            {
                var result = await _companyHierarchyService.MoveDepartmentAsync(departmentId, newParentId);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("users/{userId}/departments")]
        [Authorize(Policy = "ViewCompanyHierarchy")]
        public async Task<ActionResult<IEnumerable<CompanyHierarchy>>> GetUserDepartments(string userId)
        {
            var departments = await _companyHierarchyService.GetUserDepartmentsAsync(userId);
            return Ok(departments);
        }

        [HttpGet("departments/{departmentId}/metrics")]
        [Authorize(Policy = "ViewCompanyHierarchy")]
        public async Task<ActionResult<DepartmentMetrics>> GetDepartmentMetrics(
            int departmentId,
            [FromQuery] bool includeSubDepartments = false)
        {
            var memberCount = await _companyHierarchyService.GetDepartmentMemberCountAsync(
                departmentId,
                includeSubDepartments);

            var roleDistribution = await _companyHierarchyService.GetDepartmentRoleDistributionAsync(
                departmentId,
                includeSubDepartments);

            return Ok(new DepartmentMetrics
            {
                MemberCount = memberCount,
                RoleDistribution = roleDistribution
            });
        }

        [HttpGet("departments/{departmentId}/path")]
        [Authorize(Policy = "ViewCompanyHierarchy")]
        public async Task<ActionResult<IEnumerable<CompanyHierarchy>>> GetDepartmentPath(int departmentId)
        {
            var path = await _companyHierarchyService.GetDepartmentPathAsync(departmentId);
            return Ok(path);
        }
    }

    public class CreateDepartmentRequest
    {
        public string CompanyId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentDepartmentId { get; set; }
    }

    public class UpdateDepartmentRequest
    {
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class AddDepartmentMemberRequest
    {
        public string UserId { get; set; } = string.Empty;
        public DepartmentRole Role { get; set; }
    }

    public class UpdateDepartmentMemberRoleRequest
    {
        public DepartmentRole NewRole { get; set; }
    }

    public class DepartmentMetrics
    {
        public int MemberCount { get; set; }
        public Dictionary<DepartmentRole, int> RoleDistribution { get; set; } = new();
    }
}
