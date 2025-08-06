# Analytics Database Schema - dbAffiliate_Analytics

This document describes the database schema for the Analytics platform's primary database `dbAffiliate_Analytics`.

## Overview

The `dbAffiliate_Analytics` database is designed to store and analyze marketing campaign data, user behavior tracking, and business intelligence metrics for an affiliate marketing platform.

## Database Tables

### Core Entities

#### 1. PixelEvents
Stores raw pixel tracking events from user interactions.

| Column | Type | Description |
|--------|------|-------------|
| Id | UNIQUEIDENTIFIER | Primary key, auto-generated |
| EventType | NVARCHAR(50) | Type of event (visit, registration, deposit) |
| PlayerId | NVARCHAR(100) | Unique identifier for the user |
| BannerTag | NVARCHAR(100) | Campaign/banner identifier |
| Metadata | NVARCHAR(MAX) | JSON metadata dictionary |
| SourceIp | NVARCHAR(45) | IP address (supports IPv6) |
| UserAgent | NVARCHAR(500) | Browser user agent string |
| CreatedAt | DATETIME2(7) | Event timestamp (UTC) |

**Indexes:**
- IX_PixelEvents_PlayerId
- IX_PixelEvents_BannerTag
- IX_PixelEvents_EventType
- IX_PixelEvents_CreatedAt
- IX_PixelEvents_PlayerId_EventType (Composite)
- IX_PixelEvents_BannerTag_CreatedAt (Composite)

#### 2. Players
Tracks user lifecycle and conversion milestones.

| Column | Type | Description |
|--------|------|-------------|
| PlayerId | NVARCHAR(100) | Primary key, unique user identifier |
| FirstSeen | DATETIME2(7) | First interaction timestamp |
| LastEventAt | DATETIME2(7) | Last activity timestamp |
| RegistrationAt | DATETIME2(7) | Registration completion timestamp |
| DepositAt | DATETIME2(7) | First deposit timestamp |
| TotalDeposits | INT | Total number of deposits made |
| CreatedAt | DATETIME2(7) | Record creation timestamp |
| UpdatedAt | DATETIME2(7) | Record last modified timestamp |

**Indexes:**
- IX_Players_FirstSeen
- IX_Players_RegistrationAt
- IX_Players_DepositAt
- IX_Players_TotalDeposits

#### 3. EventSummaries
Daily aggregated metrics per campaign and event type.

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT IDENTITY | Primary key, auto-incrementing |
| EventDate | DATE | Date of the aggregated events |
| EventType | NVARCHAR(50) | Type of event being aggregated |
| BannerTag | NVARCHAR(100) | Campaign/banner identifier |
| Count | INT | Number of events for the day |
| CreatedAt | DATETIME2(7) | Record creation timestamp |
| UpdatedAt | DATETIME2(7) | Record last modified timestamp |

**Constraints:**
- UQ_EventSummaries_Date_Type_Banner (Unique constraint)

**Indexes:**
- IX_EventSummaries_EventDate
- IX_EventSummaries_BannerTag
- IX_EventSummaries_EventType
- IX_EventSummaries_BannerTag_EventDate (Composite)
- IX_EventSummaries_EventType_EventDate (Composite)

#### 4. Dashboards
Configuration and metadata for analytics dashboards.

| Column | Type | Description |
|--------|------|-------------|
| Id | NVARCHAR(100) | Primary key, dashboard identifier |
| Name | NVARCHAR(200) | Dashboard display name |
| Description | NVARCHAR(500) | Dashboard description |
| Configuration | NVARCHAR(MAX) | JSON configuration for dashboard layout |
| IsActive | BIT | Whether dashboard is active |
| CreatedAt | DATETIME2(7) | Record creation timestamp |
| UpdatedAt | DATETIME2(7) | Record last modified timestamp |
| CreatedBy | NVARCHAR(100) | User who created the dashboard |
| UpdatedBy | NVARCHAR(100) | User who last updated the dashboard |

**Indexes:**
- IX_Dashboards_Name
- IX_Dashboards_IsActive
- IX_Dashboards_CreatedAt

### Analytics Tables

#### 5. CampaignMetrics
Pre-calculated campaign performance metrics.

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT IDENTITY | Primary key |
| BannerTag | NVARCHAR(100) | Campaign identifier |
| MetricDate | DATE | Date of the metrics |
| TotalVisits | INT | Total visits for the day |
| TotalRegistrations | INT | Total registrations for the day |
| TotalDeposits | INT | Total deposits for the day |
| ConversionRate | DECIMAL(5,4) | Overall conversion rate (deposits/visits) |
| VisitToRegistrationRate | DECIMAL(5,4) | Registration conversion rate |
| RegistrationToDepositRate | DECIMAL(5,4) | Deposit conversion rate |
| CalculatedAt | DATETIME2(7) | When metrics were calculated |

**Constraints:**
- UQ_CampaignMetrics_Banner_Date (Unique constraint)

#### 6. AnomalyDetections
Detected statistical anomalies in metrics.

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT IDENTITY | Primary key |
| DetectionDate | DATE | Date when anomaly was detected |
| BannerTag | NVARCHAR(100) | Affected campaign (nullable for global anomalies) |
| MetricType | NVARCHAR(50) | Type of metric (Visits, Registrations, Deposits) |
| CurrentValue | INT | Actual value that triggered detection |
| ExpectedValue | DECIMAL(10,2) | Expected value based on historical data |
| StandardDeviation | DECIMAL(10,2) | Standard deviation of historical data |
| ZScore | DECIMAL(10,4) | Z-score of the anomaly |
| AnomalyType | NVARCHAR(20) | Type of anomaly (spike, drop) |
| IsAnomaly | BIT | Whether this qualifies as an anomaly |
| Severity | NVARCHAR(10) | Severity level (Low, Medium, High) |
| DetectedAt | DATETIME2(7) | When the anomaly was detected |

#### 7. GeoAnalytics
Geographic distribution analytics.

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT IDENTITY | Primary key |
| AnalysisDate | DATE | Date of the analysis |
| Country | NVARCHAR(100) | Country name or code |
| BannerTag | NVARCHAR(100) | Campaign identifier (nullable for global stats) |
| TotalEvents | INT | Total events from this country |
| Visits | INT | Total visits |
| Registrations | INT | Total registrations |
| Deposits | INT | Total deposits |
| ConversionRate | DECIMAL(5,4) | Conversion rate for this geography |
| CalculatedAt | DATETIME2(7) | When the analysis was performed |

#### 8. AnalyticsJobs
Background job execution tracking.

| Column | Type | Description |
|--------|------|-------------|
| Id | BIGINT IDENTITY | Primary key |
| JobName | NVARCHAR(100) | Name of the job |
| JobType | NVARCHAR(50) | Type of job (Aggregation, Cleanup, Archive) |
| Status | NVARCHAR(20) | Job status (Running, Completed, Failed, Cancelled) |
| StartedAt | DATETIME2(7) | Job start time |
| CompletedAt | DATETIME2(7) | Job completion time |
| Parameters | NVARCHAR(MAX) | JSON job parameters |
| ErrorMessage | NVARCHAR(MAX) | Error details if job failed |
| RecordsProcessed | BIGINT | Number of records processed |
| CreatedAt | DATETIME2(7) | Record creation timestamp |

## Database Views

### 1. vw_CampaignPerformanceSummary
Aggregated campaign performance with conversion rates.

```sql
SELECT 
    BannerTag,
    EventDate,
    SUM(CASE WHEN EventType = 'visit' THEN Count ELSE 0 END) AS Visits,
    SUM(CASE WHEN EventType = 'registration' THEN Count ELSE 0 END) AS Registrations,
    SUM(CASE WHEN EventType = 'deposit' THEN Count ELSE 0 END) AS Deposits,
    -- Conversion rate calculation
FROM EventSummaries
GROUP BY BannerTag, EventDate
```

### 2. vw_PlayerFunnelAnalysis
Player conversion funnel analysis.

```sql
SELECT 
    PlayerId,
    FirstSeen,
    RegistrationAt,
    DepositAt,
    TotalDeposits,
    -- Conversion flags and timing calculations
FROM Players
```

### 3. vw_DailyMetricsSummary
Daily aggregated metrics across all campaigns.

```sql
SELECT 
    EventDate,
    COUNT(DISTINCT BannerTag) AS UniqueCampaigns,
    SUM(...) AS TotalVisits,
    -- Other aggregated metrics
FROM EventSummaries
GROUP BY EventDate
```

## Stored Procedures

### 1. sp_UpsertEventSummary
Inserts or updates daily event summaries.

**Parameters:**
- @EventDate DATE
- @EventType NVARCHAR(50)
- @BannerTag NVARCHAR(100)
- @Count INT

### 2. sp_GetCampaignPerformance
Retrieves campaign performance for a date range.

**Parameters:**
- @BannerTag NVARCHAR(100)
- @FromDate DATE
- @ToDate DATE

### 3. sp_CalculateCampaignMetrics
Calculates and stores campaign metrics.

**Parameters:**
- @BannerTag NVARCHAR(100) (optional)
- @FromDate DATE
- @ToDate DATE

## Functions

### 1. fn_CalculateConversionRate
Calculates conversion rate with null handling.

**Parameters:**
- @Conversions INT
- @TotalEvents INT

**Returns:** DECIMAL(5,4)

### 2. fn_GetDateRange
Returns a table of dates for a given number of days.

**Parameters:**
- @Days INT

**Returns:** TABLE

## Entity Framework Configuration

The database schema is managed through Entity Framework Core with the following configurations:

- **AnalyticsContext**: Main DbContext with all entity sets
- **Entity Configurations**: Individual mapping configurations for each entity
- **Migrations**: Code-first database migrations
- **Seeding**: Initial data and sample data for development

## Performance Considerations

1. **Indexing Strategy**: Comprehensive indexes on frequently queried columns
2. **Partitioning**: Consider partitioning large tables (PixelEvents) by date
3. **Archiving**: Implement data archiving for old pixel events
4. **Materialized Views**: Consider materialized views for complex aggregations
5. **Query Optimization**: Use stored procedures for complex analytics queries

## Security

1. **Data Encryption**: Sensitive data should be encrypted at rest
2. **Access Control**: Implement proper database roles and permissions
3. **Audit Trail**: All tables include audit fields (CreatedAt, UpdatedAt)
4. **Data Retention**: Implement data retention policies for GDPR compliance

## Maintenance

1. **Index Maintenance**: Regular index rebuilding and statistics updates
2. **Backup Strategy**: Implement appropriate backup and recovery procedures
3. **Monitoring**: Monitor query performance and resource usage
4. **Cleanup Jobs**: Background jobs for data cleanup and archiving

## Usage Examples

### Basic Queries

```sql
-- Get campaign performance for last 7 days
SELECT * FROM vw_CampaignPerformanceSummary 
WHERE EventDate >= DATEADD(DAY, -7, GETDATE())
ORDER BY BannerTag, EventDate;

-- Find top converting campaigns
SELECT TOP 10 BannerTag, AVG(ConversionRate) as AvgConversionRate
FROM CampaignMetrics 
WHERE MetricDate >= DATEADD(DAY, -30, GETDATE())
GROUP BY BannerTag 
ORDER BY AvgConversionRate DESC;

-- Player funnel analysis
SELECT 
    COUNT(*) as TotalPlayers,
    SUM(CASE WHEN RegistrationAt IS NOT NULL THEN 1 ELSE 0 END) as RegisteredPlayers,
    SUM(CASE WHEN DepositAt IS NOT NULL THEN 1 ELSE 0 END) as DepositedPlayers
FROM Players;
```

This schema provides a solid foundation for analytics, reporting, and business intelligence in the affiliate marketing platform.