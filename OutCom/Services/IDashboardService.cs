using OutCom.Models;

namespace OutCom.Services
{
    public interface IDashboardService
    {
        /// <summary>
        /// Obtiene todas las estadísticas del dashboard en una sola llamada
        /// </summary>
        Task<DashboardStats> GetDashboardStatsAsync();

        /// <summary>
        /// Obtiene estadísticas de usuarios del sistema
        /// </summary>
        Task<UserStats> GetUserStatsAsync();

        /// <summary>
        /// Obtiene estadísticas de archivos y almacenamiento
        /// </summary>
        Task<FileStats> GetFileStatsAsync();

        /// <summary>
        /// Obtiene estadísticas de actividad y auditoría
        /// </summary>
        Task<ActivityStats> GetActivityStatsAsync();

        /// <summary>
        /// Obtiene métricas de rendimiento del sistema
        /// </summary>
        Task<SystemPerformance> GetSystemPerformanceAsync();

        /// <summary>
        /// Obtiene estadísticas de enlaces compartidos
        /// </summary>
        Task<SharedLinkStats> GetSharedLinkStatsAsync();

        /// <summary>
        /// Obtiene actividad reciente del sistema (últimas N actividades)
        /// </summary>
        Task<List<RecentActivity>> GetRecentActivitiesAsync(int count = 10);

        /// <summary>
        /// Obtiene métricas diarias para un rango de fechas
        /// </summary>
        Task<List<DailyMetric>> GetDailyMetricsAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// Obtiene los usuarios más activos en términos de archivos
        /// </summary>
        Task<List<TopFileUser>> GetTopFileUsersAsync(int count = 5);

        /// <summary>
        /// Obtiene enlaces que expiran pronto
        /// </summary>
        Task<List<ExpiringLink>> GetLinksExpiringSoonAsync(int daysAhead = 7);
    }
}