using OutCom.Data;
using OutCom.Models;
using Microsoft.AspNetCore.Identity;

namespace OutCom.Services
{
    public interface IUserManagementService
    {
        Task<IdentityResult> CreateUserAsync(string email, string password, string firstName, string lastName, UserType userType, string createdBy);
        Task<IdentityResult> UpdateUserPasswordAsync(string userId, string newPassword, string adminUserId);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<IEnumerable<ApplicationUser>> GetUsersByTypeAsync(UserType userType);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user, string adminUserId);
        Task<IdentityResult> DeactivateUserAsync(string userId, string adminUserId);
        Task<IdentityResult> ActivateUserAsync(string userId, string adminUserId);
        Task<bool> IsUserInRoleAsync(ApplicationUser user, string roleName);
        Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string roleName, string adminUserId);
        Task<IdentityResult> RemoveUserFromRoleAsync(ApplicationUser user, string roleName, string adminUserId);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
    }
}