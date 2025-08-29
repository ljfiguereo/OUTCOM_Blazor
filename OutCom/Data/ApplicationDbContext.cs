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
        public DbSet<FileItem> FileItems { get; set; }
        public DbSet<OutCom.Models.FileShare> FileShares { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuraciones específicas para SQLite
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                // Configurar tipos de datos para Identity con SQLite
                builder.Entity<ApplicationUser>(entity =>
                {
                    entity.Property(e => e.ConcurrencyStamp).HasColumnType("TEXT");
                    entity.Property(e => e.SecurityStamp).HasColumnType("TEXT");
                    entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
                    entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
                });

                builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>(entity =>
                {
                    entity.Property(e => e.ConcurrencyStamp).HasColumnType("TEXT");
                    entity.Property(e => e.Name).HasMaxLength(256);
                    entity.Property(e => e.NormalizedName).HasMaxLength(256);
                });

                builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>(entity =>
                {
                    entity.Property(e => e.ClaimType).HasMaxLength(1024);
                    entity.Property(e => e.ClaimValue).HasMaxLength(1024);
                });

                builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>(entity =>
                {
                    entity.Property(e => e.ClaimType).HasMaxLength(1024);
                    entity.Property(e => e.ClaimValue).HasMaxLength(1024);
                });

                builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>(entity =>
                {
                    entity.Property(e => e.LoginProvider).HasMaxLength(128);
                    entity.Property(e => e.Name).HasMaxLength(128);
                    entity.Property(e => e.Value).HasMaxLength(1024);
                });

                builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>(entity =>
                {
                    entity.Property(e => e.LoginProvider).HasMaxLength(128);
                    entity.Property(e => e.ProviderKey).HasMaxLength(128);
                    entity.Property(e => e.ProviderDisplayName).HasMaxLength(256);
                });
            }

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

            // FileItem configuration
            builder.Entity<FileItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Path).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Size);
                entity.Property(e => e.MimeType).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ModifiedAt).IsRequired();

                // Solo relación con Owner
                entity.HasOne(e => e.Owner)
                    .WithMany()
                    .HasForeignKey(e => e.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de FileShare
            builder.Entity<OutCom.Models.FileShare>()
                .HasOne(fs => fs.FileItem)
                .WithMany(f => f.FileShares)
                .HasForeignKey(fs => fs.FileItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OutCom.Models.FileShare>()
                .HasOne(fs => fs.SharedWithUser)
                .WithMany()
                .HasForeignKey(fs => fs.SharedWithUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OutCom.Models.FileShare>()
                .HasOne(fs => fs.SharedByUser)
                .WithMany()
                .HasForeignKey(fs => fs.SharedByUserId)
                .OnDelete(DeleteBehavior.NoAction);



            // Índices para FileShare
            builder.Entity<OutCom.Models.FileShare>()
                .HasIndex(fs => fs.SharedWithUserId);

            builder.Entity<OutCom.Models.FileShare>()
                .HasIndex(fs => fs.FileItemId);

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
            // Usuarios de prueba Prueba@123
        }
    }
}
