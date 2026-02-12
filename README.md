# CookMate API (Docker setup)

Denne mappe kan startes direkte med Docker Compose og spinner både API og database op.

## Krav

- Docker Desktop (eller Docker Engine + Compose plugin)

## Hurtig start

1. Opret lokal miljøfil:

   ```bash
   cp .env.example .env
   ```

2. (Anbefalet) Opdater secrets i `.env`:
   - `POSTGRES_PASSWORD`
   - `JWT_SIGNING_KEY` (minimum 32 tegn)

3. Start API + database:

   ```bash
   docker compose up --build
   ```

4. Åbn Swagger:
   - `http://localhost:8080/swagger`

## Stop og oprydning

- Stop services:

  ```bash
  docker compose down
  ```

- Stop og slet database-data (volume):

  ```bash
  docker compose down -v
  ```

## Services

- API: `http://localhost:8080`
- PostgreSQL: `localhost:5432`
  - DB: `POSTGRES_DB`
  - User: `POSTGRES_USER`
  - Password: `POSTGRES_PASSWORD`

## Typiske endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/me` (kræver Bearer token)
- `GET /health/live`
- `GET /health/ready`

## CI/CD baseline

Projektet bruger en GitHub Actions baseline pipeline.

### Triggers

- Pull requests mod `main`
- Push til `main`
- Manuel kørsel via `workflow_dispatch`

### Hvad CI validerer

- `dotnet restore`, `dotnet build`, `dotnet test` på `CookMate.sln`
- Docker build validering af API image (`CookMate_project/Dockerfile`)
- Upload af test artifacts (`.trx`)

### Hvad der kører på `main`

- `dotnet publish` af API
- Upload af publish artifact (`cookmate-api-publish`)

### CD status

Auto deploy er bevidst ikke aktiveret i baseline. Pipeline er gjort deploy-klar via build + artifacts.

### Anbefalet branch protection

Konfigurer følgende required status checks for `main`:

- `Build and Test`
- `Docker Validate`
