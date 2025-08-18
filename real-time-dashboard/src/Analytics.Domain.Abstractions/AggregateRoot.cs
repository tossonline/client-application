using System;
using System.Collections.Generic;

namespace Analytics.Domain.Abstractions
{
    /// <summary>
    /// Base abstract class for aggregate roots
    /// </summary>
    public abstract class AggregateRoot : IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        /// Gets the collection of domain events that have been raised but not yet committed
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Gets the version of the aggregate
        /// </summary>
        public int Version { get; protected set; }

        /// <summary>
        /// Adds a domain event to the aggregate
        /// </summary>
        /// <param name="domainEvent">The domain event to add</param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Clears all uncommitted domain events
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        /// <summary>
        /// Increments the version of the aggregate
        /// </summary>
        protected void IncrementVersion()
        {
            Version++;
        }
    }
}
