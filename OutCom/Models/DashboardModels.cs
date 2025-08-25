namespace OutCom.Models
{
    public class DashboardStats
    {
        public UserStats UserStats { get; set; } = new();
        public FileStats FileStats { get; set; } = new();
        public ActivityStats ActivityStats { get; set; } = new();
        public SystemPerformance SystemPerformance { get; set; } = new();
        public SharedLinkStats SharedLinkStats { get; set; } = new();
    }

    public class UserStats
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int AdminUsers { get; set; }
        public int RegularUsers { get; set; }
        public Dictionary<string, int> UserTypeDistribution { get; set; } = new();
        public int NewUsersThisMonth { get; set; }
        public int NewUsersThisWeek { get; set; }
        public DateTime LastUserCreated { get; set; }
    }

    public class FileStats
    {
        public int TotalFiles { get; set; }
        public int TotalFolders { get; set; }
        public long TotalSizeBytes { get; set; }
        public string TotalSizeFormatted { get; set; } = string.Empty;
        public int FilesUploadedToday { get; set; }
        public int FilesUploadedThisWeek { get; set; }
        public int FilesUploadedThisMonth { get; set; }
        public int DeletedFiles { get; set; }
        public Dictionary<string, int> FileTypeDistribution { get; set; } = new();
        public int SharedFiles { get; set; }
        public long AverageFileSize { get; set; }
        public List<TopFileUser> TopFileUsers { get; set; } = new();
    }

    public class TopFileUser
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int FileCount { get; set; }
        public long TotalSizeBytes { get; set; }
        public string TotalSizeFormatted { get; set; } = string.Empty;
    }

    public class ActivityStats
    {
        public int TotalAuditLogs { get; set; }
        public int LoginsToday { get; set; }
        public int LoginsThisWeek { get; set; }
        public int LoginsThisMonth { get; set; }
        public int FailedLoginsToday { get; set; }
        public int AdminActionsToday { get; set; }
        public int AdminActionsThisWeek { get; set; }
        public List<RecentActivity> RecentActivities { get; set; } = new();
        public Dictionary<AuditAction, int> ActionDistribution { get; set; } = new();
    }

    public class RecentActivity
    {
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public AuditAction Action { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsSuccessful { get; set; }
        public string IpAddress { get; set; } = string.Empty;
    }

    public class SystemPerformance
    {
        public double AverageFilesPerDay { get; set; }
        public double AverageFilesPerWeek { get; set; }
        public double AverageFilesPerMonth { get; set; }
        public double AverageLoginsPerDay { get; set; }
        public double StorageGrowthPercentage { get; set; }
        public int PeakUsersOnline { get; set; }
        public DateTime PeakUsageDate { get; set; }
        public int FilesUploadedToday { get; set; }
        public int FilesUploadedThisWeek { get; set; }
        public int FilesUploadedThisMonth { get; set; }
        public List<DailyMetric> DailyMetrics { get; set; } = new();
    }

    public class DailyMetric
    {
        public DateTime Date { get; set; }
        public int FileUploads { get; set; }
        public int UserLogins { get; set; }
        public int AdminActions { get; set; }
        public long StorageUsed { get; set; }
    }

    public class SharedLinkStats
    {
        public int TotalSharedLinks { get; set; }
        public int ActiveSharedLinks { get; set; }
        public int TotalActiveLinks { get; set; }
        public int ExpiredSharedLinks { get; set; }
        public int LinksExpiringThisWeek { get; set; }
        public int LinksCreatedThisMonth { get; set; }
        public List<ExpiringLink> LinksExpiringSoon { get; set; } = new();
        public List<PopularSharedFile> MostSharedFiles { get; set; } = new();
    }

    public class ExpiringLink
    {
        public string FileName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public string OwnerUserName { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public int DaysUntilExpiration { get; set; }
    }

    public class PopularSharedFile
    {
        public string FileName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        public int ShareCount { get; set; }
        public DateTime LastShared { get; set; }
    }
}