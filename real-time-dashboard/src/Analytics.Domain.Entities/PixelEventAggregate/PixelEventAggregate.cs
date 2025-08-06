using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analytics.Domain.Entities;
using Analytics.Domain.Abstractions;
using Analytics.Domain.Events;

namespace Analytics.Domain.Entities.PixelEventAggregate
{
    /// <summary>
    /// Aggregate root for pixel events - manages the lifecycle of pixel events
    /// and coordinates with other aggregates
    /// </summary>
    public class PixelEventAggregate
    {
        private readonly List<PixelEventReceived> _domainEvents;
        private readonly IPixelEventRepository _pixelEventRepository;
        private readonly IPlayerRepository _playerRepository;

        public PixelEventAggregate(
            IPixelEventRepository pixelEventRepository,
            IPlayerRepository playerRepository)
        {
            _pixelEventRepository = pixelEventRepository;
            _playerRepository = playerRepository;
            _domainEvents = new List<PixelEventReceived>();
        }

        public async Task ProcessPixelEventAsync(PixelEvent pixelEvent)
        {
            // Validate the pixel event
            if (!IsValidPixelEvent(pixelEvent))
            {
                throw new ArgumentException("Invalid pixel event");
            }

            // Check for duplicate events (business rule: no duplicates per day per type)
            if (await IsDuplicateEventAsync(pixelEvent))
            {
                return; // Skip duplicate events
            }

            // Save the pixel event
            await _pixelEventRepository.AddAsync(pixelEvent);

            // Update player tracking
            await UpdatePlayerTrackingAsync(pixelEvent);

            // Raise domain event
            var domainEvent = new PixelEventReceived
            {
                EventType = pixelEvent.EventType,
                PlayerId = pixelEvent.PlayerId,
                BannerTag = pixelEvent.BannerTag,
                Metadata = pixelEvent.Metadata,
                SourceIp = pixelEvent.SourceIp,
                UserAgent = pixelEvent.UserAgent,
                Timestamp = pixelEvent.CreatedAt
            };

            _domainEvents.Add(domainEvent);
        }

        private bool IsValidPixelEvent(PixelEvent pixelEvent)
        {
            return !string.IsNullOrWhiteSpace(pixelEvent.EventType) &&
                   !string.IsNullOrWhiteSpace(pixelEvent.PlayerId) &&
                   !string.IsNullOrWhiteSpace(pixelEvent.BannerTag) &&
                   pixelEvent.CreatedAt <= DateTime.UtcNow.AddMinutes(5) && // Allow 5 minute clock skew
                   pixelEvent.CreatedAt >= DateTime.UtcNow.AddDays(-1); // Don't accept events older than 1 day
        }

        private async Task<bool> IsDuplicateEventAsync(PixelEvent pixelEvent)
        {
            var existingEvents = await _pixelEventRepository.GetByDateRangeAsync(
                pixelEvent.CreatedAt.Date, 
                pixelEvent.CreatedAt.Date.AddDays(1));

            return existingEvents.Any(e => 
                e.PlayerId == pixelEvent.PlayerId &&
                e.EventType == pixelEvent.EventType &&
                e.BannerTag == pixelEvent.BannerTag &&
                e.CreatedAt.Date == pixelEvent.CreatedAt.Date);
        }

        private async Task UpdatePlayerTrackingAsync(PixelEvent pixelEvent)
        {
            var player = await _playerRepository.GetByPlayerIdAsync(pixelEvent.PlayerId);
            
            if (player == null)
            {
                player = Player.Create(pixelEvent.PlayerId);
                await _playerRepository.AddAsync(player);
            }

            player.UpdateLastEvent();

            // Handle specific event types
            switch (pixelEvent.EventType.ToLower())
            {
                case "registration":
                    player.Register();
                    break;
                case "deposit":
                    if (player.RegistrationAt.HasValue)
                    {
                        player.Deposit();
                    }
                    break;
            }

            await _playerRepository.UpdateAsync(player);
        }

        public IReadOnlyList<PixelEventReceived> GetDomainEvents()
        {
            return _domainEvents.AsReadOnly();
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}