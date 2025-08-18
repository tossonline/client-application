using System.Collections.Generic;

namespace Analytics.Domain.Abstractions
{
    /// <summary>
    /// Base interface for aggregate roots
    /// </summary>
    public interface IAggregateRoot
    {
        /// <summary>
        /// Gets the collection of domain events that have been raised but not yet committed
        /// </summary>
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// Clears all uncommitted domain events
        /// </summary>
        void ClearDomainEvents();

        /// <summary>
        /// Gets the version of the aggregate
        /// </summary>
        int Version { get; }
    }
}
