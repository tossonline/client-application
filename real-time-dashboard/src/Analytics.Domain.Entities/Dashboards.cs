using System;
using Analytics.Domain.Abstractions;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Dashboard entity for analytics visualization
    /// </summary>
    public class Dashboards
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}