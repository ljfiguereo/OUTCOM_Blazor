using Microsoft.AspNetCore.Identity;
using OutCom.Data;
using OutCom.Models;

namespace OutCom.Services
{
    public class DataSeederService : IDataSeederService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuditService _auditService;
        private readonly ILogger<DataSeederService> _logger;

        public DataSeederService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAuditService auditService,
            ILogger<DataSeederService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task SeedDefaultDataAsync()
        {
            try
            {
                await EnsureRolesExistAsync();
                
                // Verificar si ya existe un administrador
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                if (!adminUsers.Any())
                {
                    await CreateDefaultAdminAsync("admin@outcom.com", "Admin123!");
                }
                
                _logger.LogInformation("Inicialización de datos completada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la inicialización de datos");
                throw;
            }
        }

        public async Task<ApplicationUser> CreateDefaultAdminAsync(string email, string password)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    _logger.LogInformation("El usuario administrador ya existe: {Email}", email);
                    return existingUser;
                }

                var adminUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(adminUser, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Error al crear usuario administrador: {errors}");
                }

                // Asignar rol de administrador
                var roleResult = await _userManager.AddToRoleAsync(adminUser, "Admin");
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Error al asignar rol de administrador: {errors}");
                }

                // Log de auditoría
                await _auditService.LogAsync(
                    AuditAction.UserCreated,
                    "SYSTEM",
                    "Usuario administrador por defecto creado durante inicialización",
                    true,
                    "127.0.0.1",
                    "System Initialization"
                );

                await _auditService.LogAsync(
                    AuditAction.RoleAssigned,
                    "SYSTEM",
                    "Rol de administrador asignado durante inicialización",
                    true,
                    "127.0.0.1",
                    "System Initialization"
                );

                _logger.LogInformation("Usuario administrador por defecto creado exitosamente: {Email}", email);
                return adminUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario administrador por defecto");
                throw;
            }
        }

        public async Task EnsureRolesExistAsync()
        {
            try
            {
                var roles = new[] { "Admin", "Cliente" };

                foreach (var roleName in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        var role = new IdentityRole(roleName);
                        var result = await _roleManager.CreateAsync(role);
                        
                        if (result.Succeeded)
                        {
                            _logger.LogInformation("Rol creado exitosamente: {RoleName}", roleName);
                            
                            // Log de auditoría
                            await _auditService.LogAsync(
                                AuditAction.RoleCreated,
                                "SYSTEM",
                                $"Rol '{roleName}' creado durante inicialización del sistema",
                                true,
                                "127.0.0.1",
                                "System Initialization"
                            );
                        }
                        else
                        {
                            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                            _logger.LogError("Error al crear rol {RoleName}: {Errors}", roleName, errors);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("El rol ya existe: {RoleName}", roleName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear roles por defecto");
                throw;
            }
        }
    }
}