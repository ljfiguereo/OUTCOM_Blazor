using System.ComponentModel.DataAnnotations;

namespace OutCom.Models
{
    public enum AuditAction
    {
        UserCreated,
        UserUpdated,
        UserDeleted,
        UserActivated,
        UserDeactivated,
        PasswordChanged,
        RoleAssigned,
        RoleRemoved,
        RoleCreated,
        RoleUpdated,
        RoleDeleted,
        Login,
        Logout,
        PermissionGranted,
        PermissionRevoked,
        AdminActionPerformed
    }

    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty; // Usuario que realizó la acción

        [Required]
        public string UserEmail { get; set; } = string.Empty; // Email del usuario para referencia

        [Required]
        public AuditAction Action { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public string? TargetUserId { get; set; } // Usuario afectado por la acción (si aplica)

        public string? TargetUserEmail { get; set; } // Email del usuario afectado

        [MaxLength(1000)]
        public string? AdditionalData { get; set; } // Datos adicionales en formato JSON

        [Required]
        [MaxLength(45)]
        public string IpAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string UserAgent { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsSuccessful { get; set; } = true;

        [MaxLength(500)]
        public string? ErrorMessage { get; set; }
    }
}