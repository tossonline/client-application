using System;

namespace Analytics.Domain.Abstractions
{
    /// <summary>
    /// Base abstract class for all domain events
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        /// <summary>
        /// Gets the unique identifier for the event
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the timestamp when the event occurred
        /// </summary>
        public DateTime OccurredOn { get; }

        /// <summary>
        /// Gets the version of the event
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Initializes a new instance of the DomainEvent class
        /// </summary>
        protected DomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            Version = 1;
        }

        /// <summary>
        /// Initializes a new instance of the DomainEvent class with specified values
        /// </summary>
        /// <param name="id">The event identifier</param>
        /// <param name="occurredOn">When the event occurred</param>
        /// <param name="version">The event version</param>
        protected DomainEvent(Guid id, DateTime occurredOn, int version = 1)
        {
            Id = id;
            OccurredOn = occurredOn;
            Version = version;
        }
    }
}
