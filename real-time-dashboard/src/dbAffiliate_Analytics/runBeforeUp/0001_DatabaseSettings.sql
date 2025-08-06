-- =============================================
-- Database Settings
-- Purpose: Set up database-level configuration and settings
-- =============================================

-- Set database collation and options
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'dbAffiliate_Analytics')
BEGIN
    PRINT 'Database dbAffiliate_Analytics does not exist. This script should run after database creation.';
    RETURN;
END

-- Set database options for performance and reliability
ALTER DATABASE [dbAffiliate_Analytics] SET AUTO_CLOSE OFF;
ALTER DATABASE [dbAffiliate_Analytics] SET AUTO_SHRINK OFF;
ALTER DATABASE [dbAffiliate_Analytics] SET AUTO_CREATE_STATISTICS ON;
ALTER DATABASE [dbAffiliate_Analytics] SET AUTO_UPDATE_STATISTICS ON;
ALTER DATABASE [dbAffiliate_Analytics] SET AUTO_UPDATE_STATISTICS_ASYNC ON;
ALTER DATABASE [dbAffiliate_Analytics] SET PARAMETERIZATION SIMPLE;
ALTER DATABASE [dbAffiliate_Analytics] SET READ_COMMITTED_SNAPSHOT ON;

-- Set recovery model to FULL for production (can be changed to SIMPLE for development)
ALTER DATABASE [dbAffiliate_Analytics] SET RECOVERY FULL;

-- Set page verify to CHECKSUM for data integrity
ALTER DATABASE [dbAffiliate_Analytics] SET PAGE_VERIFY CHECKSUM;

-- Enable snapshot isolation
ALTER DATABASE [dbAffiliate_Analytics] SET ALLOW_SNAPSHOT_ISOLATION ON;

PRINT 'Database settings configured successfully';