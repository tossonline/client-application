-- =============================================
-- Initial Seed Data
-- Purpose: Insert default dashboards and initial configuration data
-- =============================================

-- Insert default dashboards
INSERT INTO [dbo].[Dashboards] (Id, Name, Description, Configuration, CreatedBy)
VALUES 
    ('campaign-overview', 'Campaign Overview Dashboard', 'Main dashboard for campaign performance monitoring', 
     '{"layout":"grid","widgets":["visits","conversions","trends"],"refreshInterval":30,"theme":"light"}', 'system'),
    
    ('player-analytics', 'Player Analytics Dashboard', 'Dashboard for player behavior and lifecycle analysis', 
     '{"layout":"tabs","widgets":["funnel","retention","geography"],"refreshInterval":60,"theme":"light"}', 'system'),
    
    ('real-time-metrics', 'Real-time Metrics Dashboard', 'Live dashboard for real-time analytics', 
     '{"layout":"stream","widgets":["live-events","live-metrics"],"refreshInterval":5,"theme":"dark"}', 'system'),
    
    ('executive-summary', 'Executive Summary Dashboard', 'High-level KPIs and business metrics', 
     '{"layout":"summary","widgets":["kpis","trends","alerts"],"refreshInterval":300,"theme":"light"}', 'system'),
    
    ('anomaly-detection', 'Anomaly Detection Dashboard', 'Monitor and investigate detected anomalies', 
     '{"layout":"detection","widgets":["anomalies","thresholds","investigations"],"refreshInterval":120,"theme":"light"}', 'system');

PRINT 'Default dashboards inserted successfully';

-- Insert initial system job record
INSERT INTO [dbo].[AnalyticsJobs] (JobName, JobType, Status, StartedAt, CompletedAt, Parameters, RecordsProcessed)
VALUES 
    ('Initial Database Setup', 'Setup', 'Completed', GETUTCDATE(), GETUTCDATE(), 
     '{"action":"create_database","version":"1.0.0"}', 0);

PRINT 'Initial system job record inserted successfully';

PRINT 'Initial seed data completed successfully';