using System;
using System.Threading;
using System.Threading.Tasks;
using Analytics.Application.Services.RealTime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.BackgroundJobs
{
    /// <summary>
    /// Background service that periodically updates real-time metrics
    /// </summary>
    public class RealTimeMetricsUpdateJob : BackgroundService
    {
        private readonly ILogger<RealTimeMetricsUpdateJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _updateInterval;

        public RealTimeMetricsUpdateJob(
            ILogger<RealTimeMetricsUpdateJob> logger,
            IServiceProvider serviceProvider,
            TimeSpan updateInterval)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _updateInterval = updateInterval;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Real-time metrics update job is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await UpdateMetricsAsync(stoppingToken);
                    await Task.Delay(_updateInterval, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Normal shutdown
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating real-time metrics");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Back off on error
                }
            }

            _logger.LogInformation("Real-time metrics update job is stopping");
        }

        private async Task UpdateMetricsAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var metricsService = scope.ServiceProvider.GetRequiredService<IRealTimeMetricsService>();

            _logger.LogDebug("Updating real-time metrics");
            await metricsService.NotifySubscribersAsync();
            _logger.LogDebug("Real-time metrics update completed");
        }
    }
}
