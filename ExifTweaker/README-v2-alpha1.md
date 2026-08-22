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

## ExifTool integration

The complete Windows ExifTool distribution is stored in
`Infrastructure/exiftool`. MSBuild copies it to `exiftool/` beside the
application during build and publish; the `exiftool_files` sidecar
directory must remain next to `exiftool.exe`.

Resolution order:

1. path configured in Settings or `EXIFTWEAKER_EXIFTOOL_PATH`;
2. bundled `exiftool/exiftool.exe` beside the application;
3. `exiftool.exe` available in `PATH`.

A configured path may point either to `exiftool.exe` or to its
containing directory. Availability is validated by executing
`exiftool -ver`, not only by checking that the file exists.

## Geocoding

Configure a Maps.co key for search and reverse geocoding:

`setx EXIFTWEAKER_MAPSCO_API_KEY "your-key"`

Restart ExifTweaker after setting it.

## WebView2

The map requires NuGet restore for `Microsoft.Web.WebView2` and the Microsoft Edge WebView2 Runtime on Windows.

## Validation before use

Run `dotnet restore`, `dotnet build --configuration Release` and
`dotnet test --configuration Release` on Windows with .NET 10. The
Windows integration suite executes the bundled ExifTool on a temporary
Unicode TIFF and validates version detection, Apply, read-back, original
backup and byte-identical restoration.

Keep testing on copies of JPEG, HEIC, RAW, MOV and MP4 files until these
operations have also been verified with your real media set.
