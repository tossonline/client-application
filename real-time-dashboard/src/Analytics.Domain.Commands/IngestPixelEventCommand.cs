using System;
using System.Collections.Generic;

namespace Analytics.Domain.Commands
{
    /// <summary>
    /// Command to ingest a pixel event from the tracking system
    /// </summary>
    public class IngestPixelEventCommand
    {
        public string EventType { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public string BannerTag { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new();
        public string? SourceIp { get; set; }
        public string? UserAgent { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
