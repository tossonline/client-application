using System;
using System.Threading.Tasks;
using Analytics.Application.Models.Response;

namespace Analytics.Application.Services.RealTime
{
    /// <summary>
    /// Service interface for real-time metrics calculation
    /// </summary>
    public interface IRealTimeMetricsService
    {
        /// <summary>
        /// Get real-time metrics for the current day
        /// </summary>
        Task<RealTimeMetricsResponse> GetCurrentMetricsAsync();

        /// <summary>
        /// Get real-time metrics for a specific campaign
        /// </summary>
        Task<RealTimeCampaignMetricsResponse> GetCampaignMetricsAsync(string bannerTag);

        /// <summary>
        /// Subscribe to real-time metric updates
        /// </summary>
        Task SubscribeToUpdatesAsync(string connectionId, Func<RealTimeMetricsResponse, Task> callback);

        /// <summary>
        /// Unsubscribe from real-time metric updates
        /// </summary>
        Task UnsubscribeFromUpdatesAsync(string connectionId);
    }
}
