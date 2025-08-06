using System;
using System.Collections.Generic;

namespace Analytics.Domain.Commands
{
    /// <summary>
    /// Command to ingest a pixel event from the tracking system
    /// </summary>
    public class IngestPixelEventCommand
    {
        public string EventType { get; set; }
        public string PlayerId { get; set; }
        public string BannerTag { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
        public string SourceIp { get; set; }
        public string UserAgent { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public IngestPixelEventCommand()
        {
        }

        public IngestPixelEventCommand(string eventType, string playerId, string bannerTag)
        {
            EventType = eventType;
            PlayerId = playerId;
            BannerTag = bannerTag;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EventType) &&
                   !string.IsNullOrWhiteSpace(PlayerId) &&
                   !string.IsNullOrWhiteSpace(BannerTag) &&
                   Timestamp <= DateTime.UtcNow.AddMinutes(5) && // Allow 5 minute clock skew
                   Timestamp >= DateTime.UtcNow.AddDays(-1); // Don't accept events older than 1 day
        }
    }
}
