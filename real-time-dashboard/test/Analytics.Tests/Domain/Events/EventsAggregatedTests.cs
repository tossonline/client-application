using System;
using System.Collections.Generic;
using Analytics.Domain.Events;
using NUnit.Framework;

namespace Analytics.Tests.Domain.Events
{
    [TestFixture]
    public class EventsAggregatedTests
    {
        [Test]
        public void EventsAggregated_Creation_WithValidData_ShouldSucceed()
        {
            // Arrange
            var fromDate = DateTime.Today.AddDays(-7);
            var toDate = DateTime.Today;
            var eventType = "visit";
            var bannerTag = "brandA";
            var totalCount = 1000;

            // Act
            var @event = new EventsAggregated
            {
                FromDate = fromDate,
                ToDate = toDate,
                EventType = eventType,
                BannerTag = bannerTag,
                TotalCount = totalCount
            };

            // Assert
            Assert.That(@event.FromDate, Is.EqualTo(fromDate));
            Assert.That(@event.ToDate, Is.EqualTo(toDate));
            Assert.That(@event.EventType, Is.EqualTo(eventType));
            Assert.That(@event.BannerTag, Is.EqualTo(bannerTag));
            Assert.That(@event.TotalCount, Is.EqualTo(totalCount));
            Assert.That(@event.DailyCounts, Is.Not.Null);
            Assert.That(@event.AggregatedAt, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-1)));
        }

        [Test]
        public void EventsAggregated_Creation_ShouldInitializeEmptyDailyCounts()
        {
            // Act
            var @event = new EventsAggregated();

            // Assert
            Assert.That(@event.DailyCounts, Is.Not.Null);
            Assert.That(@event.DailyCounts.Count, Is.EqualTo(0));
        }

        [Test]
        public void EventsAggregated_Creation_ShouldSetAggregatedAtToUtcNow()
        {
            // Arrange
            var beforeCreation = DateTime.UtcNow;

            // Act
            var @event = new EventsAggregated();

            // Assert
            Assert.That(@event.AggregatedAt, Is.GreaterThanOrEqualTo(beforeCreation)) 