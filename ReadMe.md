# Gonzaga.SalesDataProcessor.Api

A small ASP.NET Core Web API that accepts CSV file uploads, computes an aggregate and gets the average, and keeps a record of what's been processed.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Docker Desktop (only needed if you want to run it containerized)

## Running locally

```bash
dotnet run --project Gonzaga.SalesDataProcessor.Api
```

The API listens on `http://localhost:5066` by default (see `Gonzaga.SalesDataProcessor.Api/Properties/launchSettings.json`).

## Running with Docker

```bash
docker build -f Gonzaga.SalesDataProcessor.Api/Dockerfile -t gonzaga-salesdataprocessor-api .
docker run -d -p 8080:8080 -e ApiKey=your-key-here --name gonzaga-salesdataprocessor-api gonzaga-salesdataprocessor-api
```

The API is then reachable at `http://localhost:8080`.

Processed-file tracking is written to `Data/file-processing.log` inside the container. 

## Authentication

Every request under `/api` requires an `X-API-KEY` header matching the configured key. Missing or incorrect keys get a `401`.

```bash
curl -H "X-API-KEY: development-api-key" http://localhost:5066/api/sales/reports
```

## Endpoints

### `POST /api/sales/process`

Uploads a CSV and returns the record count and average of the `Amount` column.

The CSV must have a header row with `Id`, `Name`, `Amount` columns:

```csv
Id,Name,Amount
1,Patrick,1000
2,James,1500
```

Request:
```bash
curl -X POST \
  -H "X-API-KEY: development-api-key" \
  -F "file=@Test-Data/sales.csv" \
  http://localhost:5066/api/sales/process
```

Response:
```json
{
  "fileName": "sales.csv",
  "recordCount": 5,
  "averageAmount": 3000,
  "processedAt": "2026-08-22T14:28:56.301Z",
  "duration": 37
}
```

Rejected with `400` if the file is missing, empty, not a `.csv`, has no data rows.

### `GET /api/sales/reports`

Returns how many files have been processed in total, and the most recent entries.

Query parameter: `limit` (optional, default 20) — caps how many recent entries come back. Does not affect the total count.

Request:
```bash
curl -H "X-API-KEY: development-api-key" "http://localhost:5066/api/sales/reports?limit=5"
```

Response:
```json
{
  "totalFilesProcessed": 3,
  "files": [
    {
      "fileName": "sales.csv",
      "recordCount": 5,
      "averageAmount": 3000,
      "processedAt": "2026-08-16T10:42:11.123Z",
      "duration": 8
    }
  ]
}
```

## Tests

```bash
dotnet test
```

Covers the API key middleware (missing/wrong/valid key), file validation (null/empty/wrong extension), the aggregate calculation, and the file-based processing tracker (including that the total count reflects all recorded files, not just the page returned by `limit`).

## Project layout

```
Gonzaga.SalesDataProcessor.Api/         the service
Gonzaga.SalesDataProcessor.Api.Tests/   unit tests
Test-Data/                              sample CSV for manual testing
```
