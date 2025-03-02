using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserActivityService _userActivityService;

        public TeamService(ApplicationDbContext context, IUserActivityService userActivityService)
        {
            _context = context;
            _userActivityService = userActivityService;
        }

        public async Task<Team> CreateTeamAsync(string name, string description, string companyId, string teamLeaderId)
        {
            var company = await _context.Users.FindAsync(companyId);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {companyId} not found.");

            var teamLeader = await _context.Users.FindAsync(teamLeaderId);
            if (teamLeader == null)
                throw new KeyNotFoundException($"Team leader with ID {teamLeaderId} not found.");

            var team = new Team
            {
                Name = name,
                Description = description,
                CompanyId = companyId,
                TeamLeaderId = teamLeaderId
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            // Add team leader as a team member with Administrator role
            await AddTeamMemberAsync(team.Id, teamLeaderId, TeamRole.Administrator);

            await _userActivityService.LogActivityAsync(
                teamLeaderId,
                ActivityTypes.CreateTeam,
                $"Created team: {name}",
                "Team",
                team.Id.ToString());

            return team;
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.TeamMembers)
                .ThenInclude(tm => tm.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Team>> GetTeamsByCompanyAsync(string companyId)
        {
            return await _context.Teams
                .Where(t => t.CompanyId == companyId)
                .Include(t => t.TeamMembers)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsByMemberAsync(string userId)
        {
            return await _context.TeamMembers
                .Where(tm => tm.UserId == userId)
                .Include(tm => tm.Team)
                .Select(tm => tm.Team)
                .ToListAsync();
        }

        public async Task<Team> UpdateTeamAsync(int id, string name, string description)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null)
                throw new KeyNotFoundException($"Team with ID {id} not found.");

            team.Name = name;
            team.Description = description;

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                team.TeamLeaderId,
                ActivityTypes.UpdateTeam,
                $"Updated team: {name}",
                "Team",
                id.ToString());

            return team;
        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _context.Teams
                .Include(t => t.TeamMembers)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
                return false;

            _context.TeamMembers.RemoveRange(team.TeamMembers);
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                team.TeamLeaderId,
                ActivityTypes.DeleteTeam,
                $"Deleted team: {team.Name}",
                "Team",
                id.ToString());

            return true;
        }

        public async Task<TeamMember> AddTeamMemberAsync(int teamId, string userId, TeamRole role)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            var existingMember = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (existingMember != null)
                throw new InvalidOperationException($"User {userId} is already a member of team {teamId}.");

            var teamMember = new TeamMember
            {
                TeamId = teamId,
                UserId = userId,
                Role = role
            };

            _context.TeamMembers.Add(teamMember);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                team.TeamLeaderId,
                ActivityTypes.AddTeamMember,
                $"Added user {userId} to team {team.Name} with role {role}",
                "TeamMember",
                teamMember.Id.ToString());

            return teamMember;
        }

        public async Task<bool> RemoveTeamMemberAsync(int teamId, string userId)
        {
            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (teamMember == null)
                return false;

            var team = await _context.Teams.FindAsync(teamId);
            if (team?.TeamLeaderId == userId)
                throw new InvalidOperationException("Cannot remove team leader from the team.");

            _context.TeamMembers.Remove(teamMember);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                team!.TeamLeaderId,
                ActivityTypes.RemoveTeamMember,
                $"Removed user {userId} from team {team.Name}",
                "TeamMember",
                teamMember.Id.ToString());

            return true;
        }

        public async Task<bool> UpdateTeamMemberRoleAsync(int teamId, string userId, TeamRole newRole)
        {
            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            if (teamMember == null)
                return false;

            var team = await _context.Teams.FindAsync(teamId);
            if (team?.TeamLeaderId == userId && newRole != TeamRole.Administrator)
                throw new InvalidOperationException("Team leader must maintain Administrator role.");

            teamMember.Role = newRole;
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                team!.TeamLeaderId,
                ActivityTypes.UpdateTeamMember,
                $"Updated role for user {userId} in team {team.Name} to {newRole}",
                "TeamMember",
                teamMember.Id.ToString());

            return true;
        }

        public async Task<IEnumerable<TeamMember>> GetTeamMembersAsync(int teamId)
        {
            return await _context.TeamMembers
                .Where(tm => tm.TeamId == teamId)
                .Include(tm => tm.User)
                .ToListAsync();
        }

        public async Task<bool> IsUserInTeamAsync(int teamId, string userId)
        {
            return await _context.TeamMembers
                .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        }

        public async Task<TeamRole?> GetUserTeamRoleAsync(int teamId, string userId)
        {
            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

            return teamMember?.Role;
        }

        public async Task<bool> ChangeTeamLeaderAsync(int teamId, string newLeaderId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team == null)
                return false;

            var newLeader = await _context.Users.FindAsync(newLeaderId);
            if (newLeader == null)
                return false;

            var oldLeaderId = team.TeamLeaderId;
            team.TeamLeaderId = newLeaderId;

            // Ensure new leader is a team member with Administrator role
            var newLeaderMember = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == newLeaderId);

            if (newLeaderMember == null)
            {
                await AddTeamMemberAsync(teamId, newLeaderId, TeamRole.Administrator);
            }
            else if (newLeaderMember.Role != TeamRole.Administrator)
            {
                newLeaderMember.Role = TeamRole.Administrator;
            }

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                oldLeaderId,
                ActivityTypes.UpdateTeam,
                $"Changed team leader from {oldLeaderId} to {newLeaderId}",
                "Team",
                teamId.ToString());

            return true;
        }

        public async Task<bool> IsUserTeamLeaderAsync(int teamId, string userId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            return team?.TeamLeaderId == userId;
        }

        public async Task<int> GetTeamMemberCountAsync(int teamId)
        {
            return await _context.TeamMembers
                .CountAsync(tm => tm.TeamId == teamId);
        }

        public async Task<Dictionary<TeamRole, int>> GetTeamRoleDistributionAsync(int teamId)
        {
            return await _context.TeamMembers
                .Where(tm => tm.TeamId == teamId)
                .GroupBy(tm => tm.Role)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Count());
        }
    }
}
