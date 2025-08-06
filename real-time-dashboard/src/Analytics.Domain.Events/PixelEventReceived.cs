using System;
using System.Collections.Generic;

namespace Analytics.Domain.Events
{
    /// <summary>
    /// Event raised when a pixel event is received and validated
    /// </summary>
    public class PixelEventReceived
    {
        public Guid Id { get; set; }
        public string EventType { get; set; }
        public string PlayerId { get; set; }
        public string BannerTag { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public string SourceIp { get; set; }
        public string UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime ReceivedAt { get; set; }

        public PixelEventReceived()
        {
            Id = Guid.NewGuid();
            Metadata = new Dictionary<string, string>();
            ReceivedAt = DateTime.UtcNow;
        }
    }
} 