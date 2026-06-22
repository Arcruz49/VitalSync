# VitalSync

A personal health monitoring platform that combines metric tracking, AI-powered insights, and nutrition analysis in a single application.

---

## Overview

VitalSync allows users to log health metrics such as blood pressure, glucose, weight, and heart rate, and receive AI-generated analysis after each entry. The platform calculates personalized normal ranges based on the user's profile, health conditions, and medications, and raises alerts when readings fall outside those ranges. A dedicated nutrition module lets users photograph meals and automatically receive a macronutrient breakdown powered by a vision AI model.

AI analysis runs asynchronously: after a health record or nutrition entry is saved, the API publishes an event to RabbitMQ. A dedicated AI microservice consumes the event, calls the Claude API, and publishes the result back. The API then persists the insight or updates the nutrition record — all without blocking the original request.

---

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 10, Entity Framework Core, Npgsql |
| AI Service | ASP.NET Core 10, MassTransit, Anthropic SDK |
| Message Broker | RabbitMQ |
| Database | PostgreSQL |
| Frontend | Angular 19, Standalone Components, Signals |
| AI | Anthropic Claude API (health insights + food image analysis) |
| Auth | JWT via HttpOnly cookie |
| Infrastructure | Docker, Docker Compose |

---

## Features

**Health Records**
- Log readings for 12+ metric types (blood pressure, glucose, weight, BMI, SpO2, and more)
- Automatic alert generation when a reading exceeds the user's personal or default normal range
- AI-generated insight for each record, delivered asynchronously via RabbitMQ
- Filter records by metric type and date range

**Personalized Ranges**
- Normal ranges are calculated from the user's physical profile, health conditions, and current medications
- Ranges are recalculated automatically when the profile is updated
- Falls back to metric-type defaults when no personal range is available

**Nutrition Tracking**
- Upload a photo of a meal; the AI identifies the food and estimates calories, protein, carbohydrates, and fat
- Records are created immediately with `Pending` status and updated asynchronously to `Completed` or `Failed`
- Daily macro summary with progress bars relative to goals calculated from the user's profile
- Navigate between days and review individual meal records with AI confidence scores

**Alerts**
- Severity levels: Critical and Warning
- Linked back to the originating health record for context

**Account Management**
- Password recovery via email link (token hashed with SHA-256, expires in 15 minutes)
- Password reset via token from email
- Account deletion — removes all user data permanently

**Profile**
- Physical data and goals (weight, height, target weight, activity level)
- Training routine (frequency, types, seated hours, sleep)
- Health conditions and medications, each influencing the personalized range calculation
- Derived metrics: BMI, BMR, TDEE, calorie and macro goals
- Profile picture — choose from 8 illustrated avatars or use initials; avatar is shown in the sidebar and mobile topbar. Artwork by [@_gellyart](https://www.instagram.com/_gellyart/)

**Dashboard**
- Priority-ordered metric cards with trend indicators
- Recent activity feed
- AI insight card with expandable recommendations

---

## Project Structure

```
VitalSync/
├── docker-compose.yml
├── .env                          # secrets — see .env.example for all required keys
├── api/VitalSyncAPI/             # ASP.NET Core 10 Web API
│   ├── Domain/                   # Entities, interfaces, domain services
│   ├── Application/              # Use cases, DTOs, service interfaces
│   ├── Infrastructure/           # EF Core, repositories, UnitOfWork
│   ├── Controllers/              # HTTP endpoints
│   ├── Consumers/                # MassTransit consumers (InsightGenerated, NutritionAnalysisCompleted, WeeklyReportGenerated)
│   └── Events/                   # Shared contracts (namespace VitalSync.Contracts)
├── ai-service/VitalSyncAI/       # ASP.NET Core 10 AI microservice
│   ├── Consumers/                # MassTransit consumers (InsightRequested, NutritionAnalysisRequested)
│   ├── Models/                   # Shared contracts (namespace VitalSync.Contracts)
│   └── Services/                 # AnthropicService (health insights + food image analysis)
└── front/vitalsync-front/        # Angular 19 SPA
    └── src/app/
        ├── core/                 # Guards, interceptors, models, services
        ├── features/             # Page components
        └── shared/               # Layout shell, sidebar, toast
```

---

## Getting Started

### Prerequisites

- Docker and Docker Compose
- .NET 10 SDK (for running migrations locally)

### Environment

Create a `.env` file at the repository root:

```env
DB_USER=your_db_user
DB_PASSWORD=your_db_password
JWT_KEY=your_jwt_secret_key
RABBITMQ_USER=your_rabbitmq_user
RABBITMQ_PASSWORD=your_rabbitmq_password
ANTHROPIC_API_KEY=your_anthropic_key
ANTHROPIC_INSIGHT_MODEL=claude-haiku-4-5-20251001
ANTHROPIC_REPORT_MODEL=claude-sonnet-4-6

# Email (used for password recovery)
EMAIL_HOST=smtp.gmail.com
EMAIL_USER=your@gmail.com
EMAIL_PASSWORD=your_app_password   # Gmail App Password — no spaces
EMAIL_FROM=your@gmail.com
EMAIL_PORT=587
APP_BASE_URL=http://localhost:4200  # change to https://yourdomain.com in production
```

A `.env.example` file is included in the repository with all required keys and default model values.

### Running

```bash
# Start all services
docker compose up

# Rebuild the API after code changes
docker compose up --build api

# Rebuild the AI microservice after code changes
docker compose up --build ai-service

# Rebuild the frontend after any change
docker compose up --build frontend
```

| Service | URL |
|---------|-----|
| Frontend | http://localhost:4200 |
| API | http://localhost:5000 |
| RabbitMQ Management | http://localhost:15672 |

### Database Migrations

Migrations are applied automatically on API startup. To create a new migration, run locally (not inside the container):

```bash
cd api/VitalSyncAPI
export $(cat ../../.env | xargs)
dotnet ef migrations add <MigrationName>
```

To reset the database:

```bash
docker compose down -v && docker compose up db api
```

---

## API Reference

All authenticated endpoints require the `vitalsync_token` JWT cookie set by the login response.

### Authentication

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/auth/register` | — | Register a new user |
| POST | `/auth/login` | — | Login and receive JWT cookie |
| GET | `/auth/me` | Required | Get the authenticated user |
| POST | `/auth/logout` | Required | Logout and clear cookie |
| POST | `/auth/forgot-password` | — | Send password reset email (`?email=`) |
| POST | `/auth/reset-password` | — | Reset password with token from email |
| DELETE | `/auth` | Required | Delete account and all user data |

### Health Records

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/metrics` | Required | List all available metric types |
| GET | `/health-record` | Required | Get user records (filters: `metricTypeId`, `from`, `to`) |
| POST | `/health-record` | Required | Create record — triggers alert and publishes AI insight request |
| PUT | `/health-record/{id}` | Required | Update a record |
| DELETE | `/health-record/{id}` | Required | Delete a record |

### Profile

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/profile` | Required | Get current profile (404 if none) |
| POST | `/profile` | Required | Create or update profile |
| GET | `/profile/history` | Required | Profile change history |
| GET | `/profile/conditions` | Required | User health conditions |
| POST | `/profile/conditions` | Required | Replace conditions list |
| GET | `/profile/medications` | Required | User medications |
| POST | `/profile/medications` | Required | Replace medications list |
| GET | `/profile/personal-range` | Required | All personal metric ranges |
| GET | `/profile/personal-range/{metricTypeId}` | Required | Range for one metric |
| POST | `/profile/personal-range/recalculate` | Required | Recalculate all ranges (204) |

### Alerts

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/alerts` | Required | All user alerts, ordered by date descending |

### Nutrition

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| GET | `/nutrition` | Required | Nutrition records (filters: `from`, `to`) |
| POST | `/nutrition` | Required | Create record — saves as `Pending`, analysis runs asynchronously |
| GET | `/nutrition/{id}` | Required | Get a single record |
| PUT | `/nutrition/{id}` | Required | Update a record |
| DELETE | `/nutrition/{id}` | Required | Delete a record |
| GET | `/nutrition/summary` | Required | Daily summary with totals vs. goals (`date=YYYY-MM-DD&timezoneOffsetMinutes=N`) |

### Reports

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/reports/generate` | Required | Create weekly report — saves as `Pending`, AI generates asynchronously |
| GET | `/reports` | Required | List all reports (without `MetricsAnalysis`) |
| GET | `/reports/{id}` | Required | Get report by ID (with `MetricsAnalysis`) |

---

## Architecture Notes

**Clean Architecture** is applied on the API side. Dependencies flow inward: Controllers depend on Application, Application depends on Domain, Infrastructure implements the interfaces defined in Domain and Application.

**Async AI Pipeline**

All AI processing is decoupled from HTTP requests via RabbitMQ. The API publishes an event and returns immediately; the AI microservice consumes, calls Claude, and publishes the result back.

```
POST /health-record
  → save record + alert
  → publish InsightRequestedEvent
  → return 200 immediately

[RabbitMQ] → AI Service → Claude API → InsightGeneratedEvent → [RabbitMQ]
  → API consumer saves AIInsight to DB
```

```
POST /nutrition
  → save record (status: Pending)
  → publish NutritionAnalysisRequestedEvent
  → return 200 immediately

[RabbitMQ] → AI Service → Claude API (vision) → NutritionAnalysisCompletedEvent → [RabbitMQ]
  → API consumer updates record (status: Completed or Failed)
```

```
POST /reports/generate
  → save report (status: Pending)
  → publish WeeklyReportRequestedEvent
  → return 200 immediately

[RabbitMQ] → AI Service → Claude API → WeeklyReportGeneratedEvent → [RabbitMQ]
  → API consumer updates report (status: Completed or Failed), persists MetricsAnalysis
```

**Shared Contracts**

Event types used by both services live under the `VitalSync.Contracts` namespace in both projects. The namespace must match exactly — MassTransit encodes it in the message URN header (`urn:message:VitalSync.Contracts:EventName`) and rejects messages with mismatched types into a `_skipped` queue.

Exchange names are pinned via `cfg.Message<T>(m => m.SetEntityName(...))` in both `Program.cs` files to keep RabbitMQ exchange names stable and readable.

**Domain Services**
- `PersonalRangeCalculator` — derives normal ranges per metric from the user's profile, conditions, and medications
- `BodyMetricsCalculator` — computes BMI, BMR, TDEE, and macro goals from physical data. Calorie goal by goal type: `WeightLoss` → 80% TDEE, `MuscleGain` → TDEE + 250, `Maintenance` → TDEE, `Conditioning` → 105% TDEE, `ChronicConditionControl` → 85% TDEE, `GeneralHealth` → 90% TDEE. Minimum of 1200 kcal always enforced.
- `AlertGenerator` — creates an alert when a health record exceeds its applicable range; uses personal range when available, falls back to metric-type defaults

**Rate Limiting**

Applied via ASP.NET Core's built-in rate limiter, partitioned by user ID for authenticated routes and by IP for public ones:

| Policy | Applies to | Limit |
|--------|-----------|-------|
| `global` | All authenticated endpoints | 60 req/min |
| `ai-limit` | `POST /health-record`, `POST /reports/generate` | 15 req/min |
| `ai-limit-image` | `POST /nutrition` (image upload) | 5 req/min |
| `login` | `POST /auth/login` | 8 req/min |
| `register` | `POST /auth/register` | 3 req/min |

**Frontend**
- Angular 19 standalone components with the Signals API for reactive state
- Lazy-loaded feature modules behind an `authGuard`
- Global HTTP error handling via an Angular interceptor
- CSS custom properties for the design system with full dark mode support via `data-theme="dark"` on the `<html>` element
- Production build served by nginx (multi-stage Docker build); `nginx.conf` must include `try_files $uri $uri/ /index.html` for client-side routing to work

---

## VSCode Debugging

A `launch.json` is configured for attaching the C# debugger to the running API container via `vsdbg`.

```
docker compose up
# then press F5 in VSCode and select the dotnet VitalSyncAPI.dll process
```

---

## License

This project is for personal and educational use.
