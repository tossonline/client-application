using System;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class EventSummaryTests
    {
        [Test]
        public void EventSummary_WhenCreated_HasExpectedDefaults()
        {
            // Arrange & Act
            var summary = new EventSummary();

            // Assert
            Assert.That(summary.EventType, Is.EqualTo(string.Empty));
            Assert.That(summary.BannerTag, Is.Null);
            Assert.That(summary.Count, Is.EqualTo(0));
        }

        [Test]
        public void EventSummary_WithValues_SetsPropertiesCorrectly()
        {
            // Arrange
            var date = DateTime.UtcNow.Date;

            // Act
            var summary = new EventSummary
            {
                EventDate = date,
                EventType = "visit",
                BannerTag = "banner-123",
                Count = 42
            };

            // Assert
            Assert.That(summary.EventDate, Is.EqualTo(date));
            Assert.That(summary.EventType, Is.EqualTo("visit"));
            Assert.That(summary.BannerTag, Is.EqualTo("banner-123"));
            Assert.That(summary.Count, Is.EqualTo(42));
        }
    }
}