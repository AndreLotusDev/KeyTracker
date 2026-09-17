# 04 - Statistics & Export

Aggregations over the daily documents + XLSX export.

## Scope

- [x] `KeyTracker.Core/Statistics`: totals for day / week / month / year, and daily average, from the LiteDB documents
- [x] Queries directly against LiteDB, don't load everything into memory needlessly (use date ranges)
- [x] `KeyTracker.Infrastructure/Export`: generates `.xlsx` (ClosedXML) with the same columns as the dashboard (date, total, per period)
- [x] Logs `ExportStarted` / `ExportCompleted`

## Acceptance criteria

- [x] Totals match the manual sum of the test documents
- [x] Export produces an `.xlsx` that opens in Excel with correct data
