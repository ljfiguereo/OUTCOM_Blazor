using Microsoft.AspNetCore.Identity;
using OutCom.Models;
using System.ComponentModel.DataAnnotations;

namespace OutCom.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public UserType UserType { get; set; } = UserType.Client;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(200)]
        public string? CreatedBy { get; set; }

        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        // Propiedades calculadas
        public string FullName => $"{FirstName} {LastName}".Trim();

        // Navegación
        public virtual ICollection<UserRoleAssignment> UserRoleAssignments { get; set; } = new List<UserRoleAssignment>();
    }
}
