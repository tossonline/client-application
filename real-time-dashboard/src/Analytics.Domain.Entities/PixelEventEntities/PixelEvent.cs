using System;
using System.Collections.Generic;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Represents a raw pixel event (visit, registration, deposit).
    /// </summary>
    public class PixelEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EventType { get; set; }
        public string PlayerId { get; set; }
        public string BannerTag { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
        public string SourceIp { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
