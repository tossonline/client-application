# Product Requirements Document (PRD) — Real-Time Pixel Analytics Platform

Version: 1.0
Date: 2025-08-11
Owner: Analytics Platform Team
Stakeholders: Marketing (Affiliates), BI/Data, SRE/Platform, Security/Compliance, Engineering Leads

## 1. Vision and Goals
- Problem: Batch BI causes 6–8 hour delays in visibility of visits/registrations/deposits.
- Vision: Provide near real-time funnel and campaign insights within our infrastructure.
- Goals:
  - Real-time ingestion of pixel events and durable publish to Kafka
  - Minute-level aggregated metrics and trends via APIs and dashboards
  - Resilient, observable services aligned to Clean Architecture

## 2. Users and Personas
- Marketing/Affiliate Managers: monitor funnel, campaign performance, anomalies
- Operations: health, traffic spikes, error rates
- BI/Data: export/validate aggregates, reconcile with DWH
- Engineering/SRE: maintain health, scale, reliability

## 3. Scope
### In Scope
- Pixel endpoints (visit, registration, deposit), validation and enrichment
- Publish to Kafka with schema enforcement and DLQ handling
- Aggregations: daily metrics, summaries, trends, conversion rates
- Metrics API for dashboards
- Observability: health, metrics, logs, traces; Grafana/Prometheus integration
- On‑prem K8s deployments (dev/stage/prod) with SQL Server and Kafka

### Out of Scope (Phase 1)
- Cloudflare Workers/Pages/D1 stack
- Predictive analytics / ML anomaly detection
- Advanced multi-tenant authorization

## 4. Success Metrics (KPIs)
- Data latency: p95 end‑to‑end (pixel → aggregated metric) ≤ 60s
- Ingestion durability: 0 data loss for acknowledged requests
- Availability: API SLO 99.9% monthly; ingestion 99.95% monthly
- Throughput: sustain ≥ 2k events/sec; burst ≥ 5k events/sec for 5 min

## 5. Functional Requirements
### 5.1 Pixel Ingestion API
- Endpoints
  - GET `/api/pixel/visit`
  - GET `/api/pixel/registration`
  - GET `/api/pixel/deposit`
- Query params: `playerId` (required), `bannerTag` (required), `s` (optional), `b` (optional)
- Behavior
  - Validate inputs; enrich with `user-agent`, `ip`; timestamp server-side
  - Publish to Kafka; respond 200 on acceptance, 4xx on validation errors, 5xx on failures

### 5.2 Kafka Publish & DLQ
- Topic naming (env-specific), e.g. `EVT_{REGION}_{ORG}_{ENV}_AAP_EXT_PIXEL-ANALYTICS_PIXEL_EVENTS`
- Partitions: default 10; replication ≥ 3 (ISR ≥ 2); retention.ms raw: 3 days
- Compression: producer; compatibility: forward (Avro)
- DLQ topic `DLQ_{...}_PIXEL_EVENTS` for poison/failed events; alert on elevated DLQ rates

### 5.3 Aggregation & Metrics
- Consume from Kafka; idempotent aggregation (dedupe by natural key per day & event type)
- Tables: `events` (optional/raw), `daily_metrics`, `event_summary`, `players`
- Derived metrics: conversion rates, trend indicators

### 5.4 Metrics API
- GET `/api/metrics/conversion-rates?startDate&endDate`
- GET `/api/metrics/campaigns/{bannerTag}?startDate&endDate`
- GET `/api/metrics/trends/{eventType}?startDate&endDate`
- Validate ranges; paginate timeseries responses if large

### 5.5 Observability & Admin
- Health: liveness/readiness (DB, Kafka checks)
- Metrics: ingestion RTT, consumer lag, DLQ rate, API latency p50/p95/p99, error rate
- Traces: end‑to‑end spans; correlation IDs
- Config via `appsettings.*.json` + env; feature flags for metrics/topics/retention
- Grafana provisioning from repo; Prometheus scrape config in repo

## 6. Non‑Functional Requirements
- Performance: controller→Kafka ack p95 ≤ 200ms; aggregation p95 ≤ 60s
- Scalability: horizontal scale of API/consumers; partition-aware; ≥10 partitions (tunable)
- Reliability: retries with backoff; idempotency; DLQ for poison; at‑least once semantics with idempotent sinks
- Security: TLS; least-privilege service accounts for Kafka/DB; secrets via K8s; audit logs
- Compliance: avoid PII in metadata; retention raw 3d, aggregates 18m (configurable); subject deletion by `playerId` best‑effort
- Operability: structured logs; dashboards and alerts live in `deploy/grafana/**`

## 7. Data Model (Summary)
- `events(id, player_id, event_type, banner_tag, metadata_json, source_ip, user_agent, created_at)`
- `daily_metrics(event_date, event_type, count)`
- `event_summary(event_date, event_type, banner_tag, count)`
- `players(player_id, first_seen, last_event_at, registration_at, deposit_at, total_deposits)`

## 8. Environments & Dependencies
- DEV: Orange K8s; Kafka CPTKDEVDISCO; SQL Server 
- STAGE: Blue K8s; Kafka WHLKDISCO; SQL 
- PROD: Green AZ1; Kafka WHLKPARDISCO; SQL 
- Observability: Grafana/Prometheus provisioning in `deploy/grafana/**`, `deploy/prometheus/**`

## 9. Risks & Mitigations
- Under‑partitioned topics → capacity testing; increase partitions with plan
- Schema evolution breaks → enforce forward compat; contract tests
- Metadata PII leakage → allowlist metadata keys; validation and logging redaction
- Duplicate events → idempotent keys; dedupe logic

## 10. Acceptance Criteria
- Pixel endpoints 200 on valid, publish to Kafka; 400 on missing fields; 5xx on failures
- Aggregations match synthetic loads; no double count
- Metrics API returns correct structures and values for seeded ranges
- Health checks green; Grafana dashboards/alerts operational
- Load test: ≥2k events/sec with SLO adherence; DLQ rate < 0.1%

## 11. Timeline (Indicative)
- W1–2: Topic/schema finalization; DLQ; validation allow/deny lists; docs
- W3–4: Aggregator hardening; trend metrics; API pagination; dashboards
- W5: Perf tests and tuning; SRE review; staging rollout
- W6: Production canary; SLO monitoring and adjustments

## 12. References
- ADRs: `docs/adr/`
- Deploy: `deploy/k8s/**`, `deploy/grafana/**`, `deploy/prometheus/**`, `deploy/kafka/**`
- Database: `database/**`, `Analytics.Infrastructure.Persistence/**`


