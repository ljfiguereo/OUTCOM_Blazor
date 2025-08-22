using Microsoft.AspNetCore.Identity;
using OutCom.Data;
using OutCom.Models;
using OutCom.Services;
using System.Security.Claims;

namespace OutCom.Middleware
{
    public class AdminAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminAuthorizationMiddleware> _logger;

        public AdminAuthorizationMiddleware(RequestDelegate next, ILogger<AdminAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, IAuditService auditService)
        {
            // Solo aplicar middleware a rutas administrativas
            if (context.Request.Path.StartsWithSegments("/admin"))
            {
                var user = context.User;
                
                if (!user.Identity?.IsAuthenticated == true)
                {
                    _logger.LogWarning("Intento de acceso no autenticado a área administrativa desde IP: {IpAddress}", 
                        GetClientIpAddress(context));
                    
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("No autorizado");
                    return;
                }

                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = user.FindFirst(ClaimTypes.Email)?.Value;
                
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Usuario autenticado sin ID válido intentando acceder a área administrativa");
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("No autorizado");
                    return;
                }

                // Verificar si el usuario tiene rol de administrador
                var applicationUser = await userManager.FindByIdAsync(userId);
                if (applicationUser == null)
                {
                    _logger.LogWarning("Usuario no encontrado: {UserId}", userId);
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("No autorizado");
                    return;
                }

                // Verificar si el usuario está activo
                if (!applicationUser.IsActive)
                {
                    _logger.LogWarning("Usuario inactivo intentando acceder a área administrativa: {UserEmail}", userEmail);
                    
                    await auditService.LogAsync(
                        AuditAction.AdminActionPerformed,
                        userId,
                        "Intento de acceso con usuario inactivo",
                        false,
                        GetClientIpAddress(context),
                        context.Request.Headers["User-Agent"].ToString()
                    );
                    
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Cuenta desactivada");
                    return;
                }

                var isAdmin = await userManager.IsInRoleAsync(applicationUser, "Admin");
                if (!isAdmin)
                {
                    _logger.LogWarning("Usuario sin permisos administrativos intentando acceder: {UserEmail}", userEmail);
                    
                    await auditService.LogAsync(
                        AuditAction.AdminActionPerformed,
                        userId,
                        "Intento de acceso sin permisos administrativos",
                        false,
                        GetClientIpAddress(context),
                        context.Request.Headers["User-Agent"].ToString()
                    );
                    
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Permisos insuficientes");
                    return;
                }

                // Log de acceso exitoso a área administrativa
                _logger.LogInformation("Acceso administrativo autorizado para usuario: {UserEmail}", userEmail);
                
                await auditService.LogAsync(
                    AuditAction.AdminActionPerformed,
                    userId,
                    $"Acceso a área administrativa: {context.Request.Path}",
                    true,
                    GetClientIpAddress(context),
                    context.Request.Headers["User-Agent"].ToString()
                );
            }

            await _next(context);
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Connection.RemoteIpAddress?.ToString();
            }
            return ipAddress ?? "Unknown";
        }
    }

    public static class AdminAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminAuthorizationMiddleware>();
        }
    }
}