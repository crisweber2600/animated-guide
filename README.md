# Animated Guide

Animated Guide automates duplicate file detection across Microsoft 365 and Google Drive.

## Key Features
- **Cross-platform scanning:** works with OneDrive, SharePoint and Google Drive.
- **Duplicate ranking:** sorts by potential space savings.
- **Shortcut creation:** replaces extra copies with shortcuts when desired.
- **.NET 9:** built using the latest .NET 9 SDK for performance.
- **Codex tasks:** setup and build steps automated via `codex_tasks.yaml`.

## Setup
1. Install the .NET 9 SDK using the provided task `setup-dotnet`.
2. Configure Microsoft Graph and Google OAuth apps (see manual tasks in `codex_tasks.yaml`).
3. Run `codex run create-solution` to generate the solution skeleton.

## Running Tests
After implementing features, execute `codex run run-tests` to ensure coverage exceeds 80%.

## Contributing
Please read `Agents.md` for workflow rules. Updates to this README are welcome and encouraged.
