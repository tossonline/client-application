# Introduction

## What This Platform Does


This platform enables **real-time tracking and visualization of web events**—specifically visits, registrations, and deposits—across various digital properties. It integrates seamlessly with existing front-end tracking mechanisms (`rtpx.emit(...)`), captures events at the edge using **event ingestion services**, and routes them through **Kafka message queues** for reliable, scalable event processing.

The system powers a centralized dashboard that allows business users and technical teams to analyze user activity and performance metrics **within seconds**, rather than hours, all within your own infrastructure.

---

## Problem Statement

Traditional reporting workflows rely on **batch processing** by Business Intelligence (BI) teams, often resulting in a **6–8 hour delay** before web or campaign activity is visible. This delay:

- Obstructs rapid detection of spikes, drop-offs, or unusual trends.
- Slows down marketing optimization.
- Leaves stakeholders without **real-time insight** into the performance of their brand websites and user funnels.

---

## Solution Overview


This platform replaces batch-based pipelines with a **real-time, event streaming architecture**:

1. **Pixel events** (visit, registration, deposit) are emitted from websites using a lightweight JavaScript interface.
2. **ingestion services** (see `ingestion-worker/`) capture and normalize these events with minimal latency.
3. Events are **pushed into Kafka message queues** (see `deploy/kafka/`), which act as a durable, decoupled buffer.
4. A **scheduled aggregation service** (see `cron-worker/`) consumes the queue and **aggregates metrics into a local SQL database** (see `database/schema.sql`).
5. A **Next.js + React dashboard** (see `dashboard/`), running on-premises, queries the data and renders it live for business users.

This architecture is:

- **Fully and self-managed**
- **Low-latency, scalable within your infrastructure**
- **Resilient to traffic spikes**
- **No reliance on external cloud providers**

---

## Benefits

- **True real-time analytics** within your own infrastructure.
- **Immediate insights** for marketing, affiliate, and operations teams.
- **Simplified architecture** using open-source and tools—no vendor lock-in.
- **Massively scalable** while keeping infrastructure under your control.