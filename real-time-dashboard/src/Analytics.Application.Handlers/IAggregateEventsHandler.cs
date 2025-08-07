using System.Threading.Tasks;
using Analytics.Domain.Commands;

namespace Analytics.Application.Handlers
{
    /// <summary>
    /// Handler for aggregating events into summary metrics
    /// </summary>
    public interface IAggregateEventsHandler
    {
        Task Handle(AggregateEventsCommand command);
    }
}

