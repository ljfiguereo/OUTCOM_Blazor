using OutCom.Models;
using Microsoft.AspNetCore.Identity;

namespace OutCom.Services
{
    public interface IRoleManagementService
    {
        Task<IEnumerable<UserRole>> GetAllUserRolesAsync();
        Task<UserRole?> GetUserRoleByIdAsync(int id);
        Task<UserRole?> GetUserRoleByNameAsync(string name);
        Task<bool> CreateUserRoleAsync(string name, string description, UserType userType, string createdBy);
        Task<bool> UpdateUserRoleAsync(UserRole userRole, string adminUserId);
        Task<bool> DeleteUserRoleAsync(int id, string adminUserId);
        Task<IEnumerable<UserRoleAssignment>> GetUserRoleAssignmentsAsync(string userId);
        Task<bool> AssignRoleToUserAsync(string userId, int roleId, string adminUserId);
        Task<bool> RemoveRoleFromUserAsync(string userId, int roleId, string adminUserId);
        Task<IEnumerable<UserRoleAssignment>> GetAllUserRoleAssignmentsAsync();
        Task<IEnumerable<string>> GetUserPermissionsAsync(string userId);
        Task<bool> HasPermissionAsync(string userId, string permission);
        
        // Nuevos métodos para trabajar con AspNetUserRoles (Identity)
        Task<IEnumerable<IdentityUserRoleDto>> GetAllIdentityUserRoleAssignmentsAsync();
        Task<IEnumerable<IdentityUserRoleDto>> GetIdentityUserRoleAssignmentsAsync(string userId);
        Task<IEnumerable<IdentityRole>> GetAllIdentityRolesAsync();
    }
}