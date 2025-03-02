using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers
{
   // [EnableRateLimiting("default")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpPost]
        [Authorize(Policy = Permissions.CreateTeams)]
        public async Task<ActionResult<Team>> CreateTeam([FromBody] CreateTeamRequest request)
        {
            try
            {
                var team = await _teamService.CreateTeamAsync(
                    request.Name,
                    request.Description,
                    request.CompanyId,
                    request.TeamLeaderId);

                return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, team);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = Permissions.ViewTeams)]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
                return NotFound();

            return Ok(team);
        }

        [HttpGet("company/{companyId}")]
        [Authorize(Policy = Permissions.ViewTeams)]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeamsByCompany(string companyId)
        {
            var teams = await _teamService.GetTeamsByCompanyAsync(companyId);
            return Ok(teams);
        }

        [HttpGet("member/{userId}")]
        [Authorize(Policy = Permissions.ViewTeams)]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeamsByMember(string userId)
        {
            var teams = await _teamService.GetTeamsByMemberAsync(userId);
            return Ok(teams);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Permissions.UpdateTeams)]
        public async Task<ActionResult<Team>> UpdateTeam(int id, [FromBody] UpdateTeamRequest request)
        {
            try
            {
                var team = await _teamService.UpdateTeamAsync(id, request.Name, request.Description);
                return Ok(team);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Permissions.DeleteTeams)]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var result = await _teamService.DeleteTeamAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{teamId}/members")]
        [Authorize(Policy = Permissions.ManageTeamMembers)]
        public async Task<ActionResult<TeamMember>> AddTeamMember(int teamId, [FromBody] AddTeamMemberRequest request)
        {
            try
            {
                var teamMember = await _teamService.AddTeamMemberAsync(
                    teamId,
                    request.UserId,
                    request.Role);

                return CreatedAtAction(nameof(GetTeamMembers), new { teamId }, teamMember);
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

        [HttpDelete("{teamId}/members/{userId}")]
        [Authorize(Policy = Permissions.ManageTeamMembers)]
        public async Task<IActionResult> RemoveTeamMember(int teamId, string userId)
        {
            try
            {
                var result = await _teamService.RemoveTeamMemberAsync(teamId, userId);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{teamId}/members/{userId}/role")]
        [Authorize(Policy = Permissions.ManageTeamMembers)]
        public async Task<IActionResult> UpdateTeamMemberRole(
            int teamId,
            string userId,
            [FromBody] UpdateTeamMemberRoleRequest request)
        {
            try
            {
                var result = await _teamService.UpdateTeamMemberRoleAsync(teamId, userId, request.NewRole);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{teamId}/members")]
        [Authorize(Policy = Permissions.ViewTeams)]
        public async Task<ActionResult<IEnumerable<TeamMember>>> GetTeamMembers(int teamId)
        {
            var members = await _teamService.GetTeamMembersAsync(teamId);
            return Ok(members);
        }

        [HttpGet("{teamId}/members/{userId}/role")]
        [Authorize(Policy = Permissions.ViewTeams)]
        public async Task<ActionResult<TeamRole>> GetTeamMemberRole(int teamId, string userId)
        {
            var role = await _teamService.GetUserTeamRoleAsync(teamId, userId);
            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpPut("{teamId}/leader")]
        [Authorize(Policy = Permissions.ManageTeamMembers)]
        public async Task<IActionResult> ChangeTeamLeader(int teamId, [FromBody] ChangeTeamLeaderRequest request)
        {
            var result = await _teamService.ChangeTeamLeaderAsync(teamId, request.NewLeaderId);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("{teamId}/metrics")]
        [Authorize(Policy = Permissions.ViewTeams)]
        public async Task<ActionResult<TeamMetrics>> GetTeamMetrics(int teamId)
        {
            var memberCount = await _teamService.GetTeamMemberCountAsync(teamId);
            var roleDistribution = await _teamService.GetTeamRoleDistributionAsync(teamId);

            return Ok(new TeamMetrics
            {
                MemberCount = memberCount,
                RoleDistribution = roleDistribution
            });
        }
    }

    public class CreateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;
        public string TeamLeaderId { get; set; } = string.Empty;
    }

    public class UpdateTeamRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class AddTeamMemberRequest
    {
        public string UserId { get; set; } = string.Empty;
        public TeamRole Role { get; set; }
    }

    public class UpdateTeamMemberRoleRequest
    {
        public TeamRole NewRole { get; set; }
    }

    public class ChangeTeamLeaderRequest
    {
        public string NewLeaderId { get; set; } = string.Empty;
    }

    public class TeamMetrics
    {
        public int MemberCount { get; set; }
        public Dictionary<TeamRole, int> RoleDistribution { get; set; } = new();
    }
}
