using System;
using System.Threading.Tasks;
using Analytics.Application.Services.Metrics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Analytics.Infrastructure.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private readonly IMetricsCalculationService _metricsService;
        private readonly ILogger<MetricsController> _logger;

        public MetricsController(
            IMetricsCalculationService metricsService,
            ILogger<MetricsController> logger)
        {
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get conversion rates for a date range
        /// </summary>
        [HttpGet("conversion-rates")]
        public async Task<IActionResult> GetConversionRates(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.Date.AddDays(-30);
                var end = endDate ?? DateTime.UtcNow.Date;

                if (start > end)
                    return BadRequest("Start date must be before end date");

                var metrics = await _metricsService.CalculateConversionRatesAsync(start, end);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating conversion rates");
                return StatusCode(500, "An error occurred while calculating conversion rates");
            }
        }

        /// <summary>
        /// Get campaign performance metrics
        /// </summary>
        [HttpGet("campaigns/{bannerTag}")]
        public async Task<IActionResult> GetCampaignPerformance(
            string bannerTag,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bannerTag))
                    return BadRequest("Banner tag is required");

                var start = startDate ?? DateTime.UtcNow.Date.AddDays(-30);
                var end = endDate ?? DateTime.UtcNow.Date;

                if (start > end)
                    return BadRequest("Start date must be before end date");

                var performance = await _metricsService.CalculateCampaignPerformanceAsync(bannerTag, start, end);
                return Ok(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating campaign performance for banner {BannerTag}", bannerTag);
                return StatusCode(500, "An error occurred while calculating campaign performance");
            }
        }

        /// <summary>
        /// Get trend analysis for an event type
        /// </summary>
        [HttpGet("trends/{eventType}")]
        public async Task<IActionResult> GetTrendAnalysis(
            string eventType,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eventType))
                    return BadRequest("Event type is required");

                var start = startDate ?? DateTime.UtcNow.Date.AddDays(-90);
                var end = endDate ?? DateTime.UtcNow.Date;

                if (start > end)
                    return BadRequest("Start date must be before end date");

                var trends = await _metricsService.CalculateTrendMetricsAsync(eventType, start, end);
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating trend analysis for event type {EventType}", eventType);
                return StatusCode(500, "An error occurred while calculating trend analysis");
            }
        }
    }
}
