# Grate Database Migration Structure for dbAffiliate_Analytics

This project uses [Grate](https://erikbra.github.io/grate/) for database migrations. Grate is a SQL-first database migration tool that organizes SQL scripts in specific folders.

## Folder Structure

```
database/
??? grate/
?   ??? up/
?   ?   ??? 0001_CreatePixelEventsTable.sql
?   ?   ??? 0002_CreatePlayersTable.sql
?   ?   ??? 0003_CreateEventSummariesTable.sql
?   ?   ??? 0004_CreateDashboardsTable.sql
?   ?   ??? 0005_CreateCampaignMetricsTable.sql
?   ?   ??? 0006_CreateAnomalyDetectionsTable.sql
?   ?   ??? 0007_CreateGeoAnalyticsTable.sql
?   ?   ??? 0008_CreateAnalyticsJobsTable.sql
?   ?   ??? 0009_CreateIndexes.sql
?   ??? views/
?   ?   ??? vw_CampaignPerformanceSummary.sql
?   ?   ??? vw_PlayerFunnelAnalysis.sql
?   ?   ??? vw_DailyMetricsSummary.sql
?   ??? sprocs/
?   ?   ??? sp_UpsertEventSummary.sql
?   ?   ??? sp_GetCampaignPerformance.sql
?   ?   ??? sp_CalculateCampaignMetrics.sql
?   ??? functions/
?   ?   ??? fn_CalculateConversionRate.sql
?   ?   ??? fn_GetDateRange.sql
?   ??? runAfterCreateDatabase/
?   ?   ??? 0001_InitialSeedData.sql
?   ??? runBeforeUp/
?       ??? 0001_DatabaseSettings.sql
??? README.md
```

## Grate Configuration

Create a `grate.yml` configuration file:

```yaml
# grate.yml
database: "dbAffiliate_Analytics"
connectionString: "Server=localhost;Database=dbAffiliate_Analytics;Integrated Security=true;TrustServerCertificate=true"
sqlFilesDirectory: "./database/grate"
schema: "dbo"
environment: "LOCAL"
transaction: true
commandTimeout: 300
```

## Usage

Run migrations using Grate CLI:

```bash
# Install Grate globally
dotnet tool install --global grate

# Run migrations
grate --connectionstring="Server=localhost;Database=dbAffiliate_Analytics;Integrated Security=true" --sqlfilesdirectory="./database/grate"
```

## Migration Scripts

All migration scripts are organized by Grate's folder conventions:

- **up/**: One-time migration scripts (numbered for order)
- **views/**: Database views (recreated on each run)
- **sprocs/**: Stored procedures (recreated on each run)
- **functions/**: User-defined functions (recreated on each run)
- **runAfterCreateDatabase/**: Scripts to run after database creation
- **runBeforeUp/**: Scripts to run before migrations