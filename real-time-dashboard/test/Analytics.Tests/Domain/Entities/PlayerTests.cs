using System;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void Player_Create_WithValidPlayerId_ShouldSucceed()
        {
            // Arrange
            var playerId = "player123";

            // Act
            var player = Player.Create(playerId);

            // Assert
            Assert.That(player.PlayerId, Is.EqualTo(playerId));
            Assert.That(player.FirstSeen, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-1)));
            Assert.That(player.LastEventAt, Is.Null);
            Assert.That(player.RegistrationDate, Is.Null);
            Assert.That(player.FirstDepositDate, Is.Null);
        }

        [Test]
        public void Player_Register_ShouldSetRegistrationDate()
        {
            // Arrange
            var player = Player.Create("player123");

            // Act
            player.Register();

            // Assert
            Assert.That(player.RegistrationDate, Is.Not.Null);
            Assert.That(player.RegistrationDate, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-1)));
        }

        [Test]
        public void Player_Deposit_WithoutRegistration_ShouldThrowException()
        {
            // Arrange
            var player = Player.Create("player123");

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => player.Deposit());
            Assert.That(ex.Message, Is.EqualTo("Player must be registered before depositing"));
        }

        [Test]
        public void Player_Deposit_WithRegistration_ShouldSetFirstDepositDate()
        {
            // Arrange
            var player = Player.Create("player123");
            player.Register();

            // Act
            player.Deposit();

            // Assert
            Assert.That(player.FirstDepositDate, Is.Not.Null);
            Assert.That(player.FirstDepositDate, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-1)));
        }

        [Test]
        public void Player_UpdateLastEvent_ShouldUpdateLastEventAt()
        {
            // Arrange
            var player = Player.Create("player123");
            var eventTime = DateTime.UtcNow;

            // Act
            player.UpdateLastEvent(eventTime);

            // Assert
            Assert.That(player.LastEventAt, Is.EqualTo(eventTime));
        }

        [Test]
        public void Player_Create_WithNullPlayerId_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => Player.Create(null));
            Assert.That(ex.ParamName, Is.EqualTo("playerId"));
        }

        [Test]
        public void Player_Create_WithEmptyPlayerId_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Player.Create(""));
            Assert.That(ex.ParamName, Is.EqualTo("playerId"));
        }
    }
}

