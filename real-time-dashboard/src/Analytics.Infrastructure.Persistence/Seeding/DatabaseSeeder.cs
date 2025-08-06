// Copyright (c) DigiOutsource. All rights reserved.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Analytics.Domain.Entities;
using Analytics.Infrastructure.Persistence.Contexts;

namespace Analytics.Infrastructure.Persistence.Seeding
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AnalyticsContext context, ILogger logger)
        {
            try
            {
                logger.LogInformation("Starting database seeding for Analytics...");

                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Seed Dashboards
                await SeedDashboardsAsync(context, logger);

                // Seed sample data for development (optional)
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    await SeedSampleDataAsync(context, logger);
                }

                await context.SaveChangesAsync();
                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private static async Task SeedDashboardsAsync(AnalyticsContext context, ILogger logger)
        {
            if (!await context.Dashboards.AnyAsync())
            {
                logger.LogInformation("Seeding default dashboards...");

                var dashboards = new List<Dashboards>
                {
                    CreateDashboard("campaign-overview", "Campaign Overview Dashboard", 
                        "Main dashboard for campaign performance monitoring",
                        "{\"layout\":\"grid\",\"widgets\":[\"visits\",\"conversions\",\"trends\"],\"refreshInterval\":30}"),
                    
                    CreateDashboard("player-analytics", "Player Analytics Dashboard", 
                        "Dashboard for player behavior and lifecycle analysis",
                        "{\"layout\":\"tabs\",\"widgets\":[\"funnel\",\"retention\",\"geography\"],\"refreshInterval\":60}"),
                    
                    CreateDashboard("real-time-metrics", "Real-time Metrics Dashboard", 
                        "Live dashboard for real-time analytics",
                        "{\"layout\":\"stream\",\"widgets\":[\"live-events\",\"live-metrics\"],\"refreshInterval\":5}"),
                    
                    CreateDashboard("executive-summary", "Executive Summary Dashboard", 
                        "High-level KPIs and business metrics",
                        "{\"layout\":\"summary\",\"widgets\":[\"kpis\",\"trends\",\"alerts\"],\"refreshInterval\":300}"),
                    
                    CreateDashboard("anomaly-detection", "Anomaly Detection Dashboard", 
                        "Monitor and investigate detected anomalies",
                        "{\"layout\":\"detection\",\"widgets\":[\"anomalies\",\"thresholds\",\"investigations\"],\"refreshInterval\":120}")
                };

                context.Dashboards.AddRange(dashboards);
                logger.LogInformation($"Added {dashboards.Count} default dashboards.");
            }
        }

        private static async Task SeedSampleDataAsync(AnalyticsContext context, ILogger logger)
        {
            logger.LogInformation("Seeding sample data for development...");

            // Seed sample players
            await SeedSamplePlayersAsync(context);

            // Seed sample pixel events
            await SeedSamplePixelEventsAsync(context);

            // Seed sample event summaries
            await SeedSampleEventSummariesAsync(context);

            logger.LogInformation("Sample data seeding completed.");
        }

        private static async Task SeedSamplePlayersAsync(AnalyticsContext context)
        {
            if (!await context.Players.AnyAsync())
            {
                var players = new List<Player>();
                var random = new Random();

                for (int i = 1; i <= 100; i++)
                {
                    var player = Player.Create($"player_{i:D4}");
                    
                    // Simulate registration for some players
                    if (random.NextDouble() < 0.7) // 70% registration rate
                    {
                        player.Register();
                        
                        // Simulate deposits for some registered players
                        if (random.NextDouble() < 0.3) // 30% of registered players deposit
                        {
                            for (int d = 0; d < random.Next(1, 5); d++)
                            {
                                player.Deposit();
                            }
                        }
                    }
                    
                    players.Add(player);
                }

                context.Players.AddRange(players);
            }
        }

        private static async Task SeedSamplePixelEventsAsync(AnalyticsContext context)
        {
            if (!await context.PixelEvents.AnyAsync())
            {
                var pixelEvents = new List<PixelEvent>();
                var random = new Random();
                var eventTypes = new[] { "visit", "registration", "deposit" };
                var bannerTags = new[] { "campaign_a", "campaign_b", "campaign_c", "campaign_d" };

                // Generate events for the last 30 days
                for (int day = 30; day >= 0; day--)
                {
                    var date = DateTime.UtcNow.AddDays(-day);
                    var eventsPerDay = random.Next(100, 500);

                    for (int i = 0; i < eventsPerDay; i++)
                    {
                        var pixelEvent = new PixelEvent
                        {
                            EventType = eventTypes[random.Next(eventTypes.Length)],
                            PlayerId = $"player_{random.Next(1, 101):D4}",
                            BannerTag = bannerTags[random.Next(bannerTags.Length)],
                            SourceIp = $"192.168.{random.Next(1, 255)}.{random.Next(1, 255)}",
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                            CreatedAt = date.AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60)),
                            Metadata = new Dictionary<string, string>
                            {
                                { "source", "organic" },
                                { "medium", "web" },
                                { "campaign_id", $"camp_{random.Next(1000, 9999)}" }
                            }
                        };

                        pixelEvents.Add(pixelEvent);
                    }
                }

                context.PixelEvents.AddRange(pixelEvents);
            }
        }

        private static async Task SeedSampleEventSummariesAsync(AnalyticsContext context)
        {
            if (!await context.EventSummaries.AnyAsync())
            {
                var eventSummaries = new List<EventSummary>();
                var random = new Random();
                var eventTypes = new[] { "visit", "registration", "deposit" };
                var bannerTags = new[] { "campaign_a", "campaign_b", "campaign_c", "campaign_d" };

                // Generate summaries for the last 30 days
                for (int day = 30; day >= 0; day--)
                {
                    var date = DateTime.Today.AddDays(-day);

                    foreach (var bannerTag in bannerTags)
                    {
                        foreach (var eventType in eventTypes)
                        {
                            var baseCount = eventType switch
                            {
                                "visit" => random.Next(50, 200),
                                "registration" => random.Next(10, 50),
                                "deposit" => random.Next(2, 20),
                                _ => random.Next(1, 10)
                            };

                            var eventSummary = new EventSummary
                            {
                                EventDate = date,
                                EventType = eventType,
                                BannerTag = bannerTag,
                                Count = baseCount
                            };

                            eventSummaries.Add(eventSummary);
                        }
                    }
                }

                context.EventSummaries.AddRange(eventSummaries);
            }
        }

        private static Dashboards CreateDashboard(string id, string name, string description, string configuration)
        {
            return new Dashboards(); // Note: This needs to be updated based on the actual Dashboards entity constructor
            // The Dashboards entity currently only has an Id property, so we'll need to extend it
        }
    }
}