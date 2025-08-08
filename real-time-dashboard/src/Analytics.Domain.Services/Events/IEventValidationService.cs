using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Services.Events
{
    /// <summary>
    /// Service interface for validating and enriching pixel events
    /// </summary>
    public interface IEventValidationService
    {
        /// <summary>
        /// Validate and enrich a pixel event
        /// </summary>
        Task<ValidationResult> ValidateAndEnrichAsync(PixelEvent pixelEvent);

        /// <summary>
        /// Validate and enrich a batch of pixel events
        /// </summary>
        Task<IEnumerable<ValidationResult>> ValidateAndEnrichBatchAsync(IEnumerable<PixelEvent> events);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public Dictionary<string, string> EnrichedMetadata { get; set; } = new();
        public PixelEvent Event { get; set; }
    }
}
