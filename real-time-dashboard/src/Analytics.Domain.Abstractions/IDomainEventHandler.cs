using System.Threading.Tasks;

namespace Analytics.Domain.Abstractions
{
    /// <summary>
    /// Base interface for domain event handlers
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event to handle</typeparam>
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        /// <summary>
        /// Handles the domain event
        /// </summary>
        /// <param name="domainEvent">The domain event to handle</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task HandleAsync(TEvent domainEvent);
    }
}
