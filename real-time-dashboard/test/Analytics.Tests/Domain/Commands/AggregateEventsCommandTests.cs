using System;
using System.Collections.Generic;
using Analytics.Domain.Commands;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Commands
{
    [TestFixture]
    public class AggregateEventsCommandTests
    {
        [Test]
        public void AggregateEventsCommand_Creation_WithValidData_ShouldSucceed()
        {
            // Arrange
            var fromDate = DateTime.Today.AddDays(-7);
            var toDate = DateTime.Today;

            // Act
            var command = new AggregateEventsCommand(fromDate, toDate);

            // Assert
            Assert.That(command.FromDate, Is.EqualTo(fromDate));
            Assert.That(command.ToDate, Is.EqualTo(toDate));
            Assert.That(command.EventType, Is.Null);
            Assert.That(command.BannerTag, Is.Null);
        }

        [Test]
        public void AggregateEventsCommand_Creation_WithFilters_ShouldSucceed()
        {
            // Arrange
            var fromDate = DateTime.Today.AddDays(-7);
            var toDate = DateTime.Today;

            // Act
            var command = new AggregateEventsCommand(fromDate, toDate)
            {
                EventType = "visit",
                BannerTag = "brandA"
            };

            // Assert
            Assert.That(command.FromDate, Is.EqualTo(fromDate));
            Assert.That(command.ToDate, Is.EqualTo(toDate));
            Assert.That(command.EventType, Is.EqualTo("visit"));
            Assert.That(command.BannerTag, Is.EqualTo("brandA"));
        }

        [Test]
        public void AggregateEventsCommand_Validation_WithValidDateRange_ShouldReturnTrue()
        {
            // Arrange
            var command = new AggregateEventsCommand(DateTime.Today.AddDays(-7), DateTime.Today);

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void AggregateEventsCommand_Validation_WithInvalidDateRange_ShouldReturnFalse()
        {
            // Arrange
            var command = new AggregateEventsCommand(DateTime.Today, DateTime.Today.AddDays(-7));

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void AggregateEventsCommand_Validation_WithOldFromDate_ShouldReturnFalse()
        {
            // Arrange
            var command = new AggregateEventsCommand(DateTime.Today.AddDays(-400), DateTime.Today);

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void AggregateEventsCommand_Validation_WithFutureToDate_ShouldReturnFalse()
        {
            // Arrange
            var command = new AggregateEventsCommand(DateTime.Today, DateTime.Today.AddDays(2));

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void AggregateEventsCommand_Validation_WithValidFutureDate_ShouldReturnTrue()
        {
            // Arrange
            var command = new AggregateEventsCommand(DateTime.Today, DateTime.Today.AddDays(1));

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void AggregateEventsCommand_Validation_WithSameDate_ShouldReturnTrue()
        {
            // Arrange
            var command = new AggregateEventsCommand(DateTime.Today, DateTime.Today);

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void AggregateEventsCommand_DateRangeFiltering_WithDifferentEventTypes_ShouldSucceed()
        {
            // Arrange
            var visitCommand = new AggregateEventsCommand(DateTime.Today.AddDays(-7), DateTime.Today)
            {
                EventType = "visit"
            };

            var registrationCommand = new AggregateEventsCommand(DateTime.Today.AddDays(-7), DateTime.Today)
            {
                EventType = "registration"
            };

            var depositCommand = new AggregateEventsCommand(DateTime.Today.AddDays(-7), DateTime.Today)
            {
                EventType = "deposit"
            };

            // Act & Assert
            Assert.That(visitCommand.IsValid(), Is.True);
            Assert.That(registrationCommand.IsValid(), Is.True);
            Assert.That(depositCommand.IsValid(), Is.True);
        }

        [Test]
        public void AggregateEventsCommand_BannerTagFiltering_WithDifferentBannerTags_ShouldSucceed()
        {
            // Arrange
            var brandACommand = new AggregateEventsCommand(DateTime.Today.AddDays(-7), DateTime.Today)
            {
                BannerTag = "brandA"
            };

            var brandBCommand = new AggregateEventsCommand(DateTime.Today.AddDays(-7), DateTime.Today)
            {
                BannerTag = "brandB"
            };

            // Act & Assert
            Assert.That(brandACommand.IsValid(), Is.True);
            Assert.That(brandBCommand.IsValid(), Is.True);
        }

        [Test]
        public void AggregateEventsCommand_AggregationParameters_WithAllFilters_ShouldSucceed()
        {
            // Arrange
            var command = new AggregateEventsCommand(DateTime.Today.AddDays(-7), DateTime.Today)
            {
                EventType = "visit",
                BannerTag = "brandA"
            };

            // Act
            var isValid = command.IsValid();

            // Assert
            Assert.That(isValid, Is.True);
            Assert.That(command.FromDate, Is.EqualTo(DateTime.Today.AddDays(-7)));
            Assert.That(command.ToDate, Is.EqualTo(DateTime.Today));
            Assert.That(command.EventType, Is.EqualTo("visit"));
            Assert.That(command.BannerTag, Is.EqualTo("brandA"));
        }
    }
} 