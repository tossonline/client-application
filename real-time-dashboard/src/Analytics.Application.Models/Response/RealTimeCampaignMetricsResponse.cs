using System;
using System.Collections.Generic;

namespace Analytics.Application.Models.Response
{
    /// <summary>
    /// Response model for real-time campaign metrics
    /// </summary>
    public class RealTimeCampaignMetricsResponse
    {
        /// <summary>
        /// Campaign identifier (banner tag)
        /// </summary>
        public string BannerTag { get; set; }

        /// <summary>
        /// Current timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Today's campaign metrics
        /// </summary>
        public CampaignDayMetrics TodayMetrics { get; set; }

        /// <summary>
        /// Hourly breakdown for today
        /// </summary>
        public List<CampaignHourMetrics> HourlyBreakdown { get; set; } = new();

        /// <summary>
        /// Recent events from this campaign
        /// </summary>
        public List<CampaignEvent> RecentEvents { get; set; } = new();

        /// <summary>
        /// Real-time performance indicators
        /// </summary>
        public RealTimePerformance Performance { get; set; }
    }

    public class CampaignDayMetrics
    {
        public int VisitCount { get; set; }
        public int RegistrationCount { get; set; }
        public int DepositCount { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal DepositRate { get; set; }
        public decimal AverageTimeToRegistration { get; set; }
        public decimal AverageTimeToDeposit { get; set; }
    }

    public class CampaignHourMetrics
    {
        public int Hour { get; set; }
        public int VisitCount { get; set; }
        public int RegistrationCount { get; set; }
        public int DepositCount { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal DepositRate { get; set; }
    }

    public class CampaignEvent
    {
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public string PlayerId { get; set; }
        public string SourceIp { get; set; }
        public string UserAgent { get; set; }
    }

    public class RealTimePerformance
    {
        public string Status { get; set; } // "Performing Well", "Needs Attention", "Critical"
        public List<string> Alerts { get; set; } = new();
        public Dictionary<string, decimal> Metrics { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }
}
