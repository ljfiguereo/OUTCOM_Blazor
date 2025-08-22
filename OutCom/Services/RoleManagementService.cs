using OutCom.Data;
using OutCom.Models;
using Microsoft.EntityFrameworkCore;

namespace OutCom.Services
{
    public class RoleManagementService : IRoleManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public RoleManagementService(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IEnumerable<UserRole>> GetAllUserRolesAsync()
        {
            return await _context.UserRoles.OrderBy(r => r.Name).ToListAsync();
        }

        public async Task<UserRole?> GetUserRoleByIdAsync(int id)
        {
            return await _context.UserRoles.FindAsync(id);
        }

        public async Task<UserRole?> GetUserRoleByNameAsync(string name)
        {
            return await _context.UserRoles.FirstOrDefaultAsync(r => r.Name == name);
        }

        public async Task<bool> CreateUserRoleAsync(string name, string description, UserType userType, string createdBy)
        {
            try
            {
                var existingRole = await GetUserRoleByNameAsync(name);
                if (existingRole != null)
                {
                    return false; // El rol ya existe
                }

                var userRole = new UserRole
                {
                    Name = name,
                    Description = description,
                    UserType = userType,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(AuditAction.RoleCreated, createdBy, $"Rol creado: {name} (Tipo: {userType})");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(UserRole userRole, string adminUserId)
        {
            try
            {
                _context.UserRoles.Update(userRole);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(AuditAction.RoleUpdated, adminUserId, $"Rol actualizado: {userRole.Name}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserRoleAsync(int id, string adminUserId)
        {
            try
            {
                var userRole = await GetUserRoleByIdAsync(id);
                if (userRole == null)
                {
                    return false;
                }

                // Verificar si hay usuarios asignados a este rol
                var hasAssignments = await _context.UserRoleAssignments.AnyAsync(ura => ura.UserRoleId == id);
                if (hasAssignments)
                {
                    return false; // No se puede eliminar un rol que tiene usuarios asignados
                }

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(AuditAction.RoleDeleted, adminUserId, $"Rol eliminado: {userRole.Name}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<UserRoleAssignment>> GetUserRoleAssignmentsAsync(string userId)
        {
            return await _context.UserRoleAssignments
                .Include(ura => ura.UserRole)
                .Where(ura => ura.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserRoleAssignment>> GetAllUserRoleAssignmentsAsync()
        {
            return await _context.UserRoleAssignments
                .Include(ura => ura.UserRole)
                .ToListAsync();
        }

        public async Task<bool> AssignRoleToUserAsync(string userId, int roleId, string adminUserId)
        {
            try
            {
                // Verificar si la asignación ya existe
                var existingAssignment = await _context.UserRoleAssignments
                    .FirstOrDefaultAsync(ura => ura.UserId == userId && ura.UserRoleId == roleId);

                if (existingAssignment != null)
                {
                    return false; // La asignación ya existe
                }

                var assignment = new UserRoleAssignment
                {
                    UserId = userId,
                    UserRoleId = roleId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = adminUserId
                };

                _context.UserRoleAssignments.Add(assignment);
                await _context.SaveChangesAsync();

                var userRole = await GetUserRoleByIdAsync(roleId);
                await _auditService.LogAsync(AuditAction.RoleAssigned, adminUserId, $"Rol '{userRole?.Name}' asignado a usuario ID: {userId}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveRoleFromUserAsync(string userId, int roleId, string adminUserId)
        {
            try
            {
                var assignment = await _context.UserRoleAssignments
                    .FirstOrDefaultAsync(ura => ura.UserId == userId && ura.UserRoleId == roleId);

                if (assignment == null)
                {
                    return false; // La asignación no existe
                }

                _context.UserRoleAssignments.Remove(assignment);
                await _context.SaveChangesAsync();

                var userRole = await GetUserRoleByIdAsync(roleId);
                await _auditService.LogAsync(AuditAction.RoleRemoved, adminUserId, $"Rol '{userRole?.Name}' removido de usuario ID: {userId}");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(string userId)
        {
            var assignments = await GetUserRoleAssignmentsAsync(userId);
            var permissions = new List<string>();

            foreach (var assignment in assignments)
            {
                if (assignment.UserRole != null)
                {
                    // Agregar permisos basados en el tipo de rol
                    switch (assignment.UserRole.UserType)
                    {
                        case UserType.Admin:
                            permissions.AddRange(new[]
                            {
                                "CreateUser",
                                "UpdateUser",
                                "DeleteUser",
                                "ViewAllUsers",
                                "ChangePassword",
                                "ManageRoles",
                                "ViewAuditLogs",
                                "ManageSystem"
                            });
                            break;
                        case UserType.Client:
                            permissions.AddRange(new[]
                            {
                                "ViewProfile",
                                "UpdateProfile",
                                "ChangeOwnPassword",
                                "UploadFiles",
                                "ViewOwnFiles"
                            });
                            break;
                    }
                }
            }

            return permissions.Distinct();
        }

        public async Task<bool> HasPermissionAsync(string userId, string permission)
        {
            var permissions = await GetUserPermissionsAsync(userId);
            return permissions.Contains(permission);
        }
    }
}