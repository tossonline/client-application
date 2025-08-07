using System;
using System.Threading.Tasks;
using Analytics.Application.Services.RealTime;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Analytics.Infrastructure.Service.Hubs
{
    /// <summary>
    /// SignalR hub for real-time analytics updates
    /// </summary>
    public class AnalyticsHub : Hub
    {
        private readonly IRealTimeMetricsService _metricsService;
        private readonly ILogger<AnalyticsHub> _logger;

        public AnalyticsHub(
            IRealTimeMetricsService metricsService,
            ILogger<AnalyticsHub> logger)
        {
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Subscribe to real-time metrics updates
        /// </summary>
        public async Task SubscribeToMetrics()
        {
            try
            {
                await _metricsService.SubscribeToUpdatesAsync(
                    Context.ConnectionId,
                    async metrics => await Clients.Caller.SendAsync("MetricsUpdated", metrics));

                _logger.LogInformation("Client {ConnectionId} subscribed to metrics updates", Context.ConnectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing client {ConnectionId} to metrics updates", Context.ConnectionId);
                throw;
            }
        }

        /// <summary>
        /// Subscribe to campaign-specific metrics updates
        /// </summary>
        public async Task SubscribeToCampaign(string bannerTag)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"campaign_{bannerTag}");
                _logger.LogInformation("Client {ConnectionId} subscribed to campaign {BannerTag} updates", Context.ConnectionId, bannerTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing client {ConnectionId} to campaign {BannerTag} updates", Context.ConnectionId, bannerTag);
                throw;
            }
        }

        /// <summary>
        /// Unsubscribe from campaign-specific metrics updates
        /// </summary>
        public async Task UnsubscribeFromCampaign(string bannerTag)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"campaign_{bannerTag}");
                _logger.LogInformation("Client {ConnectionId} unsubscribed from campaign {BannerTag} updates", Context.ConnectionId, bannerTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing client {ConnectionId} from campaign {BannerTag} updates", Context.ConnectionId, bannerTag);
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                await _metricsService.UnsubscribeFromUpdatesAsync(Context.ConnectionId);
                _logger.LogInformation("Client {ConnectionId} disconnected", Context.ConnectionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling disconnect for client {ConnectionId}", Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
