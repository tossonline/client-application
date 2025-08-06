# Running Locally

This guide explains how to run the Real-Time Pixel Analytics Platform locally for development, testing, and debugging purposes using an Kafka-based stack.

---

## Prerequisites

- Node.js (v24+)
- Docker & Docker Compose

---

## Project Structure Overview

```bash
project-root/
├── ingestion-worker/        # Service for handling pixel GET events
├── cron-worker/             # Service for aggregating metrics
├── api/                     # Next.js API + functions
├── dashboard/               # React-based frontend
├── database/schema.sql      # SQL schema
├── deploy/kafka/            # Kafka configuration
└── docker-compose.yml       # Docker Compose for local stack
```

---

## Local Setup

### 1. Install dependencies

```bash
cd ../api
npm install

cd ../dashboard
npm install
```

---

### 2. Start Local Workers

#### Ingestion Worker
```bash
cd ingestion-worker
docker build -t ingestion-worker .
docker run --network=host ingestion-worker
```

#### Cron Worker
```bash
cd ../cron-worker
docker build -t cron-worker .
docker run --network=host cron-worker
```

> Note: Cron logic may need to be triggered manually or mocked when running locally.

---

### 3. Run the API and Dashboard

#### API (Next.js Functions)
```bash
cd ../api
npm run dev
```

#### Frontend
```bash
cd ../dashboard
npm start
```

---

## Testing Pixel Calls

You can simulate incoming events with `curl`:

```bash
curl 'http://localhost:8787/visit?playerId=123&bannerTag=brandA'
curl 'http://localhost:8787/registration?playerId=123&bannerTag=brandA'
curl 'http://localhost:8787/deposit?playerId=123&bannerTag=brandA'
```

---

## Database Setup

You can use SQLite for local development, or configure your preferred SQL database. Example for SQLite:

```bash
sqlite3 ./database/analytics.db < ./database/schema.sql
```

---

## Ready to Develop!

You’re now set up to develop and test locally before deploying to your infrastructure.