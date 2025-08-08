using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Services.Metrics
{
    /// <summary>
    /// Service interface for advanced metrics calculations
    /// </summary>
    public interface IMetricsCalculationService
    {
        /// <summary>
        /// Calculate funnel metrics for a date range
        /// </summary>
        Task<FunnelMetrics> CalculateFunnelMetricsAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Calculate cohort metrics for a specific cohort
        /// </summary>
        Task<CohortMetrics> CalculateCohortMetricsAsync(DateTime cohortDate, int daysToAnalyze);

        /// <summary>
        /// Calculate retention metrics
        /// </summary>
        Task<RetentionMetrics> CalculateRetentionMetricsAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Calculate campaign ROI metrics
        /// </summary>
        Task<CampaignROIMetrics> CalculateCampaignROIAsync(string campaignId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Calculate player lifetime value
        /// </summary>
        Task<PlayerLTVMetrics> CalculatePlayerLTVAsync(string playerId);
    }

    public class FunnelMetrics
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalVisits { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalDeposits { get; set; }
        public decimal VisitToRegistrationRate { get; set; }
        public decimal RegistrationToDepositRate { get; set; }
        public decimal OverallConversionRate { get; set; }
        public Dictionary<string, FunnelStep> Steps { get; set; } = new();
        public List<FunnelBreakdown> Breakdowns { get; set; } = new();
    }

    public class FunnelStep
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal DropoffRate { get; set; }
        public decimal ConversionRate { get; set; }
        public TimeSpan AverageTimeToNextStep { get; set; }
    }

    public class FunnelBreakdown
    {
        public string Dimension { get; set; }
        public string Value { get; set; }
        public decimal ConversionRate { get; set; }
        public int Volume { get; set; }
    }

    public class CohortMetrics
    {
        public DateTime CohortDate { get; set; }
        public int CohortSize { get; set; }
        public List<CohortPeriod> Periods { get; set; } = new();
        public Dictionary<string, decimal> Metrics { get; set; } = new();
    }

    public class CohortPeriod
    {
        public int DayNumber { get; set; }
        public int ActiveUsers { get; set; }
        public decimal RetentionRate { get; set; }
        public decimal AverageValue { get; set; }
    }

    public class RetentionMetrics
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DayOneRetention { get; set; }
        public decimal DaySevenRetention { get; set; }
        public decimal DayThirtyRetention { get; set; }
        public List<RetentionDay> DailyRetention { get; set; } = new();
        public Dictionary<string, decimal> SegmentRetention { get; set; } = new();
    }

    public class RetentionDay
    {
        public int DayNumber { get; set; }
        public int RetainedUsers { get; set; }
        public decimal RetentionRate { get; set; }
        public decimal ChurnRate { get; set; }
    }

    public class CampaignROIMetrics
    {
        public string CampaignId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal ROI { get; set; }
        public decimal CostPerAcquisition { get; set; }
        public decimal CustomerLifetimeValue { get; set; }
        public Dictionary<string, decimal> Metrics { get; set; } = new();
    }

    public class PlayerLTVMetrics
    {
        public string PlayerId { get; set; }
        public int DaysSinceRegistration { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal PredictedValue { get; set; }
        public decimal ChurnProbability { get; set; }
        public List<ValuePeriod> ValueOverTime { get; set; } = new();
        public Dictionary<string, decimal> Indicators { get; set; } = new();
    }

    public class ValuePeriod
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public decimal CumulativeValue { get; set; }
    }
}
