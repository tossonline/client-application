# dbAffiliate_Analytics - Grate Database Migrations

This folder contains the database migration scripts for the **Analytics Platform (.NET 8)** using [Grate](https://erikbra.github.io/grate/) for the `dbAffiliate_Analytics` database.

## ?? Folder Structure (Grate Script Types)

Following the [official Grate script types](https://erikbra.github.io/grate/script-types/):

```
src/dbAffiliate_Analytics/
??? runBeforeUp/              # Scripts run BEFORE all migrations
?   ??? 0001_DatabaseSettings.sql
??? up/                       # One-time migration scripts (versioned)
?   ??? 0001_CreatePixelEventsTable.sql
?   ??? 0002_CreatePlayersTable.sql
?   ??? 0003_CreateEventSummariesTable.sql
?   ??? 0004_CreateDashboardsTable.sql
?   ??? 0005_CreateCampaignMetricsTable.sql
?   ??? 0006_CreateAnomalyDetectionsTable.sql
?   ??? 0007_CreateGeoAnalyticsTable.sql
?   ??? 0008_CreateAnalyticsJobsTable.sql
?   ??? 0009_CreateIndexes.sql
??? views/                    # Database views (recreated each run)
?   ??? vw_CampaignPerformanceSummary.sql
?   ??? vw_PlayerFunnelAnalysis.sql
?   ??? vw_DailyMetricsSummary.sql
??? sprocs/                   # Stored procedures (recreated each run)
?   ??? sp_UpsertEventSummary.sql
?   ??? sp_GetCampaignPerformance.sql
?   ??? sp_CalculateCampaignMetrics.sql
??? functions/                # User-defined functions (recreated each run)
?   ??? fn_CalculateConversionRate.sql
?   ??? fn_GetDateRange.sql
??? runAfterCreateDatabase/   # Scripts run AFTER database creation
?   ??? 0001_InitialSeedData.sql
??? grate.yml                 # Grate configuration
??? RunMigration.ps1          # Migration execution script
??? README.md                 # This file
```

## ?? Analytics Platform Integration

This database schema supports the following **.NET 8 Domain Aggregates**:

### ??? **CampaignAggregate**
- **Tables**: EventSummaries, CampaignMetrics
- **Views**: vw_CampaignPerformanceSummary
- **Procedures**: sp_GetCampaignPerformance, sp_CalculateCampaignMetrics
- **Purpose**: Campaign performance tracking and analysis

### ?? **PlayerAggregate** 
- **Tables**: Players
- **Views**: vw_PlayerFunnelAnalysis
- **Purpose**: Player lifecycle and conversion tracking

### ?? **PixelEventAggregate**
- **Tables**: PixelEvents
- **Purpose**: Raw event tracking and processing

### ?? **MetricsAggregate**
- **Tables**: EventSummaries, CampaignMetrics
- **Views**: vw_DailyMetricsSummary
- **Procedures**: sp_UpsertEventSummary
- **Purpose**: Metrics calculation and aggregation

### ?? **AnalyticsAggregate**
- **Tables**: AnomalyDetections, GeoAnalytics
- **Functions**: fn_CalculateConversionRate, fn_GetDateRange
- **Purpose**: Advanced analytics, anomaly detection, geographic analysis

## ?? Quick Start

### 1. Install Grate
```powershell
# Navigate to the database folder
cd src\dbAffiliate_Analytics

# Install Grate and run migration
.\RunMigration.ps1 -Install
```

### 2. Run Migrations
```powershell
# Basic migration
.\RunMigration.ps1

# With custom connection string
.\RunMigration.ps1 -ConnectionString "Server=myserver;Database=dbAffiliate_Analytics;User Id=myuser;Password=mypass;"

# Different environment
.\RunMigration.ps1 -Environment "DEVELOPMENT"

# Dry run (preview only)
.\RunMigration.ps1 -DryRun
```

### 3. Verify Database
```sql
USE dbAffiliate_Analytics;

-- Check tables
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';

-- Check views
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS;

-- Check procedures
SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE';
```

## ?? Database Schema Overview

### Core Tables
| Table | Purpose | Used By Aggregate |
|-------|---------|-------------------|
| **PixelEvents** | Raw event tracking | PixelEventAggregate |
| **Players** | Player lifecycle | PlayerAggregate |
| **EventSummaries** | Daily metrics | CampaignAggregate, MetricsAggregate |
| **Dashboards** | UI configuration | All aggregates |

### Analytics Tables
| Table | Purpose | Used By Aggregate |
|-------|---------|-------------------|
| **CampaignMetrics** | Campaign performance | CampaignAggregate |
| **AnomalyDetections** | Statistical anomalies | AnalyticsAggregate |
| **GeoAnalytics** | Geographic data | AnalyticsAggregate |
| **AnalyticsJobs** | Background jobs | All aggregates |

### Views & Procedures
- **3 Views**: Optimized for aggregate queries
- **3 Stored Procedures**: Data operations and calculations  
- **2 Functions**: Utility functions for analytics

## ?? Development Workflow

### Adding New Tables
1. Create new script in `up/` folder: `0010_CreateNewTable.sql`
2. Run migration: `.\RunMigration.ps1`

### Modifying Views/Procedures
1. Edit file in `views/` or `sprocs/` folder
2. Run migration: `.\RunMigration.ps1` (automatically recreated)

### Environment-Specific Scripts
Create environment-specific scripts:
- `0001_CreateTable.DEVELOPMENT.sql` (DEVELOPMENT only)
- `sp_GetData.PRODUCTION.sql` (PRODUCTION only)

## ??? CI/CD Integration

### Azure DevOps
```yaml
- task: PowerShell@2
  displayName: 'Run Analytics Database Migration'
  inputs:
    filePath: 'src/dbAffiliate_Analytics/RunMigration.ps1'
    arguments: '-ConnectionString "$(DatabaseConnectionString)" -Environment "$(Environment)"'
```

### GitHub Actions
```yaml
- name: Run Analytics Database Migration
  run: |
    ./src/dbAffiliate_Analytics/RunMigration.ps1 -ConnectionString "${{ secrets.DATABASE_CONNECTION_STRING }}" -Environment "${{ github.ref_name }}"
  shell: pwsh
```

## ?? Performance Optimization

The schema includes comprehensive indexing optimized for:
- **High-volume event ingestion** (PixelEvents)
- **Campaign performance queries** (CampaignAggregate)
- **Player analytics** (PlayerAggregate)
- **Real-time metrics** (MetricsAggregate)
- **Advanced analytics** (AnalyticsAggregate)

## ?? Security & Compliance

- **Data encryption** at rest supported
- **Audit fields** on all tables (CreatedAt, UpdatedAt)
- **Input validation** via check constraints
- **Permission management** via Grate permissions folder

## ?? Monitoring

Query Grate tracking tables:
```sql
-- View migration history
SELECT * FROM RoundhousE ORDER BY entry_date DESC;

-- Check script execution
SELECT * FROM ScriptsRun ORDER BY entry_date DESC;

-- View any errors
SELECT * FROM ScriptsRunErrors ORDER BY entry_date DESC;
```

## ?? Troubleshooting

### Common Issues

1. **Grate not found**: Install with `dotnet tool install --global grate`
2. **Permission denied**: Ensure database user has CREATE/ALTER/DROP permissions
3. **Connection issues**: Test connection string with SSMS first
4. **Script errors**: Use `--dryrun` to preview changes

### Getting Help
- **Grate Docs**: https://erikbra.github.io/grate/
- **Script Types**: https://erikbra.github.io/grate/script-types/
- **Analytics Platform**: See domain aggregate documentation

---

## ? Ready for Production

This database schema is optimized for the **.NET 8 Analytics Platform** with proper:
- ? **Domain Aggregate Support**
- ? **High-Performance Indexing** 
- ? **Grate Migration Management**
- ? **CI/CD Integration**
- ? **Comprehensive Documentation**

Run `.\RunMigration.ps1` to get started! ??