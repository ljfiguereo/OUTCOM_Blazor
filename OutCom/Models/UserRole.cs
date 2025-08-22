using System.ComponentModel.DataAnnotations;

namespace OutCom.Models
{
    public enum UserType
    {
        [Display(Name = "Cliente")]
        Client = 0,
        
        [Display(Name = "Administrador")]
        Admin = 1
    }

    public class UserRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        public UserType UserType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public virtual ICollection<UserRoleAssignment> UserRoleAssignments { get; set; } = new List<UserRoleAssignment>();
    }

    public class UserRoleAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int UserRoleId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public string AssignedBy { get; set; } = string.Empty;

        // Navegación
        public virtual UserRole UserRole { get; set; } = null!;
    }
}