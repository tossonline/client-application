# Real-Time Analytics Platform

A high-performance, real-time analytics platform for tracking and analyzing user interactions, built with .NET 8 and Clean Architecture principles.

## Overview

The Real-Time Analytics Platform is designed to capture, process, and analyze user interactions in real-time. It provides:

- Real-time event tracking and processing
- Advanced analytics and metrics calculations
- Player segmentation and lifecycle management
- Customizable dashboards and visualizations
- Real-time alerts and notifications

## Domain Model

### Core Entities

#### PixelEvent
- **Purpose**: Captures raw user interactions from tracking pixels
- **Key Properties**:
  - `EventType`: Type of event (visit, registration, deposit)
  - `PlayerId`: Unique identifier of the player
  - `BannerTag`: Marketing banner identifier
  - `Metadata`: Additional event data
- **Behavior**:
  - Factory methods for different event types
  - Metadata management
  - Campaign ID extraction
  - Validation rules

#### EventSummary
- **Purpose**: Aggregates events for efficient analytics
- **Key Properties**:
  - `EventDate`: Date of the summary
  - `EventType`: Type of event summarized
  - `Count`: Total event count
  - `Period`: Aggregation period (hourly, daily, weekly, monthly)
- **Behavior**:
  - Increment/decrement counts
  - Merge summaries
  - Period-based aggregation

#### DailyMetric
- **Purpose**: Provides daily performance metrics
- **Key Properties**:
  - `VisitCount`: Total daily visits
  - `RegistrationCount`: Total daily registrations
  - `DepositCount`: Total daily deposits
  - `ConversionRate`: Registration/visit rate
  - `DepositRate`: Deposit/registration rate
  - `Trend`: Performance trend indicator
- **Behavior**:
  - Rate calculations
  - Trend analysis
  - Metric updates

#### Player
- **Purpose**: Manages player lifecycle and value
- **Key Properties**:
  - `Status`: Current player status
  - `Segment`: Player segment based on behavior
  - `TotalDeposits`: Number of deposits made
  - `TotalDepositAmount`: Total value of deposits
- **Behavior**:
  - Lifecycle management (registration, deposits)
  - Segment calculation
  - Value tracking
  - Activity monitoring

#### Dashboard
- **Purpose**: Configurable analytics visualization
- **Key Properties**:
  - `Type`: Dashboard category
  - `Layout`: Widget arrangement
  - `Configuration`: Dashboard settings
- **Behavior**:
  - Layout management
  - Widget configuration
  - Activation/deactivation

### Domain Services

#### Event Validation Service
- Validates event data
- Enriches events with additional context
- Applies business rules
- Ensures data quality

#### Metrics Calculation Service
- Calculates complex metrics
- Performs trend analysis
- Generates insights
- Supports real-time updates

#### Player Segmentation Service
- Manages player segments
- Applies segmentation rules
- Tracks segment transitions
- Generates recommendations

### Value Objects

#### TimePeriod
- Represents aggregation periods
- Supports: Hourly, Daily, Weekly, Monthly

#### TrendIndicator
- Represents metric trends
- Values: Increasing, Stable, Decreasing

#### PlayerStatus
- Represents player lifecycle stages
- Values: Visitor, Registered, Deposited

#### PlayerSegment
- Represents player value segments
- Values: New, Regular, VIP, NonDepositor, Inactive

## Architecture

The platform follows Clean Architecture principles:

```
src/
├── Domain/                 # Core domain entities and interfaces
│   ├── Entities/          # Domain entities
│   ├── Events/            # Domain events
│   ├── Services/          # Domain services
│   └── Repositories/      # Repository interfaces
│
├── Application/           # Application services and use cases
│   ├── Services/         # Application services
│   ├── Handlers/         # Command/Query handlers
│   └── Models/           # DTOs and view models
│
├── Infrastructure/        # External concerns and implementations
│   ├── Persistence/      # Database implementations
│   ├── Services/         # External service implementations
│   └── API/              # Web API controllers
│
└── Shared/               # Shared utilities and helpers
```

## Key Features

1. **Real-Time Event Processing**
   - Instant event capture and validation
   - Real-time metric updates
   - Live dashboard updates

2. **Advanced Analytics**
   - Conversion funnel analysis
   - Campaign performance tracking
   - Player value analysis
   - Trend detection

3. **Player Management**
   - Lifecycle tracking
   - Segmentation
   - Value monitoring
   - Behavior analysis

4. **Customizable Dashboards**
   - Multiple dashboard types
   - Flexible layouts
   - Real-time updates
   - Widget configuration

5. **Alerts & Notifications**
   - Real-time alerts
   - Performance monitoring
   - Threshold-based notifications
   - Trend alerts

## Getting Started

1. **Prerequisites**
   - .NET 8 SDK
   - PostgreSQL database
   - Redis (optional, for caching)

2. **Installation**
   ```bash
   git clone <repository-url>
   cd real-time-dashboard
   dotnet restore
   dotnet build
   ```

3. **Configuration**
   - Update `appsettings.json` with your database connection
   - Configure any external service API keys
   - Set up logging preferences

4. **Database Setup**
   ```bash
   dotnet ef database update
   ```

5. **Running the Application**
   ```bash
   dotnet run --project src/Analytics.Infrastructure.Service
   ```

## Documentation

- Introduction: docs/introduction/introduction.md
- Solution Overview: docs/solution/solution.md
- Application API: docs/application/application.md
- Deployment: docs/deployment/deployment.md
- Running Locally: docs/running-locally/running-locally.md
- External Dependencies: docs/external-dependencies/external-dependencies.md
- Product Requirements Document (PRD): docs/requirements/prd.md

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.