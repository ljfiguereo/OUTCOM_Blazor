using OutCom.Data;
using OutCom.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace OutCom.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IAuditService auditService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _auditService = auditService;
        }

        public async Task<IdentityResult> CreateUserAsync(string email, string password, string firstName, string lastName, UserType userType, string createdBy)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                UserType = userType,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Asignar rol por defecto basado en el tipo de usuario
                string roleName = userType == UserType.Admin ? "Administrador" : "Cliente";
                await AddUserToRoleAsync(user, roleName, createdBy);

                await _auditService.LogAsync(AuditAction.UserCreated, createdBy, $"Usuario creado: {email} (Tipo: {userType})");
            }

            return result;
        }

        public async Task<IdentityResult> UpdateUserPasswordAsync(string userId, string newPassword, string adminUserId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                await _auditService.LogAsync(AuditAction.PasswordChanged, adminUserId, $"Contraseña cambiada para usuario: {user.Email}");
            }

            return result;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByTypeAsync(UserType userType)
        {
            return await _userManager.Users.Where(u => u.UserType == userType).ToListAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user, string adminUserId)
        {
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _auditService.LogAsync(AuditAction.UserUpdated, adminUserId, $"Usuario actualizado: {user.Email}");
            }

            return result;
        }

        public async Task<IdentityResult> DeactivateUserAsync(string userId, string adminUserId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });
            }

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _auditService.LogAsync(AuditAction.UserDeactivated, adminUserId, $"Usuario desactivado: {user.Email}");
            }

            return result;
        }

        public async Task<IdentityResult> ActivateUserAsync(string userId, string adminUserId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });
            }

            user.IsActive = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _auditService.LogAsync(AuditAction.UserActivated, adminUserId, $"Usuario activado: {user.Email}");
            }

            return result;
        }

        public async Task<bool> IsUserInRoleAsync(ApplicationUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string roleName, string adminUserId)
        {
            // Verificar si el rol existe
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                await _auditService.LogAsync(AuditAction.RoleAssigned, adminUserId, $"Rol '{roleName}' asignado a usuario: {user.Email}");
            }

            return result;
        }

        public async Task<IdentityResult> RemoveUserFromRoleAsync(ApplicationUser user, string roleName, string adminUserId)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                await _auditService.LogAsync(AuditAction.RoleRemoved, adminUserId, $"Rol '{roleName}' removido de usuario: {user.Email}");
            }

            return result;
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}