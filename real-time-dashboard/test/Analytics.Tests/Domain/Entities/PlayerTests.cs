using System;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void Create_WithValidPlayerId_ReturnsNewPlayer()
        {
            // Arrange
            var playerId = "test-player-123";

            // Act
            var player = Player.Create(playerId);

            // Assert
            Assert.That(player, Is.Not.Null);
            Assert.That(player.PlayerId, Is.EqualTo(playerId));
            Assert.That(player.FirstSeen.Date, Is.EqualTo(DateTime.UtcNow.Date));
            Assert.That(player.RegistrationDate, Is.Null);
            Assert.That(player.FirstDepositDate, Is.Null);
            Assert.That(player.LastEventAt, Is.Null);
        }

        [Test]
        public void Create_WithEmptyPlayerId_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => Player.Create(string.Empty));
        }

        [Test]
        public void Create_WithNullPlayerId_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => Player.Create(null));
        }

        [Test]
        public void Register_SetsRegistrationDate()
        {
            // Arrange
            var player = Player.Create("test-player-123");

            // Act
            player.Register();

            // Assert
            Assert.That(player.RegistrationDate, Is.Not.Null);
            Assert.That(player.RegistrationDate.Value.Date, Is.EqualTo(DateTime.UtcNow.Date));
        }

        [Test]
        public void Deposit_WhenNotRegistered_ThrowsInvalidOperationException()
        {
            // Arrange
            var player = Player.Create("test-player-123");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => player.Deposit());
        }

        [Test]
        public void Deposit_WhenRegistered_SetsFirstDepositDate()
        {
            // Arrange
            var player = Player.Create("test-player-123");
            player.Register();

            // Act
            player.Deposit();

            // Assert
            Assert.That(player.FirstDepositDate, Is.Not.Null);
            Assert.That(player.FirstDepositDate.Value.Date, Is.EqualTo(DateTime.UtcNow.Date));
        }

        [Test]
        public void Deposit_WhenAlreadyDeposited_DoesNotUpdateFirstDepositDate()
        {
            // Arrange
            var player = Player.Create("test-player-123");
            player.Register();
            player.Deposit();
            var firstDepositDate = player.FirstDepositDate;

            // Act
            player.Deposit();

            // Assert
            Assert.That(player.FirstDepositDate, Is.EqualTo(firstDepositDate));
        }

        [Test]
        public void UpdateLastEvent_UpdatesLastEventTimestamp()
        {
            // Arrange
            var player = Player.Create("test-player-123");
            var eventTime = DateTime.UtcNow.AddMinutes(-5);

            // Act
            player.UpdateLastEvent(eventTime);

            // Assert
            Assert.That(player.LastEventAt, Is.EqualTo(eventTime));
        }
    }
}