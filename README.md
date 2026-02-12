# CookMate Backend

## Krav

- Docker Desktop (eller Docker Engine + Compose plugin)

## Hurtig startup

1. Opret lokal enviroment:

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

## Stop og clean up

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


