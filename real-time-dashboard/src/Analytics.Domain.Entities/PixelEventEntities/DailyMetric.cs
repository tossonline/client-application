using System;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents daily metrics for analytics
    /// </summary>
    public class DailyMetric
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int VisitCount { get; set; }
        public int RegistrationCount { get; set; }
        public int DepositCount { get; set; }
        public decimal ConversionRate { get; set; }
    }
}
