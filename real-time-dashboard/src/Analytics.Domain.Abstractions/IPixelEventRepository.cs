using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analytics.Domain.Abstractions
{
    public interface IPixelEventRepository
    {
        Task AddAsync(IPixelEvent pixelEvent);
        Task<IPixelEvent> GetByIdAsync(Guid id);
        Task<IEnumerable<IPixelEvent>> GetByPlayerIdAsync(string playerId);
        Task<IEnumerable<IPixelEvent>> GetByDateRangeAsync(DateTime from, DateTime to);
    }

    public interface IPixelEvent
    {
        Guid Id { get; set; }
        string EventType { get; set; }
        string PlayerId { get; set; }
        string BannerTag { get; set; }
        Dictionary<string, string> Metadata { get; set; }
        string SourceIp { get; set; }
        string UserAgent { get; set; }
        DateTime CreatedAt { get; set; }
    }
}