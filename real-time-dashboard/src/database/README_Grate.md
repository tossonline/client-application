# Analytics Database Migration with Grate

This project uses [Grate](https://erikbra.github.io/grate/) for database migrations. Grate is a SQL-first database migration tool that organizes SQL scripts in specific folders and provides versioning and change tracking.

## Why Grate?

- **SQL-First**: Write migrations in pure SQL
- **Folder-Based**: Organized structure for different types of database objects
- **Versioning**: Automatic change tracking and versioning
- **Idempotent**: Views, stored procedures, and functions are recreated on each run
- **Cross-Platform**: Works on Windows, macOS, and Linux
- **CI/CD Friendly**: Easy integration with build pipelines

## Project Structure

```
database/
??? grate/
?   ??? up/                           # One-time migration scripts (versioned)
?   ?   ??? 0001_CreatePixelEventsTable.sql
?   ?   ??? 0002_CreatePlayersTable.sql
?   ?   ??? 0003_CreateEventSummariesTable.sql
?   ?   ??? 0004_CreateDashboardsTable.sql
?   ?   ??? 0005_CreateCampaignMetricsTable.sql
?   ?   ??? 0006_CreateAnomalyDetectionsTable.sql
?   ?   ??? 0007_CreateGeoAnalyticsTable.sql
?   ?   ??? 0008_CreateAnalyticsJobsTable.sql
?   ?   ??? 0009_CreateIndexes.sql
?   ??? views/                        # Database views (recreated each run)
?   ?   ??? vw_CampaignPerformanceSummary.sql
?   ?   ??? vw_PlayerFunnelAnalysis.sql
?   ?   ??? vw_DailyMetricsSummary.sql
?   ??? sprocs/                       # Stored procedures (recreated each run)
?   ?   ??? sp_UpsertEventSummary.sql
?   ?   ??? sp_GetCampaignPerformance.sql
?   ?   ??? sp_CalculateCampaignMetrics.sql
?   ??? functions/                    # User-defined functions (recreated each run)
?   ?   ??? fn_CalculateConversionRate.sql
?   ?   ??? fn_GetDateRange.sql
?   ??? runAfterCreateDatabase/       # Scripts run after database creation
?   ?   ??? 0001_InitialSeedData.sql
?   ??? runBeforeUp/                  # Scripts run before migrations
?       ??? 0001_DatabaseSettings.sql
??? grate.yml                         # Grate configuration file
??? RunMigration.ps1                  # PowerShell script to run migrations
??? README.md                         # This file
```

## Quick Start

### 1. Install Grate

**Option A: Using the PowerShell script (Recommended)**
```powershell
.\database\RunMigration.ps1 -Install
```

**Option B: Manual installation**
```bash
# Install globally using .NET CLI
dotnet tool install --global grate

# Verify installation
grate --version
```

### 2. Configure Connection String

Edit the connection string in `grate.yml` or pass it as a parameter:

```yaml
connectionString: "Server=localhost;Database=dbAffiliate_Analytics;Integrated Security=true;TrustServerCertificate=true"
```

### 3. Run Migrations

**Using PowerShell script (Recommended):**
```powershell
# Run migrations with default settings
.\database\RunMigration.ps1

# Run with custom connection string
.\database\RunMigration.ps1 -ConnectionString "Server=myserver;Database=dbAffiliate_Analytics;User Id=myuser;Password=mypass;"

# Run in different environment
.\database\RunMigration.ps1 -Environment "DEVELOPMENT"

# Dry run (preview changes without applying)
.\database\RunMigration.ps1 -DryRun
```

**Using Grate CLI directly:**
```bash
# Navigate to the database directory
cd database

# Run migrations
grate --connectionstring="Server=localhost;Database=dbAffiliate_Analytics;Integrated Security=true" --sqlfilesdirectory="./grate"
```

## Database Schema

### Core Tables

| Table | Purpose | Records |
|-------|---------|---------|
| **PixelEvents** | Raw pixel tracking events | High volume |
| **Players** | User lifecycle tracking | Medium volume |
| **EventSummaries** | Daily aggregated metrics | Low volume |
| **Dashboards** | Dashboard configurations | Very low volume |

### Analytics Tables

| Table | Purpose | Records |
|-------|---------|---------|
| **CampaignMetrics** | Pre-calculated campaign performance | Medium volume |
| **AnomalyDetections** | Statistical anomaly alerts | Low volume |
| **GeoAnalytics** | Geographic distribution data | Low volume |
| **AnalyticsJobs** | Background job tracking | Low volume |

### Database Objects

- **3 Views**: Campaign performance, player funnel, daily metrics
- **3 Stored Procedures**: Event upserts, performance queries, metric calculations
- **2 Functions**: Conversion rate calculations, date range utilities
- **Comprehensive Indexes**: Optimized for analytics queries

## Migration Workflow

### 1. One-Time Scripts (`up/` folder)
- Run only once per database
- Numbered for execution order (0001, 0002, etc.)
- Create tables, add columns, data migrations
- **Never modify existing scripts** - create new ones

### 2. Repeatable Scripts
- **Views** (`views/`): Recreated on every run
- **Stored Procedures** (`sprocs/`): Recreated on every run  
- **Functions** (`functions/`): Recreated on every run
- Can be modified and will update on next migration

### 3. Special Folders
- **runBeforeUp**: Database settings, pre-migration setup
- **runAfterCreateDatabase**: Initial seed data, default records

## Development Workflow

### Adding a New Table
1. Create a new script in `up/` folder with next number: `0010_CreateNewTable.sql`
2. Add indexes in the same script or create `0011_CreateNewTableIndexes.sql`
3. Run migration: `.\RunMigration.ps1`

### Modifying a View/Procedure
1. Edit the existing file in `views/` or `sprocs/` folder
2. Run migration: `.\RunMigration.ps1`
3. The object will be dropped and recreated

### Adding Sample Data
1. Add INSERT statements to `runAfterCreateDatabase/0001_InitialSeedData.sql`
2. Or create a new script: `runAfterCreateDatabase/0002_AddSampleData.sql`

## Environment Management

Grate supports different environments for different deployment stages:

```powershell
# Development
.\RunMigration.ps1 -Environment "DEVELOPMENT"

# Testing  
.\RunMigration.ps1 -Environment "TEST"

# Production
.\RunMigration.ps1 -Environment "PRODUCTION"
```

Environment-specific scripts can be created by adding the environment suffix:
- `0001_CreateTable.DEVELOPMENT.sql` (only runs in DEVELOPMENT)
- `sp_GetData.TEST.sql` (only runs in TEST)

## CI/CD Integration

### Azure DevOps Pipeline
```yaml
- task: PowerShell@2
  displayName: 'Run Database Migration'
  inputs:
    filePath: 'database/RunMigration.ps1'
    arguments: '-ConnectionString "$(DatabaseConnectionString)" -Environment "$(Environment)"'
```

### GitHub Actions
```yaml
- name: Run Database Migration
  run: |
    ./database/RunMigration.ps1 -ConnectionString "${{ secrets.DATABASE_CONNECTION_STRING }}" -Environment "${{ github.ref_name }}"
  shell: pwsh
```

### Docker
```dockerfile
# Install Grate in container
RUN dotnet tool install --global grate

# Copy migration scripts
COPY database/grate /app/database/grate

# Run migrations
RUN grate --connectionstring="$CONNECTION_STRING" --sqlfilesdirectory="/app/database/grate"
```

## Best Practices

### 1. Migration Scripts
- **Never modify existing `up/` scripts** - they are versioned
- Use descriptive names: `0010_AddUserPreferencesTable.sql`
- Include rollback instructions in comments
- Test scripts on a copy of production data

### 2. Stored Procedures and Views
- Document parameters and return values
- Use consistent naming conventions
- Include error handling in stored procedures
- Optimize for performance with proper indexing

### 3. Version Control
- Commit migration scripts with application code
- Review all database changes in pull requests
- Tag releases that include schema changes

### 4. Environments
- Always test migrations in development first
- Use identical schema across all environments
- Backup production before running migrations

## Troubleshooting

### Common Issues

**1. Grate not found**
```
Solution: Install Grate using: dotnet tool install --global grate
```

**2. Permission denied**
```
Solution: Ensure database user has CREATE, ALTER, DROP permissions
```

**3. Script failed to run**
```
Solution: Check the Grate output for specific SQL errors
Use --dryrun to preview changes without applying them
```

**4. Connection string issues**
```
Solution: Test connection string with SQL Server Management Studio first
Ensure server name, credentials, and database name are correct
```

### Logging and Debugging

Enable verbose logging:
```powershell
.\RunMigration.ps1 -Verbosity Debug
```

Dry run to preview changes:
```powershell
.\RunMigration.ps1 -DryRun
```

## Monitoring

Grate creates tracking tables to monitor migrations:
- **RoundhousE**: Tracks script execution history
- **ScriptsRun**: Details of each script execution
- **ScriptsRunErrors**: Error details for failed scripts

Query execution history:
```sql
SELECT * FROM RoundhousE ORDER BY entry_date DESC;
SELECT * FROM ScriptsRun ORDER BY entry_date DESC;
```

## Support

- **Grate Documentation**: https://erikbra.github.io/grate/
- **GitHub Repository**: https://github.com/erikbra/grate
- **Analytics Team**: Contact for database-specific questions

---

**Next Steps:**
1. Install Grate: `.\RunMigration.ps1 -Install`
2. Run initial migration: `.\RunMigration.ps1`
3. Verify database: Connect and check tables/views
4. Start developing: Your Analytics application is ready!