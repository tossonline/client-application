using System;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Commands;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Handlers
{
    /// <summary>
    /// Handler for aggregating events into summary metrics
    /// </summary>
    public class AggregateEventsHandler : IAggregateEventsHandler
    {
        private readonly IPixelEventRepository _pixelEventRepository;
        private readonly IEventSummaryRepository _eventSummaryRepository;
        private readonly ILogger<AggregateEventsHandler> _logger;

        public AggregateEventsHandler(
            IPixelEventRepository pixelEventRepository,
            IEventSummaryRepository eventSummaryRepository,
            ILogger<AggregateEventsHandler> logger)
        {
            _pixelEventRepository = pixelEventRepository ?? throw new ArgumentNullException(nameof(pixelEventRepository));
            _eventSummaryRepository = eventSummaryRepository ?? throw new ArgumentNullException(nameof(eventSummaryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(AggregateEventsCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            _logger.LogInformation("Aggregating events for date: {EventDate}", command.EventDate);

            try
            {
                // Get events for the specified date
                var events = await _pixelEventRepository.GetByDateAsync(command.EventDate);

                // Group events by type and banner tag
                var groupedEvents = events
                    .GroupBy(e => new { e.EventType, e.BannerTag })
                    .Select(g => new EventSummary
                    {
                        EventDate = command.EventDate,
                        EventType = g.Key.EventType,
                        BannerTag = g.Key.BannerTag,
                        Count = g.Count()
                    })
                    .ToList();

                // Save aggregated summaries
                foreach (var summary in groupedEvents)
                {
                    await _eventSummaryRepository.AddAsync(summary);
                }

                _logger.LogInformation("Successfully aggregated {Count} event summaries for date: {EventDate}", 
                    groupedEvents.Count, command.EventDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to aggregate events for date: {EventDate}", command.EventDate);
                throw;
            }
        }
    }
}

