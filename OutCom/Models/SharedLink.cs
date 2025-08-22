using System.ComponentModel.DataAnnotations;

namespace OutCom.Models
{
    public class SharedLink
    {
        [Key]
        public Guid Id { get; set; } // El ID único que irá en la URL

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string OwnerUserId { get; set; } = string.Empty; // A quién pertenece el archivo

        public DateTime? ExpirationDate { get; set; } // Opcional: para enlaces que expiran
    }
}
