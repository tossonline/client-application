using System;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class EventSummaryTests
    {
        [Test]
        public void EventSummary_Creation_WithValidData_ShouldSucceed()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var eventType = "visit";
            var bannerTag = "brandA";
            var count = 100;

            // Act
            var eventSummary = new EventSummary
            {
                EventDate = eventDate,
                EventType = eventType,
                BannerTag = bannerTag,
                Count = count
            };

            // Assert
            Assert.That(eventSummary.EventDate, Is.EqualTo(eventDate));
            Assert.That(eventSummary.EventType, Is.EqualTo(eventType));
            Assert.That(eventSummary.BannerTag, Is.EqualTo(bannerTag));
            Assert.That(eventSummary.Count, Is.EqualTo(count));
        }

        [Test]
        public void EventSummary_WithNullBannerTag_ShouldSucceed()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var eventType = "visit";
            var count = 50;

            // Act
            var eventSummary = new EventSummary
            {
                EventDate = eventDate,
                EventType = eventType,
                BannerTag = null,
                Count = count
            };

            // Assert
            Assert.That(eventSummary.BannerTag, Is.Null);
        }

        [Test]
        public void EventSummary_WithZeroCount_ShouldSucceed()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var eventType = "visit";
            var bannerTag = "brandA";

            // Act
            var eventSummary = new EventSummary
            {
                EventDate = eventDate,
                EventType = eventType,
                BannerTag = bannerTag,
                Count = 0
            };

            // Assert
            Assert.That(eventSummary.Count, Is.EqualTo(0));
        }

        [Test]
        public void EventSummary_WithNegativeCount_ShouldSucceed()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var eventType = "visit";
            var bannerTag = "brandA";

            // Act
            var eventSummary = new EventSummary
            {
                EventDate = eventDate,
                EventType = eventType,
                BannerTag = bannerTag,
                Count = -10
            };

            // Assert
            Assert.That(eventSummary.Count, Is.EqualTo(-10));
        }

        [Test]
        public void EventSummary_WithDifferentEventTypes_ShouldSucceed()
        {
            // Arrange
            var eventDate = DateTime.Today;
            var visitEvent = new EventSummary
            {
                EventDate = eventDate,
                EventType = "visit",
                BannerTag = "brandA",
                Count = 100
            };

            var registrationEvent = new EventSummary
            {
                EventDate = eventDate,
                EventType = "registration",
                BannerTag = "brandA",
                Count = 25
            };

            var depositEvent = new EventSummary
            {
                EventDate = eventDate,
                EventType = "deposit",
                BannerTag = "brandA",
                Count = 10
            };

            // Assert
            Assert.That(visitEvent.EventType, Is.EqualTo("visit"));
            Assert.That(registrationEvent.EventType, Is.EqualTo("registration"));
            Assert.That(depositEvent.EventType, Is.EqualTo("deposit"));
        }
    }
}

