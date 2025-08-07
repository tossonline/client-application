using System;
using System.Threading.Tasks;

namespace Analytics.Domain.Abstractions
{
    /// <summary>
    /// Messaging service interface for publishing events
    /// </summary>
    public interface IMessagingService
    {
        /// <summary>
        /// Publish a message to a topic
        /// </summary>
        Task PublishAsync<T>(string topic, T message) where T : class;

        /// <summary>
        /// Subscribe to a topic
        /// </summary>
        Task SubscribeAsync<T>(string topic, Func<T, Task> handler) where T : class;

        /// <summary>
        /// Unsubscribe from a topic
        /// </summary>
        Task UnsubscribeAsync<T>(string topic, Func<T, Task> handler) where T : class;
    }
}

