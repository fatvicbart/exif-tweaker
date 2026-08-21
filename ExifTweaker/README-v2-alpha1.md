# ExifTweaker v2 alpha 3 — phases 0-15

This source tree contains the v2 foundations, batch workflow and geolocation workflow.

## Included

- Hard-coded Maps.co API key removed. Configure `EXIFTWEAKER_MAPSCO_API_KEY` in the Windows environment.
- `PhotoItem`, `PhotoMetadata`, `MetadataPatch` domain models with pending non-destructive edits.
- ExifTool is the metadata read/write engine; ExifLibrary is removed from the app code path.
- JSON/batch metadata reads (`exiftool -json -n`).
- `async`/`await` based import/write flow with cancellation.
- File/folder drag & drop with recursive discovery.
- Extended photo/video extension discovery.
- Import session statistics, filters and pending-change status.
- Explicit Designer-declared WinForms controls; no dynamic control creation in code-behind.
- Batch date staging and relative shifts.
- GPS staging/removal with latitude, longitude and optional altitude.
- Copy/paste GPS between selected media.
- WebView2/Leaflet map bridge with click-to-stage GPS, multiple markers and selection zoom.
- Maps.co search geocoding, selectable results, reverse geocoding and memory cache.
- Apply preview dialog before disk writes.
- Per-file Apply/Restore report with backup availability.
- ExifTool original-file backups and restore workflow.
- Undo/redo/reset before Apply.

## ExifTool installation

Place ExifTool at `exiftool\\exiftool.exe` beside the application output, or make `exiftool` available in PATH.
The binary is intentionally not bundled in this source archive.

## Geocoding

Configure a Maps.co key for search and reverse geocoding:

`setx EXIFTWEAKER_MAPSCO_API_KEY "your-key"`

Restart ExifTweaker after setting it.

## WebView2

The map requires NuGet restore for `Microsoft.Web.WebView2` and the Microsoft Edge WebView2 Runtime on Windows.

## Validation before use

Run `dotnet restore` then `dotnet build` on Windows with .NET 10. Test on copies of JPEG, HEIC, RAW, MOV and MP4 files until ExifTool read/write, backup and restoration have been verified with your real media set.
