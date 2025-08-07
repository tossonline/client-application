using System.Threading.Tasks;
using Analytics.Domain.Commands;

namespace Analytics.Application.Handlers
{
    /// <summary>
    /// Handler for ingesting pixel events from tracking systems
    /// </summary>
    public interface IIngestPixelEventHandler
    {
        Task Handle(IngestPixelEventCommand command);
    }
}

