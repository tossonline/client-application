using System;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class PlayerTests
    {
        private const string ValidPlayerId = "player123";

        [Test]
        public void Create_WithValidId_CreatesPlayer()
        {
            // Act
            var player = Player.Create(ValidPlayerId);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(player.PlayerId, Is.EqualTo(ValidPlayerId));
                Assert.That(player.FirstSeen, Is.LessThanOrEqualTo(DateTime.UtcNow));
                Assert.That(player.LastEventAt, Is.Null);
                Assert.That(player.RegistrationDate, Is.Null);
                Assert.That(player.FirstDepositDate, Is.Null);
                Assert.That(player.TotalDeposits, Is.EqualTo(0));
                Assert.That(player.TotalDepositAmount, Is.EqualTo(0));
                Assert.That(player.Status, Is.EqualTo(PlayerStatus.Visitor));
                Assert.That(player.Segment, Is.EqualTo(PlayerSegment.New));
            });
        }

        [Test]
        public void Create_WithNullId_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => Player.Create(null));
            Assert.That(ex.Message, Does.Contain("Player ID cannot be null"));
        }

        [Test]
        public void Register_UpdatesRegistrationDateAndStatus()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);

            // Act
            player.Register();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(player.RegistrationDate, Is.Not.Null);
                Assert.That(player.Status, Is.EqualTo(PlayerStatus.Registered));
            });
        }

        [Test]
        public void Register_WhenAlreadyRegistered_ThrowsInvalidOperationException()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            player.Register();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => player.Register());
            Assert.That(ex.Message, Does.Contain("already registered"));
        }

        [Test]
        public void Deposit_UpdatesDepositDateAndStatus()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            player.Register();

            // Act
            player.Deposit(100);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(player.FirstDepositDate, Is.Not.Null);
                Assert.That(player.Status, Is.EqualTo(PlayerStatus.Deposited));
                Assert.That(player.TotalDeposits, Is.EqualTo(1));
                Assert.That(player.TotalDepositAmount, Is.EqualTo(100));
            });
        }

        [Test]
        public void Deposit_WithoutRegistration_ThrowsInvalidOperationException()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => player.Deposit(100));
            Assert.That(ex.Message, Does.Contain("must be registered"));
        }

        [Test]
        public void Deposit_WithNegativeAmount_ThrowsArgumentException()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            player.Register();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => player.Deposit(-100));
            Assert.That(ex.Message, Does.Contain("must be positive"));
        }

        [Test]
        public void UpdateLastEvent_UpdatesTimestamp()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            var eventTime = DateTime.UtcNow;

            // Act
            player.UpdateLastEvent(eventTime);

            // Assert
            Assert.That(player.LastEventAt, Is.EqualTo(eventTime));
        }

        [Test]
        public void UpdateLastEvent_WithTimestampBeforeFirstSeen_ThrowsArgumentException()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            var eventTime = player.FirstSeen.AddMinutes(-1);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => player.UpdateLastEvent(eventTime));
            Assert.That(ex.Message, Does.Contain("cannot be before first seen"));
        }

        [Test]
        public void GetLifetimeDays_CalculatesCorrectly()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            var lastEvent = player.FirstSeen.AddDays(5);
            player.UpdateLastEvent(lastEvent);

            // Act
            var lifetimeDays = player.GetLifetimeDays();

            // Assert
            Assert.That(lifetimeDays, Is.EqualTo(5));
        }

        [Test]
        public void GetTimeToRegistration_CalculatesCorrectly()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            System.Threading.Thread.Sleep(100); // Small delay
            player.Register();

            // Act
            var timeToRegistration = player.GetTimeToRegistration();

            // Assert
            Assert.That(timeToRegistration, Is.GreaterThan(0));
        }

        [Test]
        public void GetTimeToFirstDeposit_CalculatesCorrectly()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            player.Register();
            System.Threading.Thread.Sleep(100); // Small delay
            player.Deposit(100);

            // Act
            var timeToDeposit = player.GetTimeToFirstDeposit();

            // Assert
            Assert.That(timeToDeposit, Is.GreaterThan(0));
        }

        [Test]
        public void GetAverageDepositAmount_CalculatesCorrectly()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            player.Register();
            player.Deposit(100);
            player.Deposit(200);

            // Act
            var avgAmount = player.GetAverageDepositAmount();

            // Assert
            Assert.That(avgAmount, Is.EqualTo(150));
        }

        [Test]
        public void GetAverageDepositAmount_WithNoDeposits_ReturnsZero()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);

            // Act
            var avgAmount = player.GetAverageDepositAmount();

            // Assert
            Assert.That(avgAmount, Is.EqualTo(0));
        }

        [Test]
        public void Segment_UpdatesToVIP_AfterMultipleDeposits()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            player.Register();

            // Act
            for (int i = 0; i < 5; i++)
            {
                player.Deposit(100);
            }

            // Assert
            Assert.That(player.Segment, Is.EqualTo(PlayerSegment.VIP));
        }

        [Test]
        public void Segment_UpdatesToRegular_AfterTwoDeposits()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            player.Register();

            // Act
            player.Deposit(100);
            player.Deposit(100);

            // Assert
            Assert.That(player.Segment, Is.EqualTo(PlayerSegment.Regular));
        }

        [Test]
        public void Segment_UpdatesToNonDepositor_AfterInactivity()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            player.Register();
            player.UpdateLastEvent(DateTime.UtcNow.AddDays(-8));

            // Assert
            Assert.That(player.Segment, Is.EqualTo(PlayerSegment.NonDepositor));
        }

        [Test]
        public void Segment_UpdatesToInactive_AfterLongInactivity()
        {
            // Arrange
            var player = Player.Create(ValidPlayerId);
            player.UpdateLastEvent(DateTime.UtcNow.AddDays(-31));

            // Assert
            Assert.That(player.Segment, Is.EqualTo(PlayerSegment.Inactive));
        }
    }
}