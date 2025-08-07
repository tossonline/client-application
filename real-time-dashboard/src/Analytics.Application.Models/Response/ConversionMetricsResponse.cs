using System;
using System.Collections.Generic;

namespace Analytics.Application.Models.Response
{
    /// <summary>
    /// Response model for conversion metrics
    /// </summary>
    public class ConversionMetricsResponse
    {
        /// <summary>
        /// Overall conversion rate (registrations/visits)
        /// </summary>
        public decimal OverallConversionRate { get; set; }

        /// <summary>
        /// Deposit conversion rate (deposits/registrations)
        /// </summary>
        public decimal DepositConversionRate { get; set; }

        /// <summary>
        /// Daily conversion rates
        /// </summary>
        public List<DailyConversionRate> DailyRates { get; set; } = new();

        /// <summary>
        /// Campaign-specific conversion rates
        /// </summary>
        public List<CampaignConversionRate> CampaignRates { get; set; } = new();
    }

    public class DailyConversionRate
    {
        public DateTime Date { get; set; }
        public int VisitCount { get; set; }
        public int RegistrationCount { get; set; }
        public int DepositCount { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal DepositRate { get; set; }
    }

    public class CampaignConversionRate
    {
        public string BannerTag { get; set; }
        public int VisitCount { get; set; }
        public int RegistrationCount { get; set; }
        public int DepositCount { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal DepositRate { get; set; }
    }
}
