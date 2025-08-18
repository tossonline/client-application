using System;
using System.Threading.Tasks;
using Analytics.Domain.Commands;
using Analytics.Domain.Services.Events;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Handlers
{
    /// <summary>
    /// Handler for aggregating events into summary metrics
    /// </summary>
    public class AggregateEventsHandler : IAggregateEventsHandler
    {
        private readonly IEventAggregationService _eventAggregationService;
        private readonly ILogger<AggregateEventsHandler> _logger;

        public AggregateEventsHandler(
            IEventAggregationService eventAggregationService,
            ILogger<AggregateEventsHandler> logger)
        {
            _eventAggregationService = eventAggregationService ?? throw new ArgumentNullException(nameof(eventAggregationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AggregateEventsCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            _logger.LogInformation("Starting event aggregation for date: {EventDate}", command.EventDate);

            try
            {
                // Use the domain service to aggregate events
                var summaries = await _eventAggregationService.AggregateEventsForDateAsync(command.EventDate);

                _logger.LogInformation("Successfully completed event aggregation for date: {EventDate}. Generated {SummaryCount} summaries", 
                    command.EventDate, summaries.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to aggregate events for date: {EventDate}", command.EventDate);
                throw;
            }
        }
    }
}

