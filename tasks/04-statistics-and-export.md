# 04 - Statistics & Export

Aggregations over the daily documents + XLSX export.

## Scope

- `KeyTracker.Core/Statistics`: totals for day / week / month / year, and daily average, from the LiteDB documents
- Queries directly against LiteDB, don't load everything into memory needlessly (use date ranges)
- `KeyTracker.Infrastructure/Export`: generates `.xlsx` (ClosedXML) with the same columns as the dashboard (date, total, per period)
- Logs `ExportStarted` / `ExportCompleted`

## Acceptance criteria

- Totals match the manual sum of the test documents
- Export produces an `.xlsx` that opens in Excel with correct data
