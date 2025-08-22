using OutCom.Data;
using OutCom.Models;
using Microsoft.EntityFrameworkCore;

namespace OutCom.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(AuditAction action, string userId, string details, bool success = true, string? ipAddress = null, string? userAgent = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            var auditLog = new AuditLog
            {
                Action = action,
                UserId = userId,
                UserEmail = "", // Se puede obtener del contexto si es necesario
                Description = details,
                IsSuccessful = success,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress ?? httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown",
                UserAgent = userAgent ?? httpContext?.Request?.Headers["User-Agent"].ToString() ?? "Unknown"
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, string? userId = null, AuditAction? action = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(log => log.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(log => log.Timestamp <= toDate.Value);

            if (!string.IsNullOrEmpty(userId))
                query = query.Where(log => log.UserId == userId);

            if (action.HasValue)
                query = query.Where(log => log.Action == action.Value);

            return await query.OrderByDescending(log => log.Timestamp).ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(string userId, int pageSize = 50, int pageNumber = 1)
        {
            return await _context.AuditLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count = 100)
        {
            return await _context.AuditLogs
                .OrderByDescending(log => log.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task CleanupOldLogsAsync(int daysToKeep = 90)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
            var oldLogs = await _context.AuditLogs
                .Where(log => log.Timestamp < cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _context.AuditLogs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();
            }
        }
    }
}