using System;
using System.Threading.Tasks;
using Analytics.Domain.Abstractions;
using Analytics.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Handlers.DomainEventHandlers
{
    /// <summary>
    /// Handler for PlayerRegistered domain events
    /// </summary>
    public class PlayerRegisteredHandler : IDomainEventHandler<PlayerRegistered>
    {
        private readonly ILogger<PlayerRegisteredHandler> _logger;

        public PlayerRegisteredHandler(ILogger<PlayerRegisteredHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(PlayerRegistered domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            _logger.LogInformation("Processing PlayerRegistered event for player {PlayerId} at {RegistrationDate}", 
                domainEvent.PlayerId, domainEvent.RegistrationDate);

            try
            {
                // Handle side effects of player registration
                // This could include:
                // - Sending welcome emails
                // - Creating player profiles
                // - Updating marketing lists
                // - Triggering onboarding workflows
                // - Sending notifications to support team
                // - Updating conversion metrics

                await Task.CompletedTask; // Placeholder for async operations

                _logger.LogInformation("Successfully processed PlayerRegistered event for player {PlayerId}", 
                    domainEvent.PlayerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process PlayerRegistered event for player {PlayerId}", 
                    domainEvent.PlayerId);
                throw;
            }
        }
    }
}
