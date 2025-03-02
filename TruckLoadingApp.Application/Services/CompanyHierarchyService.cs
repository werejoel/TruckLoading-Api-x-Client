using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class CompanyHierarchyService : ICompanyHierarchyService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserActivityService _userActivityService;

        public CompanyHierarchyService(
            ApplicationDbContext context,
            IUserActivityService userActivityService)
        {
            _context = context;
            _userActivityService = userActivityService;
        }

        public async Task<CompanyHierarchy> CreateDepartmentAsync(
            string companyId,
            string departmentName,
            string? description,
            int? parentDepartmentId)
        {
            var company = await _context.Users.FindAsync(companyId);
            if (company == null)
                throw new KeyNotFoundException($"Company with ID {companyId} not found.");

            int level = 0;
            if (parentDepartmentId.HasValue)
            {
                var parentDepartment = await _context.CompanyHierarchy.FindAsync(parentDepartmentId.Value);
                if (parentDepartment == null)
                    throw new KeyNotFoundException($"Parent department with ID {parentDepartmentId.Value} not found.");

                if (parentDepartment.CompanyId != companyId)
                    throw new InvalidOperationException("Parent department must belong to the same company.");

                level = parentDepartment.Level + 1;
            }

            var department = new CompanyHierarchy
            {
                CompanyId = companyId,
                DepartmentName = departmentName,
                Description = description,
                ParentDepartmentId = parentDepartmentId,
                Level = level
            };

            _context.CompanyHierarchy.Add(department);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                companyId,
                ActivityTypes.CreateDepartment,
                $"Created department: {departmentName}",
                "Department",
                department.Id.ToString());

            return department;
        }

        public async Task<CompanyHierarchy?> GetDepartmentByIdAsync(int id)
        {
            return await _context.CompanyHierarchy
                .Include(d => d.ParentDepartment)
                .Include(d => d.SubDepartments)
                .Include(d => d.DepartmentMembers)
                    .ThenInclude(dm => dm.User)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<CompanyHierarchy>> GetDepartmentsByCompanyAsync(
            string companyId,
            bool includeSubDepartments = true)
        {
            var query = _context.CompanyHierarchy
                .Where(d => d.CompanyId == companyId);

            if (includeSubDepartments)
            {
                query = query.Include(d => d.SubDepartments);
            }

            return await query.OrderBy(d => d.Level).ThenBy(d => d.DepartmentName).ToListAsync();
        }

        public async Task<IEnumerable<CompanyHierarchy>> GetSubDepartmentsAsync(int departmentId)
        {
            return await _context.CompanyHierarchy
                .Where(d => d.ParentDepartmentId == departmentId)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();
        }

        public async Task<CompanyHierarchy> UpdateDepartmentAsync(
            int id,
            string departmentName,
            string? description)
        {
            var department = await _context.CompanyHierarchy.FindAsync(id);
            if (department == null)
                throw new KeyNotFoundException($"Department with ID {id} not found.");

            department.DepartmentName = departmentName;
            department.Description = description;

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                department.CompanyId,
                ActivityTypes.UpdateDepartment,
                $"Updated department: {departmentName}",
                "Department",
                id.ToString());

            return department;
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _context.CompanyHierarchy
                .Include(d => d.SubDepartments)
                .Include(d => d.DepartmentMembers)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (department == null)
                return false;

            if (department.SubDepartments.Any())
                throw new InvalidOperationException("Cannot delete department with sub-departments.");

            _context.DepartmentMembers.RemoveRange(department.DepartmentMembers);
            _context.CompanyHierarchy.Remove(department);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                department.CompanyId,
                ActivityTypes.DeleteDepartment,
                $"Deleted department: {department.DepartmentName}",
                "Department",
                id.ToString());

            return true;
        }

        public async Task<DepartmentMember> AddDepartmentMemberAsync(
            int departmentId,
            string userId,
            DepartmentRole role)
        {
            var department = await _context.CompanyHierarchy.FindAsync(departmentId);
            if (department == null)
                throw new KeyNotFoundException($"Department with ID {departmentId} not found.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            var existingMember = await _context.DepartmentMembers
                .FirstOrDefaultAsync(dm => dm.DepartmentId == departmentId && dm.UserId == userId);

            if (existingMember != null)
                throw new InvalidOperationException($"User {userId} is already a member of department {departmentId}.");

            var departmentMember = new DepartmentMember
            {
                DepartmentId = departmentId,
                UserId = userId,
                Role = role
            };

            _context.DepartmentMembers.Add(departmentMember);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                department.CompanyId,
                ActivityTypes.AddDepartmentMember,
                $"Added user {userId} to department {department.DepartmentName} with role {role}",
                "DepartmentMember",
                departmentMember.Id.ToString());

            return departmentMember;
        }

        public async Task<bool> RemoveDepartmentMemberAsync(int departmentId, string userId)
        {
            var departmentMember = await _context.DepartmentMembers
                .FirstOrDefaultAsync(dm => dm.DepartmentId == departmentId && dm.UserId == userId);

            if (departmentMember == null)
                return false;

            var department = await _context.CompanyHierarchy.FindAsync(departmentId);

            _context.DepartmentMembers.Remove(departmentMember);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                department!.CompanyId,
                ActivityTypes.RemoveDepartmentMember,
                $"Removed user {userId} from department {department.DepartmentName}",
                "DepartmentMember",
                departmentMember.Id.ToString());

            return true;
        }

        public async Task<bool> UpdateDepartmentMemberRoleAsync(
            int departmentId,
            string userId,
            DepartmentRole newRole)
        {
            var departmentMember = await _context.DepartmentMembers
                .FirstOrDefaultAsync(dm => dm.DepartmentId == departmentId && dm.UserId == userId);

            if (departmentMember == null)
                return false;

            var department = await _context.CompanyHierarchy.FindAsync(departmentId);

            departmentMember.Role = newRole;
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                department!.CompanyId,
                ActivityTypes.UpdateDepartmentMember,
                $"Updated role for user {userId} in department {department.DepartmentName} to {newRole}",
                "DepartmentMember",
                departmentMember.Id.ToString());

            return true;
        }

        public async Task<IEnumerable<DepartmentMember>> GetDepartmentMembersAsync(
            int departmentId,
            bool includeSubDepartments = false)
        {
            if (!includeSubDepartments)
            {
                return await _context.DepartmentMembers
                    .Where(dm => dm.DepartmentId == departmentId)
                    .Include(dm => dm.User)
                    .ToListAsync();
            }

            var department = await _context.CompanyHierarchy
                .Include(d => d.SubDepartments)
                .FirstOrDefaultAsync(d => d.Id == departmentId);

            if (department == null)
                return Enumerable.Empty<DepartmentMember>();

            var departmentIds = new List<int> { departmentId };
            departmentIds.AddRange(department.SubDepartments.Select(d => d.Id));

            return await _context.DepartmentMembers
                .Where(dm => departmentIds.Contains(dm.DepartmentId))
                .Include(dm => dm.User)
                .ToListAsync();
        }

        public async Task<bool> MoveDepartmentAsync(int departmentId, int newParentDepartmentId)
        {
            var department = await _context.CompanyHierarchy.FindAsync(departmentId);
            if (department == null)
                return false;

            var newParent = await _context.CompanyHierarchy.FindAsync(newParentDepartmentId);
            if (newParent == null)
                return false;

            if (department.CompanyId != newParent.CompanyId)
                throw new InvalidOperationException("Cannot move department to a different company.");

            if (await IsDepartmentAncestor(department.Id, newParentDepartmentId))
                throw new InvalidOperationException("Cannot move department to its own subdepartment.");

            department.ParentDepartmentId = newParentDepartmentId;
            department.Level = newParent.Level + 1;

            await UpdateSubDepartmentLevels(department);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                department.CompanyId,
                ActivityTypes.UpdateDepartment,
                $"Moved department {department.DepartmentName} to new parent department",
                "Department",
                departmentId.ToString());

            return true;
        }

        public async Task<IEnumerable<CompanyHierarchy>> GetUserDepartmentsAsync(string userId)
        {
            return await _context.DepartmentMembers
                .Where(dm => dm.UserId == userId)
                .Include(dm => dm.Department)
                .Select(dm => dm.Department)
                .ToListAsync();
        }

        public async Task<Dictionary<DepartmentRole, int>> GetDepartmentRoleDistributionAsync(
            int departmentId,
            bool includeSubDepartments = false)
        {
            var members = await GetDepartmentMembersAsync(departmentId, includeSubDepartments);
            return members
                .GroupBy(dm => dm.Role)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<int> GetDepartmentMemberCountAsync(
            int departmentId,
            bool includeSubDepartments = false)
        {
            if (!includeSubDepartments)
            {
                return await _context.DepartmentMembers
                    .CountAsync(dm => dm.DepartmentId == departmentId);
            }

            var department = await _context.CompanyHierarchy
                .Include(d => d.SubDepartments)
                .FirstOrDefaultAsync(d => d.Id == departmentId);

            if (department == null)
                return 0;

            var departmentIds = new List<int> { departmentId };
            departmentIds.AddRange(department.SubDepartments.Select(d => d.Id));

            return await _context.DepartmentMembers
                .CountAsync(dm => departmentIds.Contains(dm.DepartmentId));
        }

        public async Task<IEnumerable<CompanyHierarchy>> GetDepartmentPathAsync(int departmentId)
        {
            var path = new List<CompanyHierarchy>();
            var current = await _context.CompanyHierarchy
                .Include(d => d.ParentDepartment)
                .FirstOrDefaultAsync(d => d.Id == departmentId);

            while (current != null)
            {
                path.Add(current);
                current = current.ParentDepartment;
            }

            path.Reverse();
            return path;
        }

        private async Task<bool> IsDepartmentAncestor(int departmentId, int ancestorId)
        {
            var current = await _context.CompanyHierarchy.FindAsync(departmentId);
            while (current?.ParentDepartmentId != null)
            {
                if (current.ParentDepartmentId == ancestorId)
                    return true;
                current = await _context.CompanyHierarchy.FindAsync(current.ParentDepartmentId);
            }
            return false;
        }

        private async Task UpdateSubDepartmentLevels(CompanyHierarchy department)
        {
            var subDepartments = await _context.CompanyHierarchy
                .Where(d => d.ParentDepartmentId == department.Id)
                .ToListAsync();

            foreach (var subDepartment in subDepartments)
            {
                subDepartment.Level = department.Level + 1;
                await UpdateSubDepartmentLevels(subDepartment);
            }
        }
    }
}
