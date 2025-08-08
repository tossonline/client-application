using System;
using System.Collections.Generic;

namespace Analytics.Domain.Entities
{
    /// <summary>
    /// Dashboard entity for analytics visualization.
    /// This entity represents a configurable analytics dashboard that can display
    /// various metrics, charts, and KPIs in a customizable layout.
    /// </summary>
    /// <remarks>
    /// Dashboards are the primary interface for users to view and analyze data.
    /// They support different types of visualizations through widgets and can be
    /// customized for different use cases like campaign monitoring or real-time analytics.
    /// </remarks>
    public class Dashboard
    {
        /// <summary>
        /// Gets the unique identifier for the dashboard
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the name of the dashboard
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the description of the dashboard
        /// </summary>
        public string Description { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the type of dashboard
        /// </summary>
        public DashboardType Type { get; private set; }

        /// <summary>
        /// Gets the identifier of the dashboard owner
        /// </summary>
        public string OwnerId { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the timestamp when the dashboard was created
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the timestamp when the dashboard was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Gets whether the dashboard is currently active
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Gets the layout configuration for the dashboard
        /// </summary>
        public DashboardLayout Layout { get; private set; }

        /// <summary>
        /// Gets additional configuration settings for the dashboard
        /// </summary>
        public Dictionary<string, object> Configuration { get; private set; } = new();

        /// <summary>
        /// Private constructor for EF Core
        /// </summary>
        private Dashboard() { }

        /// <summary>
        /// Creates a new dashboard
        /// </summary>
        /// <param name="name">The name of the dashboard</param>
        /// <param name="description">The description of the dashboard</param>
        /// <param name="type">The type of dashboard</param>
        /// <param name="ownerId">The identifier of the dashboard owner</param>
        /// <returns>A new Dashboard instance</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when name or ownerId is null or empty
        /// </exception>
        public static Dashboard Create(string name, string description, DashboardType type, string ownerId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));

            if (string.IsNullOrWhiteSpace(ownerId))
                throw new ArgumentException("Owner ID cannot be null or empty", nameof(ownerId));

            return new Dashboard
            {
                Name = name,
                Description = description,
                Type = type,
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Layout = new DashboardLayout()
            };
        }

        /// <summary>
        /// Updates dashboard details
        /// </summary>
        /// <param name="name">The new name for the dashboard</param>
        /// <param name="description">The new description for the dashboard</param>
        /// <exception cref="ArgumentException">Thrown when name is null or empty</exception>
        public void Update(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));

            Name = name;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Activates the dashboard
        /// </summary>
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Deactivates the dashboard
        /// </summary>
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the dashboard layout
        /// </summary>
        /// <param name="layout">The new layout configuration</param>
        /// <exception cref="ArgumentNullException">Thrown when layout is null</exception>
        public void UpdateLayout(DashboardLayout layout)
        {
            Layout = layout ?? throw new ArgumentNullException(nameof(layout));
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates a configuration setting
        /// </summary>
        /// <param name="key">The configuration key</param>
        /// <param name="value">The configuration value</param>
        /// <exception cref="ArgumentException">Thrown when key is null or empty</exception>
        public void UpdateConfiguration(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Configuration key cannot be null or empty", nameof(key));

            Configuration[key] = value;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Represents the type of dashboard
    /// </summary>
    public enum DashboardType
    {
        /// <summary>
        /// General overview dashboard showing key metrics
        /// </summary>
        Overview,

        /// <summary>
        /// Campaign-specific dashboard for monitoring performance
        /// </summary>
        Campaign,

        /// <summary>
        /// Conversion funnel and optimization dashboard
        /// </summary>
        Conversion,

        /// <summary>
        /// Real-time analytics dashboard
        /// </summary>
        RealTime,

        /// <summary>
        /// Custom dashboard with user-defined metrics
        /// </summary>
        Custom
    }

    /// <summary>
    /// Represents the layout configuration for a dashboard
    /// </summary>
    public class DashboardLayout
    {
        /// <summary>
        /// Gets or sets the list of widgets in the dashboard
        /// </summary>
        public List<DashboardWidget> Widgets { get; set; } = new();

        /// <summary>
        /// Gets or sets the CSS grid layout configuration
        /// </summary>
        public string GridLayout { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a widget within a dashboard
    /// </summary>
    public class DashboardWidget
    {
        /// <summary>
        /// Gets or sets the unique identifier for the widget
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of widget (e.g., chart, metric, table)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title of the widget
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the widget-specific configuration
        /// </summary>
        public Dictionary<string, object> Configuration { get; set; } = new();

        /// <summary>
        /// Gets or sets the position of the widget in the grid
        /// </summary>
        public GridPosition Position { get; set; } = new();
    }

    /// <summary>
    /// Represents the position of a widget in the dashboard grid
    /// </summary>
    public class GridPosition
    {
        /// <summary>
        /// Gets or sets the row number in the grid
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the column number in the grid
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the width in grid units
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height in grid units
        /// </summary>
        public int Height { get; set; }
    }
}