using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Analytics.Domain.Services.Events
{
    public class EventValidationService : IEventValidationService
    {
        private readonly ILogger<EventValidationService> _logger;
        private readonly Dictionary<string, Func<PixelEvent, Task<List<string>>>> _validationRules;
        private readonly Dictionary<string, Func<PixelEvent, Task<Dictionary<string, string>>>> _enrichmentRules;

        public EventValidationService(ILogger<EventValidationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _validationRules = new Dictionary<string, Func<PixelEvent, Task<List<string>>>>
            {
                { "basic", ValidateBasicRulesAsync },
                { "player", ValidatePlayerRulesAsync },
                { "campaign", ValidateCampaignRulesAsync },
                { "timing", ValidateTimingRulesAsync }
            };

            _enrichmentRules = new Dictionary<string, Func<PixelEvent, Task<Dictionary<string, string>>>>
            {
                { "device", EnrichDeviceInfoAsync },
                { "location", EnrichLocationInfoAsync },
                { "campaign", EnrichCampaignInfoAsync },
                { "timing", EnrichTimingInfoAsync }
            };
        }

        public async Task<ValidationResult> ValidateAndEnrichAsync(PixelEvent pixelEvent)
        {
            if (pixelEvent == null)
                throw new ArgumentNullException(nameof(pixelEvent));

            var result = new ValidationResult { Event = pixelEvent };

            try
            {
                // Run all validation rules
                var validationTasks = _validationRules.Values.Select(rule => rule(pixelEvent));
                var validationResults = await Task.WhenAll(validationTasks);
                result.Errors = validationResults.SelectMany(errors => errors).ToList();

                // If valid, run enrichment rules
                if (!result.Errors.Any())
                {
                    var enrichmentTasks = _enrichmentRules.Values.Select(rule => rule(pixelEvent));
                    var enrichmentResults = await Task.WhenAll(enrichmentTasks);
                    result.EnrichedMetadata = enrichmentResults
                        .SelectMany(dict => dict)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    // Add enriched metadata to the event
                    foreach (var metadata in result.EnrichedMetadata)
                    {
                        pixelEvent.AddMetadata(metadata.Key, metadata.Value);
                    }
                }

                result.IsValid = !result.Errors.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating event {EventType} for player {PlayerId}", 
                    pixelEvent.EventType, pixelEvent.PlayerId);
                result.Errors.Add($"Validation error: {ex.Message}");
                result.IsValid = false;
            }

            return result;
        }

        public async Task<IEnumerable<ValidationResult>> ValidateAndEnrichBatchAsync(IEnumerable<PixelEvent> events)
        {
            if (events == null)
                throw new ArgumentNullException(nameof(events));

            var tasks = events.Select(ValidateAndEnrichAsync);
            return await Task.WhenAll(tasks);
        }

        private async Task<List<string>> ValidateBasicRulesAsync(PixelEvent pixelEvent)
        {
            var errors = new List<string>();

            if (!pixelEvent.IsValid())
            {
                errors.Add("Event fails basic validation rules");
            }

            if (pixelEvent.Timestamp > DateTime.UtcNow)
            {
                errors.Add("Event timestamp cannot be in the future");
            }

            // Add more basic validation rules as needed
            await Task.CompletedTask; // Async context for future rules
            return errors;
        }

        private async Task<List<string>> ValidatePlayerRulesAsync(PixelEvent pixelEvent)
        {
            var errors = new List<string>();

            if (pixelEvent.EventType == "deposit" && !pixelEvent.Metadata.ContainsKey("amount"))
            {
                errors.Add("Deposit events must include amount in metadata");
            }

            if (pixelEvent.EventType == "registration" && 
                (!pixelEvent.Metadata.ContainsKey("email") || !pixelEvent.Metadata.ContainsKey("country")))
            {
                errors.Add("Registration events must include email and country in metadata");
            }

            // Add more player-specific validation rules
            await Task.CompletedTask; // Async context for future rules
            return errors;
        }

        private async Task<List<string>> ValidateCampaignRulesAsync(PixelEvent pixelEvent)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(pixelEvent.CampaignId))
            {
                errors.Add("Campaign ID could not be extracted from banner tag");
            }

            // Add more campaign-specific validation rules
            await Task.CompletedTask; // Async context for future rules
            return errors;
        }

        private async Task<List<string>> ValidateTimingRulesAsync(PixelEvent pixelEvent)
        {
            var errors = new List<string>();

            if (pixelEvent.Timestamp < DateTime.UtcNow.AddDays(-7))
            {
                errors.Add("Event is too old (more than 7 days)");
            }

            // Add more timing-specific validation rules
            await Task.CompletedTask; // Async context for future rules
            return errors;
        }

        private async Task<Dictionary<string, string>> EnrichDeviceInfoAsync(PixelEvent pixelEvent)
        {
            var metadata = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(pixelEvent.UserAgent))
            {
                // Add basic device detection
                var userAgent = pixelEvent.UserAgent.ToLower();
                metadata["device_type"] = userAgent.Contains("mobile") ? "mobile" : "desktop";
                
                // Add browser detection
                if (userAgent.Contains("chrome"))
                    metadata["browser"] = "chrome";
                else if (userAgent.Contains("firefox"))
                    metadata["browser"] = "firefox";
                else if (userAgent.Contains("safari"))
                    metadata["browser"] = "safari";
                else
                    metadata["browser"] = "other";
            }

            await Task.CompletedTask; // Async context for future enrichment
            return metadata;
        }

        private async Task<Dictionary<string, string>> EnrichLocationInfoAsync(PixelEvent pixelEvent)
        {
            var metadata = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(pixelEvent.SourceIp))
            {
                // Add basic geo info (in real implementation, use a geo-IP service)
                metadata["geo_lookup_timestamp"] = DateTime.UtcNow.ToString("O");
            }

            await Task.CompletedTask; // Async context for future enrichment
            return metadata;
        }

        private async Task<Dictionary<string, string>> EnrichCampaignInfoAsync(PixelEvent pixelEvent)
        {
            var metadata = new Dictionary<string, string>();

            // Add campaign type based on banner tag format
            var parts = pixelEvent.BannerTag.Split('-');
            if (parts.Length > 1)
            {
                metadata["campaign_type"] = parts[1];
            }

            if (parts.Length > 2)
            {
                metadata["banner_size"] = parts[2];
            }

            await Task.CompletedTask; // Async context for future enrichment
            return metadata;
        }

        private async Task<Dictionary<string, string>> EnrichTimingInfoAsync(PixelEvent pixelEvent)
        {
            var metadata = new Dictionary<string, string>();

            // Add timing context
            metadata["day_of_week"] = pixelEvent.Timestamp.DayOfWeek.ToString();
            metadata["hour_of_day"] = pixelEvent.Timestamp.Hour.ToString();
            metadata["is_weekend"] = (pixelEvent.Timestamp.DayOfWeek == DayOfWeek.Saturday || 
                                    pixelEvent.Timestamp.DayOfWeek == DayOfWeek.Sunday).ToString();

            await Task.CompletedTask; // Async context for future enrichment
            return metadata;
        }
    }
}
