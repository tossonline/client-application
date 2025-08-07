using System;
using System.Collections.Generic;

namespace Analytics.Application.Models.Alerts
{
    /// <summary>
    /// Alert model for real-time notifications
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// Unique identifier for the alert
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Alert type
        /// </summary>
        public AlertType Type { get; set; }

        /// <summary>
        /// Alert severity level
        /// </summary>
        public AlertSeverity Severity { get; set; }

        /// <summary>
        /// Alert title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Alert message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Alert timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Alert source (e.g., campaign ID, metric name)
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Alert metadata
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new();

        /// <summary>
        /// Alert status
        /// </summary>
        public AlertStatus Status { get; set; }

        /// <summary>
        /// Acknowledgment details
        /// </summary>
        public AlertAcknowledgment Acknowledgment { get; set; }

        /// <summary>
        /// Suggested actions
        /// </summary>
        public List<string> SuggestedActions { get; set; } = new();
    }

    public enum AlertType
    {
        ConversionRate,
        DepositRate,
        TrafficSpike,
        TrafficDrop,
        AnomalyDetected,
        PerformanceIssue,
        CampaignAlert,
        SystemAlert
    }

    public enum AlertSeverity
    {
        Info,
        Warning,
        Critical
    }

    public enum AlertStatus
    {
        Active,
        Acknowledged,
        Resolved,
        Expired
    }

    public class AlertAcknowledgment
    {
        public string UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Comment { get; set; }
    }

    public class AlertSubscription
    {
        public List<AlertType> Types { get; set; } = new();
        public AlertSeverity MinimumSeverity { get; set; }
        public List<string> Sources { get; set; } = new();
        public Dictionary<string, string> Filters { get; set; } = new();
    }

    public class AlertHistoryFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<AlertType> Types { get; set; }
        public List<AlertSeverity> Severities { get; set; }
        public List<AlertStatus> Statuses { get; set; }
        public List<string> Sources { get; set; }
        public int? Limit { get; set; }
    }
}
