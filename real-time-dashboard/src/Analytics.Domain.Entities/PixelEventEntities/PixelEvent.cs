using System;
using System.Collections.Generic;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents a pixel event from tracking systems
    /// </summary>
    public class PixelEvent
    {
        public int Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public string BannerTag { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new();
        public string? SourceIp { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
