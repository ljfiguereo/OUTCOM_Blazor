using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OutCom.Models;

namespace OutCom.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : 
        IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<SharedLink> SharedLinks { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserRoleAssignment> UserRoleAssignments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuración de UserRoleAssignment
            builder.Entity<UserRoleAssignment>()
                .HasOne(ura => ura.UserRole)
                .WithMany(ur => ur.UserRoleAssignments)
                .HasForeignKey(ura => ura.UserRoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de ApplicationUser
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.UserRoleAssignments)
                .WithOne()
                .HasForeignKey(ura => ura.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para mejorar rendimiento
            builder.Entity<AuditLog>()
                .HasIndex(al => al.UserId);

            builder.Entity<AuditLog>()
                .HasIndex(al => al.Timestamp);

            builder.Entity<AuditLog>()
                .HasIndex(al => al.Action);

            builder.Entity<UserRoleAssignment>()
                .HasIndex(ura => ura.UserId);

            // Datos iniciales para roles
            builder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = 1,
                    Name = "Administrador",
                    Description = "Acceso completo al sistema con permisos administrativos",
                    UserType = UserType.Admin,
                    CreatedAt = DateTime.UtcNow
                },
                new UserRole
                {
                    Id = 2,
                    Name = "Cliente",
                    Description = "Usuario estándar con acceso a funcionalidades básicas",
                    UserType = UserType.Client,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
