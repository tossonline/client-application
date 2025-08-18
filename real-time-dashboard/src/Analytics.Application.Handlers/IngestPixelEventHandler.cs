using System;
using System.Threading.Tasks;
using Analytics.Domain.Commands;
using Analytics.Domain.Entities;
using Analytics.Domain.Entities.Common;
using Analytics.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Handlers
{
    /// <summary>
    /// Handler for ingesting pixel events from tracking systems
    /// </summary>
    public class IngestPixelEventHandler : IIngestPixelEventHandler
    {
        private readonly IPixelEventRepository _pixelEventRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ILogger<IngestPixelEventHandler> _logger;

        public IngestPixelEventHandler(
            IPixelEventRepository pixelEventRepository,
            IPlayerRepository playerRepository,
            ILogger<IngestPixelEventHandler> logger)
        {
            _pixelEventRepository = pixelEventRepository ?? throw new ArgumentNullException(nameof(pixelEventRepository));
            _playerRepository = playerRepository ?? throw new ArgumentNullException(nameof(playerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(IngestPixelEventCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            _logger.LogInformation("Ingesting pixel event: {EventType} for player {PlayerId}", 
                command.EventType, command.PlayerId);

            try
            {
                // Validate and create value objects
                var eventType = EventType.Create(command.EventType);
                var bannerTag = BannerTag.Create(command.BannerTag);

                // Create pixel event using the factory methods
                var pixelEvent = eventType.Value switch
                {
                    "visit" => PixelEvent.CreateVisit(command.PlayerId, bannerTag.Value, command.SourceIp, command.UserAgent),
                    "registration" => PixelEvent.CreateRegistration(command.PlayerId, bannerTag.Value, command.SourceIp, command.UserAgent),
                    "deposit" => PixelEvent.CreateDeposit(command.PlayerId, bannerTag.Value, command.SourceIp, command.UserAgent),
                    _ => throw new ArgumentException($"Unsupported event type: {command.EventType}")
                };

                // Add metadata if provided
                if (command.Metadata != null)
                {
                    foreach (var kvp in command.Metadata)
                    {
                        pixelEvent.AddMetadata(kvp.Key, kvp.Value);
                    }
                }

                // Set timestamp if provided
                if (command.Timestamp.HasValue)
                {
                    // Note: This would require a method to update timestamp in PixelEvent
                    // For now, we'll use the timestamp from the command if it's valid
                    if (command.Timestamp.Value > DateTime.UtcNow.AddDays(-7)) // Basic validation
                    {
                        // In a real implementation, you might want to add a method to update the timestamp
                        // pixelEvent.UpdateTimestamp(command.Timestamp.Value);
                    }
                }

                // Save pixel event
                await _pixelEventRepository.AddAsync(pixelEvent);

                // Update player information
                var player = await _playerRepository.GetByPlayerIdAsync(command.PlayerId);
                if (player == null)
                {
                    player = Player.Create(command.PlayerId);
                    await _playerRepository.AddAsync(player);
                }

                // Update player based on event type
                player.UpdateLastEvent(pixelEvent.Timestamp);
                
                if (eventType == EventType.Registration)
                {
                    player.Register();
                }
                else if (eventType == EventType.Deposit)
                {
                    var amount = 0m;
                    if (command.Metadata?.ContainsKey("amount") == true && 
                        decimal.TryParse(command.Metadata["amount"], out var parsedAmount))
                    {
                        amount = parsedAmount;
                    }
                    player.Deposit(amount);
                }

                await _playerRepository.UpdateAsync(player);

                _logger.LogInformation("Successfully ingested pixel event: {EventType} for player {PlayerId}", 
                    command.EventType, command.PlayerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ingest pixel event: {EventType} for player {PlayerId}", 
                    command.EventType, command.PlayerId);
                throw;
            }
        }
    }
}

