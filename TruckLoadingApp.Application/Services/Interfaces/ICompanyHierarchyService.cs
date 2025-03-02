using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ICompanyHierarchyService
    {
        Task<CompanyHierarchy> CreateDepartmentAsync(
            string companyId,
            string departmentName,
            string? description,
            int? parentDepartmentId);

        Task<CompanyHierarchy?> GetDepartmentByIdAsync(int id);

        Task<IEnumerable<CompanyHierarchy>> GetDepartmentsByCompanyAsync(
            string companyId,
            bool includeSubDepartments = true);

        Task<IEnumerable<CompanyHierarchy>> GetSubDepartmentsAsync(int departmentId);

        Task<CompanyHierarchy> UpdateDepartmentAsync(
            int id,
            string departmentName,
            string? description);

        Task<bool> DeleteDepartmentAsync(int id);

        Task<DepartmentMember> AddDepartmentMemberAsync(
            int departmentId,
            string userId,
            DepartmentRole role);

        Task<bool> RemoveDepartmentMemberAsync(
            int departmentId,
            string userId);

        Task<bool> UpdateDepartmentMemberRoleAsync(
            int departmentId,
            string userId,
            DepartmentRole newRole);

        Task<IEnumerable<DepartmentMember>> GetDepartmentMembersAsync(
            int departmentId,
            bool includeSubDepartments = false);

        Task<bool> MoveDepartmentAsync(
            int departmentId,
            int newParentDepartmentId);

        Task<IEnumerable<CompanyHierarchy>> GetUserDepartmentsAsync(string userId);

        Task<Dictionary<DepartmentRole, int>> GetDepartmentRoleDistributionAsync(
            int departmentId,
            bool includeSubDepartments = false);

        Task<int> GetDepartmentMemberCountAsync(
            int departmentId,
            bool includeSubDepartments = false);

        Task<IEnumerable<CompanyHierarchy>> GetDepartmentPathAsync(int departmentId);
    }
}
