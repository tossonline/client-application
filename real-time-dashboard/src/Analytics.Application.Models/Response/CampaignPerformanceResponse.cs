using System;
using System.Collections.Generic;

namespace Analytics.Application.Models.Response
{
    /// <summary>
    /// Response model for campaign performance metrics
    /// </summary>
    public class CampaignPerformanceResponse
    {
        /// <summary>
        /// Campaign identifier (banner tag)
        /// </summary>
        public string BannerTag { get; set; }

        /// <summary>
        /// Total visits for the campaign
        /// </summary>
        public int TotalVisits { get; set; }

        /// <summary>
        /// Total registrations from the campaign
        /// </summary>
        public int TotalRegistrations { get; set; }

        /// <summary>
        /// Total deposits from the campaign
        /// </summary>
        public int TotalDeposits { get; set; }

        /// <summary>
        /// Overall conversion rate
        /// </summary>
        public decimal ConversionRate { get; set; }

        /// <summary>
        /// Deposit conversion rate
        /// </summary>
        public decimal DepositRate { get; set; }

        /// <summary>
        /// Daily performance metrics
        /// </summary>
        public List<DailyPerformance> DailyMetrics { get; set; } = new();

        /// <summary>
        /// Performance comparison with other campaigns
        /// </summary>
        public CampaignComparison Comparison { get; set; }
    }

    public class DailyPerformance
    {
        public DateTime Date { get; set; }
        public int VisitCount { get; set; }
        public int RegistrationCount { get; set; }
        public int DepositCount { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal DepositRate { get; set; }
    }

    public class CampaignComparison
    {
        public decimal AverageConversionRate { get; set; }
        public decimal ConversionRatePercentile { get; set; }
        public decimal AverageDepositRate { get; set; }
        public decimal DepositRatePercentile { get; set; }
        public string Performance { get; set; } // "Above Average", "Average", "Below Average"
    }
}
