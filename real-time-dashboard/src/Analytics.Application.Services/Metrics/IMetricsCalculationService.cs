using System;
using System.Threading.Tasks;
using Analytics.Application.Models.Response;

namespace Analytics.Application.Services.Metrics
{
    /// <summary>
    /// Service interface for calculating various analytics metrics
    /// </summary>
    public interface IMetricsCalculationService
    {
        /// <summary>
        /// Calculate conversion rates for a specific date range
        /// </summary>
        Task<ConversionMetricsResponse> CalculateConversionRatesAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Calculate campaign performance metrics
        /// </summary>
        Task<CampaignPerformanceResponse> CalculateCampaignPerformanceAsync(string bannerTag, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Calculate trend metrics for a specific event type
        /// </summary>
        Task<TrendMetricsResponse> CalculateTrendMetricsAsync(string eventType, DateTime startDate, DateTime endDate);
    }
}
