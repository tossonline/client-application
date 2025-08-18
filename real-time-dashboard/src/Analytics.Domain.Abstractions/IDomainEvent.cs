using System;

namespace Analytics.Domain.Abstractions
{
    /// <summary>
    /// Base interface for all domain events
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// Gets the unique identifier for the event
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the timestamp when the event occurred
        /// </summary>
        DateTime OccurredOn { get; }

        /// <summary>
        /// Gets the version of the event
        /// </summary>
        int Version { get; }
    }
}
