using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Application.Handlers;
using Analytics.Domain.Commands;
using Analytics.Domain.Entities;
using Analytics.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq; // Added for .Where() and .Sum()

namespace Analytics.Infrastructure.Service.Controllers
{
    /// <summary>
    /// Analytics API controller for real-time pixel event processing
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IIngestPixelEventHandler _ingestPixelEventHandler;
        private readonly IAggregateEventsHandler _aggregateEventsHandler;
        private readonly IPixelEventRepository _pixelEventRepository;
        private readonly IEventSummaryRepository _eventSummaryRepository;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IIngestPixelEventHandler ingestPixelEventHandler,
            IAggregateEventsHandler aggregateEventsHandler,
            IPixelEventRepository pixelEventRepository,
            IEventSummaryRepository eventSummaryRepository,
            ILogger<AnalyticsController> logger)
        {
            _ingestPixelEventHandler = ingestPixelEventHandler ?? throw new ArgumentNullException(nameof(ingestPixelEventHandler));
            _aggregateEventsHandler = aggregateEventsHandler ?? throw new ArgumentNullException(nameof(aggregateEventsHandler));
            _pixelEventRepository = pixelEventRepository ?? throw new ArgumentNullException(nameof(pixelEventRepository));
            _eventSummaryRepository = eventSummaryRepository ?? throw new ArgumentNullException(nameof(eventSummaryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Ingest a pixel event
        /// </summary>
        [HttpPost("pixel")]
        public async Task<IActionResult> IngestPixelEvent([FromBody] IngestPixelEventRequest request)
        {
            try
            {
                var command = new IngestPixelEventCommand
                {
                    EventType = request.EventType,
                    PlayerId = request.PlayerId,
                    BannerTag = request.BannerTag,
                    Metadata = request.Metadata,
                    SourceIp = request.SourceIp,
                    UserAgent = request.UserAgent,
                    Timestamp = request.Timestamp ?? DateTime.UtcNow
                };

                await _ingestPixelEventHandler.Handle(command);

                return Ok(new { message = "Pixel event ingested successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ingest pixel event");
                return BadRequest(new { error = "Failed to ingest pixel event", details = ex.Message });
            }
        }

        /// <summary>
        /// Aggregate events for a specific date
        /// </summary>
        [HttpPost("aggregate")]
        public async Task<IActionResult> AggregateEvents([FromBody] AggregateEventsRequest request)
        {
            try
            {
                var command = new AggregateEventsCommand
                {
                    EventDate = request.EventDate
                };

                await _aggregateEventsHandler.Handle(command);

                return Ok(new { message = "Events aggregated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to aggregate events");
                return BadRequest(new { error = "Failed to aggregate events", details = ex.Message });
            }
        }

        /// <summary>
        /// Get event summaries for a date range
        /// </summary>
        [HttpGet("summaries")]
        public async Task<IActionResult> GetEventSummaries([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var summaries = await _eventSummaryRepository.GetByDateRangeAsync(startDate, endDate);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get event summaries");
                return BadRequest(new { error = "Failed to get event summaries", details = ex.Message });
            }
        }

        /// <summary>
        /// Get pixel events for a player
        /// </summary>
        [HttpGet("player/{playerId}/events")]
        public async Task<IActionResult> GetPlayerEvents(string playerId)
        {
            try
            {
                var events = await _pixelEventRepository.GetByPlayerIdAsync(playerId);
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get player events");
                return BadRequest(new { error = "Failed to get player events", details = ex.Message });
            }
        }

        /// <summary>
        /// Get real-time metrics
        /// </summary>
        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics([FromQuery] DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Today;
                var summaries = await _eventSummaryRepository.GetByDateAsync(targetDate);

                var metrics = new
                {
                    Date = targetDate,
                    TotalVisits = summaries.Where(s => s.EventType == "visit").Sum(s => s.Count),
                    TotalRegistrations = summaries.Where(s => s.EventType == "registration").Sum(s => s.Count),
                    TotalDeposits = summaries.Where(s => s.EventType == "deposit").Sum(s => s.Count),
                    ConversionRate = summaries.Any(s => s.EventType == "visit") && summaries.Any(s => s.EventType == "registration")
                        ? (double)summaries.Where(s => s.EventType == "registration").Sum(s => s.Count) / 
                          summaries.Where(s => s.EventType == "visit").Sum(s => s.Count) * 100
                        : 0
                };

                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get metrics");
                return BadRequest(new { error = "Failed to get metrics", details = ex.Message });
            }
        }
    }

    /// <summary>
    /// Request model for ingesting pixel events
    /// </summary>
    public class IngestPixelEventRequest
    {
        public string EventType { get; set; }
        public string PlayerId { get; set; }
        public string BannerTag { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
        public string SourceIp { get; set; }
        public string UserAgent { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    /// <summary>
    /// Request model for aggregating events
    /// </summary>
    public class AggregateEventsRequest
    {
        public DateTime EventDate { get; set; }
    }
}

