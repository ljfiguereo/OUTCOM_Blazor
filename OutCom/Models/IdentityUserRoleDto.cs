using Microsoft.AspNetCore.Identity;
using OutCom.Data;

namespace OutCom.Models
{
    /// <summary>
    /// DTO para representar las asignaciones de roles de Identity
    /// </summary>
    public class IdentityUserRoleDto
    {
        public string UserId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserFirstName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        
        // Propiedades de navegación virtuales para facilitar el acceso
        public ApplicationUser? User { get; set; }
        public IdentityRole? Role { get; set; }
    }
}