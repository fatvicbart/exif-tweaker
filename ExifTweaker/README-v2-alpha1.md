# ExifTweaker v2 alpha 1 — phases 0–4

This refactor introduces the new application core while retaining the existing WinForms UI.

## Included
- Hard-coded Maps.co API key removed. Configure `EXIFTWEAKER_MAPSCO_API_KEY` in the Windows environment.
- `PhotoItem`, `PhotoMetadata`, `MetadataPatch` domain models.
- ExifTool is now the metadata read/write engine; ExifLibrary is removed.
- JSON/batch metadata reads (`exiftool -json -n`).
- `async`/`await` based import/write flow; BackgroundWorker and manual thread polling removed.
- File/folder drag & drop with recursive discovery.
- Extended photo/video extension discovery.
- ExifTool original-file backups are kept when metadata is written.

## ExifTool installation
Place ExifTool at `exiftool\\exiftool.exe` beside the application output, or make `exiftool` available in PATH.
The binary is intentionally not bundled in this source archive.

## Current compatibility behaviour
The existing **Change** button still writes immediately so the old UI remains usable. Internally it now creates a `MetadataPatch` and applies it through ExifTool. The full non-destructive Pending Changes / Preview / Apply UX belongs to the next implementation phase.

## Geocoding
The source tree contains no API secret. Configure a Maps.co key for the current geocoding button:

`setx EXIFTWEAKER_MAPSCO_API_KEY "your-key"`

Restart ExifTweaker after setting it.
