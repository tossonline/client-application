using System;
using System.Threading.Tasks;
using Analytics.Domain.Commands;
using Analytics.Domain.Entities;
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
                // Create pixel event
                var pixelEvent = new PixelEvent
                {
                    EventType = command.EventType,
                    PlayerId = command.PlayerId,
                    BannerTag = command.BannerTag,
                    Metadata = command.Metadata,
                    SourceIp = command.SourceIp,
                    UserAgent = command.UserAgent,
                    Timestamp = command.Timestamp ?? DateTime.UtcNow
                };

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
                
                if (command.EventType.Equals("registration", StringComparison.OrdinalIgnoreCase))
                {
                    player.Register();
                }
                else if (command.EventType.Equals("deposit", StringComparison.OrdinalIgnoreCase))
                {
                    player.Deposit();
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

