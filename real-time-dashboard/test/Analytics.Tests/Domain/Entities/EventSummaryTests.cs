using System;
using Analytics.Domain.Entities;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Entities
{
    [TestFixture]
    public class EventSummaryTests
    {
        private DateTime _testDate;
        private const string EventType = "visit";
        private const string BannerTag = "campaign1-banner";

        [SetUp]
        public void Setup()
        {
            _testDate = DateTime.UtcNow.Date;
        }

        [Test]
        public void Create_WithValidData_CreatesSummary()
        {
            // Act
            var summary = EventSummary.Create(_testDate, EventType, BannerTag, TimePeriod.Daily);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(summary.EventDate, Is.EqualTo(_testDate));
                Assert.That(summary.EventType, Is.EqualTo(EventType));
                Assert.That(summary.BannerTag, Is.EqualTo(BannerTag));
                Assert.That(summary.Count, Is.EqualTo(0));
                Assert.That(summary.Period, Is.EqualTo(TimePeriod.Daily));
            });
        }

        [Test]
        public void Create_WithNullEventType_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                EventSummary.Create(_testDate, null, BannerTag, TimePeriod.Daily));
            Assert.That(ex.Message, Does.Contain("Event type cannot be null"));
        }

        [Test]
        public void Create_WithEmptyEventType_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                EventSummary.Create(_testDate, string.Empty, BannerTag, TimePeriod.Daily));
            Assert.That(ex.Message, Does.Contain("Event type cannot be null"));
        }

        [Test]
        public void IncrementCount_IncreasesCount()
        {
            // Arrange
            var summary = EventSummary.Create(_testDate, EventType, BannerTag, TimePeriod.Daily);

            // Act
            summary.IncrementCount();
            summary.IncrementCount(2);

            // Assert
            Assert.That(summary.Count, Is.EqualTo(3));
        }

        [Test]
        public void IncrementCount_WithNegativeAmount_ThrowsArgumentException()
        {
            // Arrange
            var summary = EventSummary.Create(_testDate, EventType, BannerTag, TimePeriod.Daily);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => summary.IncrementCount(-1));
            Assert.That(ex.Message, Does.Contain("must be positive"));
        }

        [Test]
        public void DecrementCount_DecreasesCount()
        {
            // Arrange
            var summary = EventSummary.Create(_testDate, EventType, BannerTag, TimePeriod.Daily);
            summary.IncrementCount(5);

            // Act
            summary.DecrementCount();
            summary.DecrementCount(2);

            // Assert
            Assert.That(summary.Count, Is.EqualTo(2));
        }

        [Test]
        public void DecrementCount_WithNegativeAmount_ThrowsArgumentException()
        {
            // Arrange
            var summary = EventSummary.Create(_testDate, EventType, BannerTag, TimePeriod.Daily);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => summary.DecrementCount(-1));
            Assert.That(ex.Message, Does.Contain("must be positive"));
        }

        [Test]
        public void DecrementCount_BelowZero_ThrowsInvalidOperationException()
        {
            // Arrange
            var summary = EventSummary.Create(_testDate, EventType, BannerTag, TimePeriod.Daily);
            summary.IncrementCount(1);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => summary.DecrementCount(2));
            Assert.That(ex.Message, Does.Contain("below zero"));
        }

        [Test]
        public void Merge_WithValidSummary_MergesCounts()
        {
            // Arrange
            var summary1 = EventSummary.Create(_testDate, EventType, BannerTag, TimePeriod.Daily);
            var summary2 = EventSummary.Create(_testDate, EventType, BannerTag, TimePeriod.Daily);
            summary1.IncrementCount(3);
            summary2.IncrementCount(2);

            // Act
            summary1.Merge(summary2);

            // Assert
            Assert.That(summary1.Count, Is.EqualTo(5));
        }

        [Test]
        public void Merge_WithNullSummary_ThrowsArgumentNullException()
        {
            // Arrange
            var summary = EventSummary.Create(_testDate, EventType, BannerTag, TimePeriod.Daily);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => summary.Merge(null));
        }

        [Test]
        public void Merge_WithDifferentEventType_ThrowsArgumentException()
        {
            // Arrange
            var summary1 = EventSummary.Create(_testDate, "visit", BannerTag, TimePeriod.Daily);
            var summary2 = EventSummary.Create(_testDate, "registration", BannerTag, TimePeriod.Daily);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => summary1.Merge(summary2));
            Assert.That(ex.Message, Does.Contain("different event types"));
        }

        [Test]
        public void Merge_WithDifferentDate_ThrowsArgumentException()
        {
            // Arrange
            var summary1 = EventSummary.Create(_testDate, EventType, BannerTag, TimePeriod.Daily);
            var summary2 = EventSummary.Create(_testDate.AddDays(1), EventType, BannerTag, TimePeriod.Daily);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => summary1.Merge(summary2));
            Assert.That(ex.Message, Does.Contain("different"));
        }

        [TestCase(TimePeriod.Hourly)]
        [TestCase(TimePeriod.Daily)]
        [TestCase(TimePeriod.Weekly)]
        [TestCase(TimePeriod.Monthly)]
        public void Create_WithDifferentPeriods_SetsPeriodCorrectly(TimePeriod period)
        {
            // Act
            var summary = EventSummary.Create(_testDate, EventType, BannerTag, period);

            // Assert
            Assert.That(summary.Period, Is.EqualTo(period));
        }
    }
}