using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ITeamService
    {
        Task<Team> CreateTeamAsync(string name, string description, string companyId, string teamLeaderId);
        Task<Team?> GetTeamByIdAsync(int id);
        Task<IEnumerable<Team>> GetTeamsByCompanyAsync(string companyId);
        Task<IEnumerable<Team>> GetTeamsByMemberAsync(string userId);
        Task<Team> UpdateTeamAsync(int id, string name, string description);
        Task<bool> DeleteTeamAsync(int id);

        // Team Member Management
        Task<TeamMember> AddTeamMemberAsync(int teamId, string userId, TeamRole role);
        Task<bool> RemoveTeamMemberAsync(int teamId, string userId);
        Task<bool> UpdateTeamMemberRoleAsync(int teamId, string userId, TeamRole newRole);
        Task<IEnumerable<TeamMember>> GetTeamMembersAsync(int teamId);
        Task<bool> IsUserInTeamAsync(int teamId, string userId);
        Task<TeamRole?> GetUserTeamRoleAsync(int teamId, string userId);

        // Team Leader Management
        Task<bool> ChangeTeamLeaderAsync(int teamId, string newLeaderId);
        Task<bool> IsUserTeamLeaderAsync(int teamId, string userId);

        // Team Metrics
        Task<int> GetTeamMemberCountAsync(int teamId);
        Task<Dictionary<TeamRole, int>> GetTeamRoleDistributionAsync(int teamId);
    }
}
