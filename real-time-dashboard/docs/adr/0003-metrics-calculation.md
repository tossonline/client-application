# ADR 3: Metrics Calculation Strategy

## Status

Accepted

## Context

The Real-Time Analytics Platform needs to calculate various metrics efficiently while:
- Supporting real-time updates
- Maintaining historical data
- Providing accurate trends
- Scaling with data volume
- Supporting custom metrics

We needed to decide on strategies for calculating, storing, and updating metrics.

## Decision

We decided to implement a multi-level metrics calculation strategy:

1. **Real-Time Metrics**
   - Simple counts and rates
   - Current day metrics
   - Player-specific metrics
   - Dashboard KPIs

2. **Aggregated Metrics**
   - Daily summaries
   - Trend calculations
   - Complex analytics
   - Historical data

3. **Custom Metrics**
   - User-defined calculations
   - Flexible formulas
   - Derived metrics
   - Composite KPIs

Key implementation patterns:

1. **Calculation Hierarchy**
   ```plaintext
   Raw Events → Basic Metrics → Aggregated Metrics → Derived Metrics
   ```

2. **Update Strategies**
   - Immediate updates for real-time
   - Scheduled updates for aggregates
   - On-demand for custom metrics
   - Cached calculations

3. **Storage Approach**
   - Hot data in memory
   - Warm data in database
   - Cold data archived
   - Efficient querying

## Consequences

### Positive

1. **Performance**
   - Fast real-time updates
   - Efficient calculations
   - Optimized storage
   - Quick retrieval

2. **Flexibility**
   - Custom metrics support
   - Multiple time periods
   - Various aggregations
   - Different views

3. **Scalability**
   - Horizontal scaling
   - Resource optimization
   - Load distribution
   - Cache utilization

4. **Accuracy**
   - Consistent calculations
   - Verifiable results
   - Audit support
   - Error detection

### Negative

1. **Complexity**
   - Multiple calculation paths
   - Cache management
   - Data synchronization
   - Error handling

2. **Resource Usage**
   - Memory overhead
   - Processing power
   - Storage space
   - Network bandwidth

3. **Maintenance**
   - Complex testing
   - Performance tuning
   - Bug fixing
   - Documentation

## Alternatives Considered

1. **Pure Real-Time**
   - Simple implementation
   - High resource usage
   - Performance issues
   - Limited history

2. **Batch Only**
   - Efficient processing
   - Limited real-time
   - High latency
   - Simple implementation

3. **Stream Processing**
   - Real-time capable
   - Complex setup
   - Resource intensive
   - Future consideration

## Implementation Notes

1. **Metric Calculation**
   ```csharp
   public class MetricsCalculator
   {
       public async Task<DailyMetric> CalculateDailyMetricsAsync(
           DateTime date,
           string eventType)
       {
           var events = await GetDailyEvents(date, eventType);
           var metric = DailyMetric.Create(date, eventType);

           metric.UpdateVisitCount(events.Count(e => e.EventType == "visit"));
           metric.UpdateRegistrationCount(events.Count(e => e.EventType == "registration"));
           metric.UpdateDepositCount(events.Count(e => e.EventType == "deposit"));

           var previousDay = await GetPreviousDayMetrics(date, eventType);
           metric.UpdateTrend(previousDay);

           return metric;
       }
   }
   ```

2. **Custom Metrics**
   ```csharp
   public class CustomMetricCalculator
   {
       public async Task<decimal> CalculateCustomMetricAsync(
           CustomMetricDefinition definition,
           DateTime startDate,
           DateTime endDate)
       {
           var baseMetrics = await GetBaseMetrics(
               definition.Dependencies,
               startDate,
               endDate);

           return definition.Formula.Calculate(baseMetrics);
       }
   }
   ```

3. **Caching Strategy**
   ```csharp
   public class MetricsCache
   {
       private readonly IMemoryCache _memoryCache;
       private readonly IDistributedCache _distributedCache;

       public async Task<T> GetOrCalculateAsync<T>(
           string key,
           Func<Task<T>> calculator,
           TimeSpan? slidingExpiration = null)
       {
           // Try memory cache
           if (_memoryCache.TryGetValue(key, out T value))
               return value;

           // Try distributed cache
           var cached = await _distributedCache.GetAsync(key);
           if (cached != null)
               return Deserialize<T>(cached);

           // Calculate and cache
           value = await calculator();
           await CacheValueAsync(key, value, slidingExpiration);
           return value;
       }
   }
   ```

## Future Considerations

1. **Advanced Analytics**
   - Machine learning integration
   - Predictive metrics
   - Anomaly detection
   - Pattern recognition

2. **Performance Optimization**
   - Query optimization
   - Cache strategies
   - Data partitioning
   - Parallel processing

3. **New Features**
   - Real-time alerts
   - Custom dashboards
   - Metric correlations
   - Advanced reporting

4. **Integration**
   - External data sources
   - Third-party analytics
   - Data export
   - API access
