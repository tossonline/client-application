using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analytics.Domain.Entities;

namespace Analytics.Domain.Repositories
{
    public interface IPixelEventRepository
    {
        Task AddAsync(PixelEvent pixelEvent);
        Task<PixelEvent> GetByIdAsync(Guid id);
        Task<IEnumerable<PixelEvent>> GetByPlayerIdAsync(string playerId);
        Task<IEnumerable<PixelEvent>> GetByDateRangeAsync(DateTime from, DateTime to);
        // Add more query methods as needed
    }
}
