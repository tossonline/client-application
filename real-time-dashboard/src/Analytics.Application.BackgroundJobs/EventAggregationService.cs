using System;
using System.Threading;
using System.Threading.Tasks;
using Analytics.Application.Handlers;
using Analytics.Domain.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.BackgroundJobs
{
    public class EventAggregationService : BackgroundService
    {
        private readonly ILogger<EventAggregationService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _aggregationInterval;

        public EventAggregationService(
            ILogger<EventAggregationService> logger,
            IServiceProvider serviceProvider,
            TimeSpan aggregationInterval)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _aggregationInterval = aggregationInterval;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Event Aggregation Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await AggregateEventsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while aggregating events.");
                }

                await Task.Delay(_aggregationInterval, stoppingToken);
            }
        }

        private async Task AggregateEventsAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IAggregateEventsHandler>();

            var command = new AggregateEventsCommand
            {
                EventDate = DateTime.UtcNow.Date
            };

            _logger.LogInformation("Starting event aggregation for date: {Date}", command.EventDate);
            await handler.Handle(command);
            _logger.LogInformation("Completed event aggregation for date: {Date}", command.EventDate);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Event Aggregation Service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}
