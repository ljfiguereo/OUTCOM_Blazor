using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using OutCom.Data;
using OutCom.Models;

namespace OutCom.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _auditService;

        public DashboardService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuditService auditService)
        {
            _context = context;
            _userManager = userManager;
            _auditService = auditService;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var stats = new DashboardStats
            {
                UserStats = await GetUserStatsAsync(),
                FileStats = await GetFileStatsAsync(),
                ActivityStats = await GetActivityStatsAsync(),
                SystemPerformance = await GetSystemPerformanceAsync(),
                SharedLinkStats = await GetSharedLinkStatsAsync()
            };

            return stats;
        }

        public async Task<UserStats> GetUserStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var allUsers = await _userManager.Users.ToListAsync();
            var activeUsers = allUsers.Where(u => u.IsActive).ToList();
            var inactiveUsers = allUsers.Where(u => !u.IsActive).ToList();
            var adminUsers = allUsers.Where(u => u.UserType == UserType.Admin).ToList();
            var regularUsers = allUsers.Where(u => u.UserType == UserType.Client).ToList();

            var newUsersThisWeek = allUsers.Where(u => u.CreatedAt >= startOfWeek).Count();
            var newUsersThisMonth = allUsers.Where(u => u.CreatedAt >= startOfMonth).Count();
            var lastUserCreated = allUsers.OrderByDescending(u => u.CreatedAt).FirstOrDefault()?.CreatedAt ?? DateTime.MinValue;

            return new UserStats
            {
                TotalUsers = allUsers.Count,
                ActiveUsers = activeUsers.Count,
                InactiveUsers = inactiveUsers.Count,
                AdminUsers = adminUsers.Count,
                RegularUsers = regularUsers.Count,
                NewUsersThisWeek = newUsersThisWeek,
                NewUsersThisMonth = newUsersThisMonth,
                LastUserCreated = lastUserCreated
            };
        }

        public async Task<FileStats> GetFileStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfDay = now.Date;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek).Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var allFiles = await _context.FileItems.Where(f => !f.IsDeleted).ToListAsync();
            var files = allFiles.Where(f => f.Type == FileItemType.File).ToList();
            var folders = allFiles.Where(f => f.Type == FileItemType.Folder).ToList();
            var deletedFiles = await _context.FileItems.Where(f => f.IsDeleted).CountAsync();

            var totalSize = files.Sum(f => f.Size);
            var totalSizeFormatted = FormatFileSize(totalSize);

            var filesUploadedToday = files.Where(f => f.CreatedAt >= startOfDay).Count();
            var filesUploadedThisWeek = files.Where(f => f.CreatedAt >= startOfWeek).Count();
            var filesUploadedThisMonth = files.Where(f => f.CreatedAt >= startOfMonth).Count();

            // Distribución por tipo de archivo
            var fileTypeDistribution = files
                .GroupBy(f => GetFileExtension(f.Name))
                .ToDictionary(g => g.Key, g => g.Count());

            // Calcular archivos compartidos y tamaño promedio
            var sharedFiles = await _context.SharedLinks.CountAsync();
            var averageFileSize = files.Count > 0 ? totalSize / files.Count : 0;

            // Top usuarios por archivos
            var topFileUsers = await GetTopFileUsersAsync(5);

            return new FileStats
            {
                TotalFiles = files.Count,
                TotalFolders = folders.Count,
                TotalSizeBytes = totalSize,
                TotalSizeFormatted = totalSizeFormatted,
                FilesUploadedToday = filesUploadedToday,
                FilesUploadedThisWeek = filesUploadedThisWeek,
                FilesUploadedThisMonth = filesUploadedThisMonth,
                DeletedFiles = deletedFiles,
                FileTypeDistribution = fileTypeDistribution,
                SharedFiles = sharedFiles,
                AverageFileSize = averageFileSize,
                TopFileUsers = topFileUsers
            };
        }

        public async Task<ActivityStats> GetActivityStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfDay = now.Date;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek).Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var allLogs = await _context.AuditLogs.ToListAsync();
            var totalAuditLogs = allLogs.Count;

            var loginsToday = allLogs.Where(l => l.Action == AuditAction.Login && l.Timestamp >= startOfDay).Count();
            var loginsThisWeek = allLogs.Where(l => l.Action == AuditAction.Login && l.Timestamp >= startOfWeek).Count();
            var loginsThisMonth = allLogs.Where(l => l.Action == AuditAction.Login && l.Timestamp >= startOfMonth).Count();

            var failedLoginsToday = allLogs.Where(l => l.Action == AuditAction.Login && !l.IsSuccessful && l.Timestamp >= startOfDay).Count();

            var adminActions = new[] { AuditAction.UserCreated, AuditAction.UserUpdated, AuditAction.UserDeactivated, AuditAction.UserActivated, AuditAction.RoleAssigned, AuditAction.RoleRemoved };
            var adminActionsToday = allLogs.Where(l => adminActions.Contains(l.Action) && l.Timestamp >= startOfDay).Count();
            var adminActionsThisWeek = allLogs.Where(l => adminActions.Contains(l.Action) && l.Timestamp >= startOfWeek).Count();

            var recentActivities = await GetRecentActivitiesAsync(10);

            var actionDistribution = allLogs
                .GroupBy(l => l.Action)
                .ToDictionary(g => g.Key, g => g.Count());

            return new ActivityStats
            {
                TotalAuditLogs = totalAuditLogs,
                LoginsToday = loginsToday,
                LoginsThisWeek = loginsThisWeek,
                LoginsThisMonth = loginsThisMonth,
                FailedLoginsToday = failedLoginsToday,
                AdminActionsToday = adminActionsToday,
                AdminActionsThisWeek = adminActionsThisWeek,
                RecentActivities = recentActivities,
                ActionDistribution = actionDistribution
            };
        }

        public async Task<SystemPerformance> GetSystemPerformanceAsync()
        {
            var now = DateTime.UtcNow;
            var thirtyDaysAgo = now.AddDays(-30);
            var sevenDaysAgo = now.AddDays(-7);

            var recentFiles = await _context.FileItems
                .Where(f => f.Type == FileItemType.File && !f.IsDeleted && f.CreatedAt >= thirtyDaysAgo)
                .ToListAsync();

            var recentLogins = await _context.AuditLogs
                .Where(l => l.Action == AuditAction.Login && l.Timestamp >= thirtyDaysAgo)
                .ToListAsync();

            var averageFilesPerDay = recentFiles.Count / 30.0;
            var averageFilesPerWeek = recentFiles.Where(f => f.CreatedAt >= sevenDaysAgo).Count() / 7.0 * 7;
            var averageFilesPerMonth = recentFiles.Count;
            var averageLoginsPerDay = recentLogins.Count / 30.0;

            // Calcular crecimiento de almacenamiento
            var oldSize = await _context.FileItems
                .Where(f => f.Type == FileItemType.File && !f.IsDeleted && f.CreatedAt < thirtyDaysAgo)
                .SumAsync(f => f.Size);
            var newSize = recentFiles.Sum(f => f.Size);
            var storageGrowthPercentage = oldSize > 0 ? (newSize / (double)oldSize) * 100 : 0;

            var dailyMetrics = await GetDailyMetricsAsync(sevenDaysAgo, now);

            return new SystemPerformance
            {
                AverageFilesPerDay = averageFilesPerDay,
                AverageFilesPerWeek = averageFilesPerWeek,
                AverageFilesPerMonth = averageFilesPerMonth,
                AverageLoginsPerDay = averageLoginsPerDay,
                StorageGrowthPercentage = storageGrowthPercentage,
                PeakUsersOnline = 0, // Esto requeriría tracking de sesiones activas
                PeakUsageDate = DateTime.UtcNow,
                FilesUploadedToday = recentFiles.Where(f => f.CreatedAt >= now.Date).Count(),
                FilesUploadedThisWeek = recentFiles.Where(f => f.CreatedAt >= sevenDaysAgo).Count(),
                FilesUploadedThisMonth = recentFiles.Count,
                DailyMetrics = dailyMetrics
            };
        }

        public async Task<SharedLinkStats> GetSharedLinkStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfWeek = now.AddDays(7); // Próximos 7 días
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var allLinks = await _context.SharedLinks.ToListAsync();
            var activeLinks = allLinks.Where(l => l.ExpirationDate > now).ToList();
            var expiredLinks = allLinks.Where(l => l.ExpirationDate <= now).ToList();
            var linksExpiringThisWeek = allLinks.Where(l => l.ExpirationDate > now && l.ExpirationDate <= startOfWeek).ToList();
            var linksCreatedThisMonth = allLinks.Where(l => l.ExpirationDate >= startOfMonth).ToList();

            var linksExpiringSoon = await GetLinksExpiringSoonAsync(7);

            // Archivos más compartidos (simulado, ya que no tenemos tracking de accesos)
            var mostSharedFiles = allLinks
                .GroupBy(l => l.FileName)
                .Select(g => new PopularSharedFile
                {
                    FileName = g.Key,
                    OwnerEmail = "N/A", // SharedLink no tiene OwnerEmail directamente
                    ShareCount = g.Count(),
                    LastShared = g.Max(l => l.ExpirationDate) ?? DateTime.UtcNow
                })
                .OrderByDescending(f => f.ShareCount)
                .Take(5)
                .ToList();

            return new SharedLinkStats
            {
                TotalSharedLinks = allLinks.Count,
                ActiveSharedLinks = activeLinks.Count,
                TotalActiveLinks = activeLinks.Count,
                ExpiredSharedLinks = expiredLinks.Count,
                LinksExpiringThisWeek = linksExpiringThisWeek.Count,
                LinksCreatedThisMonth = linksCreatedThisMonth.Count,
                LinksExpiringSoon = linksExpiringSoon,
                MostSharedFiles = mostSharedFiles
            };
        }

        public async Task<List<RecentActivity>> GetRecentActivitiesAsync(int count = 10)
        {
            var recentLogs = await _context.AuditLogs
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToListAsync();

            // --- PASO 2: Extraer los IDs de usuario únicos de los logs obtenidos ---
            var userIds = recentLogs.Select(l => l.UserId).Distinct().ToList();

            // --- PASO 3: Obtener los usuarios correspondientes desde Identity ---
            // Usamos un diccionario para que la búsqueda sea instantánea (mucho más rápido que un bucle)
            var usersDictionary = await _userManager.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName); // O cualquier otro campo que necesites

            return recentLogs.Select(log => new RecentActivity
            {
                UserEmail = log.UserEmail,
                //UserName = "N/A", // AuditLog no tiene UserName directamente
                Action = log.Action,
                Description = log.Description,
                Timestamp = log.Timestamp,
                IsSuccessful = log.IsSuccessful,
                IpAddress = log.IpAddress,
                // Si no lo encuentra (ej. usuario eliminado), ponemos un valor por defecto.
                UserName = usersDictionary.ContainsKey(log.UserId)? usersDictionary[log.UserId]: "N/A"
            }).ToList();
        }

        public async Task<List<DailyMetric>> GetDailyMetricsAsync(DateTime fromDate, DateTime toDate)
        {
            var metrics = new List<DailyMetric>();
            var currentDate = fromDate.Date;

            while (currentDate <= toDate.Date)
            {
                var nextDate = currentDate.AddDays(1);

                var fileUploads = await _context.FileItems
                    .Where(f => f.Type == FileItemType.File && f.CreatedAt >= currentDate && f.CreatedAt < nextDate)
                    .CountAsync();

                var userLogins = await _context.AuditLogs
                    .Where(l => l.Action == AuditAction.Login && l.Timestamp >= currentDate && l.Timestamp < nextDate)
                    .CountAsync();

                var adminActions = await _context.AuditLogs
                    .Where(l => (l.Action == AuditAction.UserCreated || l.Action == AuditAction.UserUpdated || 
                                l.Action == AuditAction.RoleAssigned) && l.Timestamp >= currentDate && l.Timestamp < nextDate)
                    .CountAsync();

                var storageUsed = await _context.FileItems
                    .Where(f => f.Type == FileItemType.File && !f.IsDeleted && f.CreatedAt < nextDate)
                    .SumAsync(f => f.Size);

                metrics.Add(new DailyMetric
                {
                    Date = currentDate,
                    FileUploads = fileUploads,
                    UserLogins = userLogins,
                    AdminActions = adminActions,
                    StorageUsed = storageUsed
                });

                currentDate = nextDate;
            }

            return metrics;
        }

        public async Task<List<TopFileUser>> GetTopFileUsersAsync(int count = 5)
        {
            var userFiles = await _context.FileItems
                .Where(f => f.Type == FileItemType.File && !f.IsDeleted)
                .Include(f => f.Owner)
                .GroupBy(f => f.OwnerId)
                .Select(g => new
                {
                    UserId = g.Key,
                    User = g.First().Owner,
                    FileCount = g.Count(),
                    TotalSize = g.Sum(f => f.Size)
                })
                .OrderByDescending(x => x.FileCount)
                .Take(count)
                .ToListAsync();

            return userFiles.Select(uf => new TopFileUser
            {
                UserId = uf.UserId,
                UserName = $"{uf.User?.FirstName} {uf.User?.LastName}".Trim(),
                UserEmail = uf.User?.Email ?? "",
                FileCount = uf.FileCount,
                TotalSizeBytes = uf.TotalSize,
                TotalSizeFormatted = FormatFileSize(uf.TotalSize)
            }).ToList();
        }

        public async Task<List<ExpiringLink>> GetLinksExpiringSoonAsync(int daysAhead = 7)
        {
            var now = DateTime.UtcNow;
            var futureDate = now.AddDays(daysAhead);

            var expiringLinks = await _context.SharedLinks
                .Where(l => l.ExpirationDate > now && l.ExpirationDate <= futureDate)
                .OrderBy(l => l.ExpirationDate)
                .ToListAsync();

            return expiringLinks.Select(l => new ExpiringLink
            {
                FileName = l.FileName,
                OwnerEmail = "N/A", // SharedLink no tiene OwnerEmail directamente
                OwnerUserName = "N/A", // SharedLink no tiene OwnerUserName directamente
                ExpirationDate = l.ExpirationDate ?? DateTime.UtcNow,
                DaysUntilExpiration = l.ExpirationDate.HasValue ? (int)(l.ExpirationDate.Value - now).TotalDays : 0
            }).ToList();
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private static string GetFileExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            return string.IsNullOrEmpty(extension) ? "Sin extensión" : extension;
        }
    }
}