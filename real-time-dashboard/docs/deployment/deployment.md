# Deployment Guide

This document outlines the steps and infrastructure components required to deploy the real-time pixel analytics platform using **Cloudflare’s serverless stack**.

---

## Infrastructure Components

| Component            | Platform         | Purpose                                     |
|----------------------|------------------|---------------------------------------------|
| **Cloudflare Workers** | Cloudflare Edge | Ingest pixel events and enqueue to queue    |
| **Cloudflare Queues**  | Cloudflare Edge | Buffer and decouple event processing        |
| **Cron Triggers**      | Cloudflare Edge | Periodically aggregate event metrics        |
| **Cloudflare D1**      | Cloudflare Edge | SQL-based event metric storage              |
| **Cloudflare Pages**   | Cloudflare Edge | Host React dashboard and Next.js API layer  |

---

## Prerequisites

- [Cloudflare Account](https://dash.cloudflare.com/)
- [Wrangler CLI](https://developers.cloudflare.com/workers/wrangler/install/)
- Node.js (v18 or newer)

---

## Project Structure

```bash
project-root/
│
├── ingestion-worker/        # Cloudflare Worker for handling pixel events
├── cron-worker/             # Worker with scheduled trigger for aggregation
├── api/               # Next.js API (Cloudflare Pages Functions)
├── dashboard/         # React frontend (Cloudflare Pages static site)
├── database/                # SQL schema for D1
└── wrangler.toml            # Worker and Pages configuration
```

---

## 🚀 Deployment Steps

### 1. Install Wrangler

```bash
npm install -g wrangler
```

### 2. Authenticate with Cloudflare

```bash
wrangler login
```

---

### 3. Deploy Workers

#### Ingestion Worker

```bash
cd ingestion-worker
wrangler deploy
```

#### Cron Aggregator

```bash
cd cron-worker
wrangler deploy
```

Ensure you have this in your `wrangler.toml` for scheduling:
```toml
[triggers]
crons = ["*5 * * * *"]  # every 5 min
```

---

### 4. Create & Configure Queue

```bash
wrangler queues create events_queue
```

Bind it in your `wrangler.toml`:
```toml
[[queues.producers]]
queue = "events_queue"
binding = "EVENTS_QUEUE"

[[queues.consumers]]
queue = "events_queue"
binding = "EVENTS_QUEUE"
```

---

### 5. Create Cloudflare D1 Database

```bash
wrangler d1 create analytics_db
```

Bind in `wrangler.toml`:
```toml
[[d1_databases]]
binding = "DB"
database_name = "analytics_db"
database_id = "<your-d1-id>"
```

Then push your schema:

```bash
wrangler d1 push analytics_db --file=./database/schema.sql
```

---

### 6. Deploy Cloudflare Pages

```bash
cd api
npm install
npm run build
```

Then deploy via:

```bash
wrangler pages deploy .output --project-name=pixel-analytics
```

Make sure Pages project is connected to the Git repo or deploy it manually through the Cloudflare Dashboard.

---

### 7. Test the System

- Simulate a pixel event:
```bash
curl https://your-domain.com/visit?playerId=123&bannerTag=abc
```

- Check D1 metrics:
```bash
wrangler d1 execute analytics_db --command "SELECT * FROM daily_metrics"
```

- Visit the dashboard:
```url
https://your-pages-domain.com
```

---

## Environment Variables (Optional)

Add secrets using:

```bash
wrangler secret put API_KEY
```

Use them in Workers:
```js
const key = env.API_KEY;
```

---

## Notes

- You can monitor logs with:
  ```bash
  wrangler tail
  ```
- Schedule database cleanup jobs for `events` if needed.
- Version lock your schema in a `database/schema.sql`.

---

## Deployment Complete

Your serverless real-time analytics system is now running on the Cloudflare Edge — fast, scalable, and fully managed.