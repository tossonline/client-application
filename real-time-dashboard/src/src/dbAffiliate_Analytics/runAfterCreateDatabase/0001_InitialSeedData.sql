-- =============================================
-- Initial Seed Data
-- Purpose: Insert default dashboards and initial configuration data
-- Database: dbAffiliate_Analytics
-- Script Type: runAfterCreateDatabase - runs after database creation
-- =============================================

-- Insert default dashboards for Analytics platform
INSERT INTO [dbo].[Dashboards] (Id, Name, Description, Configuration, CreatedBy)
VALUES 
    ('campaign-overview', 'Campaign Overview Dashboard', 'Main dashboard for campaign performance monitoring via CampaignAggregate', 
     '{"layout":"grid","widgets":["visits","conversions","trends"],"refreshInterval":30,"theme":"light","aggregates":["CampaignAggregate","MetricsAggregate"]}', 'system'),
    
    ('player-analytics', 'Player Analytics Dashboard', 'Dashboard for player behavior and lifecycle analysis via PlayerAggregate', 
     '{"layout":"tabs","widgets":["funnel","retention","geography"],"refreshInterval":60,"theme":"light","aggregates":["PlayerAggregate","AnalyticsAggregate"]}', 'system'),
    
    ('real-time-metrics', 'Real-time Metrics Dashboard', 'Live dashboard for real-time analytics via PixelEventAggregate', 
     '{"layout":"stream","widgets":["live-events","live-metrics"],"refreshInterval":5,"theme":"dark","aggregates":["PixelEventAggregate","MetricsAggregate"]}', 'system'),
    
    ('executive-summary', 'Executive Summary Dashboard', 'High-level KPIs and business metrics via AnalyticsAggregate', 
     '{"layout":"summary","widgets":["kpis","trends","alerts"],"refreshInterval":300,"theme":"light","aggregates":["AnalyticsAggregate","CampaignAggregate"]}', 'system'),
    
    ('anomaly-detection', 'Anomaly Detection Dashboard', 'Monitor and investigate detected anomalies via AnalyticsAggregate', 
     '{"layout":"detection","widgets":["anomalies","thresholds","investigations"],"refreshInterval":120,"theme":"light","aggregates":["AnalyticsAggregate"]}', 'system');

PRINT 'Default dashboards inserted successfully';
PRINT 'Dashboards configured for: CampaignAggregate, PlayerAggregate, PixelEventAggregate, MetricsAggregate, AnalyticsAggregate';

-- Insert initial system job record
INSERT INTO [dbo].[AnalyticsJobs] (JobName, JobType, Status, StartedAt, CompletedAt, Parameters, RecordsProcessed)
VALUES 
    ('Initial Database Setup', 'Setup', 'Completed', GETUTCDATE(), GETUTCDATE(), 
     '{"action":"create_database","version":"1.0.0","aggregates":["CampaignAggregate","PlayerAggregate","PixelEventAggregate","MetricsAggregate","AnalyticsAggregate"]}', 0);

PRINT 'Initial system job record inserted successfully';
PRINT 'dbAffiliate_Analytics initial seed data completed successfully';
PRINT 'Ready for .NET 8 Analytics Platform with Domain Aggregates';