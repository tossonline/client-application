# =============================================
# Grate Migration Script for Analytics Database
# Purpose: Run database migrations using Grate
# =============================================

param(
    [Parameter(Mandatory=$false)]
    [string]$ConnectionString = "Server=localhost;Database=dbAffiliate_Analytics;Integrated Security=true;TrustServerCertificate=true",
    
    [Parameter(Mandatory=$false)]
    [string]$Environment = "LOCAL",
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun,
    
    [Parameter(Mandatory=$false)]
    [switch]$Install
)

# Colors for output
$ErrorColor = "Red"
$SuccessColor = "Green"
$InfoColor = "Cyan"
$WarningColor = "Yellow"

Write-Host "Analytics Database Migration Script" -ForegroundColor $InfoColor
Write-Host "====================================" -ForegroundColor $InfoColor

# Check if Grate is installed
$grateInstalled = $false
try {
    $grateVersion = grate --version 2>$null
    if ($grateVersion) {
        Write-Host "Grate is installed: $grateVersion" -ForegroundColor $SuccessColor
        $grateInstalled = $true
    }
} catch {
    Write-Host "Grate is not installed or not in PATH" -ForegroundColor $WarningColor
}

# Install Grate if requested or not installed
if ($Install -or -not $grateInstalled) {
    Write-Host "Installing Grate..." -ForegroundColor $InfoColor
    try {
        dotnet tool install --global grate --version 1.6.0
        Write-Host "Grate installed successfully" -ForegroundColor $SuccessColor
    } catch {
        Write-Error "Failed to install Grate: $_" -ForegroundColor $ErrorColor
        exit 1
    }
}

# Set up paths
$scriptDir = $PSScriptRoot
$grateDir = Join-Path $scriptDir "grate"
$configFile = Join-Path $scriptDir "grate.yml"

Write-Host "Script Directory: $scriptDir" -ForegroundColor $InfoColor
Write-Host "Grate Directory: $grateDir" -ForegroundColor $InfoColor
Write-Host "Environment: $Environment" -ForegroundColor $InfoColor

# Check if grate directory exists
if (-not (Test-Path $grateDir)) {
    Write-Error "Grate directory not found: $grateDir" -ForegroundColor $ErrorColor
    exit 1
}

# Build Grate command
$grateArgs = @(
    "--connectionstring=`"$ConnectionString`""
    "--sqlfilesdirectory=`"$grateDir`""
    "--environment=$Environment"
    "--transaction"
    "--commandtimeout=300"
)

# Add dry run if specified
if ($DryRun) {
    $grateArgs += "--dryrun"
    Write-Host "Running in DRY RUN mode - no changes will be made" -ForegroundColor $WarningColor
}

# Add verbosity
$grateArgs += "--verbosity=Information"

Write-Host "Running Grate migration..." -ForegroundColor $InfoColor
Write-Host "Command: grate $($grateArgs -join ' ')" -ForegroundColor $InfoColor

try {
    # Run Grate
    $process = Start-Process -FilePath "grate" -ArgumentList $grateArgs -NoNewWindow -Wait -PassThru
    
    if ($process.ExitCode -eq 0) {
        Write-Host "Migration completed successfully!" -ForegroundColor $SuccessColor
        
        if (-not $DryRun) {
            Write-Host "Database 'dbAffiliate_Analytics' is ready for use." -ForegroundColor $SuccessColor
            Write-Host "You can now run your Analytics application." -ForegroundColor $InfoColor
        }
    } else {
        Write-Error "Migration failed with exit code: $($process.ExitCode)" -ForegroundColor $ErrorColor
        exit $process.ExitCode
    }
} catch {
    Write-Error "Failed to run Grate: $_" -ForegroundColor $ErrorColor
    exit 1
}

Write-Host "`nMigration Summary:" -ForegroundColor $InfoColor
Write-Host "- Tables: 8 (PixelEvents, Players, EventSummaries, Dashboards, CampaignMetrics, AnomalyDetections, GeoAnalytics, AnalyticsJobs)" -ForegroundColor $InfoColor
Write-Host "- Views: 3 (Campaign Performance, Player Funnel, Daily Metrics)" -ForegroundColor $InfoColor
Write-Host "- Stored Procedures: 3 (Upsert Events, Get Performance, Calculate Metrics)" -ForegroundColor $InfoColor
Write-Host "- Functions: 2 (Conversion Rate, Date Range)" -ForegroundColor $InfoColor
Write-Host "- Indexes: Comprehensive indexing for performance" -ForegroundColor $InfoColor