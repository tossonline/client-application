using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Application.Models.Alerts;

namespace Analytics.Application.Services.Alerts
{
    /// <summary>
    /// Service interface for managing real-time alerts
    /// </summary>
    public interface IAlertService
    {
        /// <summary>
        /// Check for new alerts based on current metrics
        /// </summary>
        Task<List<Alert>> CheckForAlertsAsync();

        /// <summary>
        /// Subscribe to alert notifications
        /// </summary>
        Task SubscribeToAlertsAsync(string connectionId, AlertSubscription subscription);

        /// <summary>
        /// Unsubscribe from alert notifications
        /// </summary>
        Task UnsubscribeFromAlertsAsync(string connectionId);

        /// <summary>
        /// Acknowledge an alert
        /// </summary>
        Task AcknowledgeAlertAsync(string alertId, string userId);

        /// <summary>
        /// Get active alerts
        /// </summary>
        Task<List<Alert>> GetActiveAlertsAsync();

        /// <summary>
        /// Get alert history
        /// </summary>
        Task<List<Alert>> GetAlertHistoryAsync(AlertHistoryFilter filter);
    }
}
