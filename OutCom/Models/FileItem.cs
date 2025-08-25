using System.ComponentModel.DataAnnotations;
using OutCom.Data;

namespace OutCom.Models
{
    public enum FileItemType
    {
        File = 0,
        Folder = 1
    }

    public class FileItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Path { get; set; } = string.Empty;

        [Required]
        public FileItemType Type { get; set; }

        public long Size { get; set; } = 0;

        [MaxLength(100)]
        public string? MimeType { get; set; }

        [Required]
        public string OwnerId { get; set; } = string.Empty; // Usuario que subió el archivo

        public string? ClientId { get; set; } // Cliente al que está asignado (null = visible para todos)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? Title { get; set; } // Título personalizado del archivo

        public DateTime? ExpirationDate { get; set; } // Fecha de expiración para eliminación automática

        // Propiedades para carpetas (sin navegación por ahora)
        public int? ParentFolderId { get; set; }

        // Navegación simplificada
        public virtual ApplicationUser Owner { get; set; } = null!;
        public virtual ICollection<FileShare> FileShares { get; set; } = new List<FileShare>();
    }

    public class FileShare
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FileItemId { get; set; }

        [Required]
        public string SharedWithUserId { get; set; } = string.Empty;

        [Required]
        public string SharedByUserId { get; set; } = string.Empty;

        public DateTime SharedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }

        public bool CanEdit { get; set; } = false;

        public bool CanDelete { get; set; } = false;

        public bool IsActive { get; set; } = true;

        // Navegación
        public virtual FileItem FileItem { get; set; } = null!;
        public virtual ApplicationUser SharedWithUser { get; set; } = null!;
        public virtual ApplicationUser SharedByUser { get; set; } = null!;
    }
}