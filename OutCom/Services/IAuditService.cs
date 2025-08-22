using OutCom.Models;

namespace OutCom.Services
{
    public interface IAuditService
    {
        Task LogAsync(AuditAction action, string userId, string details, bool success = true, string? ipAddress = null, string? userAgent = null);
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, string? userId = null, AuditAction? action = null);
        Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(string userId, int pageSize = 50, int pageNumber = 1);
        Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count = 100);
        Task CleanupOldLogsAsync(int daysToKeep = 90);
    }
}