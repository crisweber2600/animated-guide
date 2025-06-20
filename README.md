# DupScan

DupScan is an example solution demonstrating a multi-project layout using .NET 9.
It now includes a core library with duplicate detection logic and BDD tests.
Duplicate groups are ranked by how many bytes you can reclaim by linking files.
The `CsvHelper` package is used to export results for further analysis and the
CLI can write summaries with the `--out` option.
The projects target **.NET 9.0** so ensure you have the latest SDK installed.

## Project Goals
- Showcase how to organize a .NET 9 solution with several cooperating projects.
- Demonstrate duplicate detection across Microsoft Graph and Google Drive.
- Provide BDD scenarios that document expected behavior.
- Offer a CLI capable of scanning and linking duplicates.
- Keep code coverage above 80% with automated tests.

## Projects
- **DupScan.Core** – domain models and hash-based detection.
- **DupScan.Adapters** – infrastructure and external integrations.
- **DupScan.Orchestration** – combines scanner results and links duplicates when requested.
- **DupScan.Graph** – OneDrive/SharePoint integrations.
  Uses device-code authentication via Azure Identity to connect to Microsoft Graph.
- **DupScan.Google** – Google Drive integrations.
- **DupScan.Graph** now includes a shortcut-based linker for duplicates.
- **DupScan.Google** adds a matching link service for Google Drive files.
- **DupScan.Cli** – command-line entry point built with System.CommandLine.
- **DupScan.Tests** – xUnit and Reqnroll test suite with code coverage.
- **Integration Servers** – WireMock-based Graph and Google mocks under
  `DupScan.Tests/Integration` used by BDD scenarios.

## Getting Started
1. Run `dotnet restore` to download dependencies.
   Use `dotnet restore -warnaserror` to catch version conflicts early.
2. Build the solution with `dotnet build DupScan.sln`.
3. Update `appsettings.json` with your Graph and Google credentials.
4. Ensure the Graph `TenantId` and `ClientId` plus the Google `ClientId` and `ClientSecret` values are valid before scanning.
5. Execute the CLI project using `dotnet run --project DupScan.Cli`.
6. Try exporting results by running `dotnet run --project DupScan.Cli -- --out results.csv`.
7. Install additional packages like `CsvHelper` with `dotnet add <proj> package <name>`.
8. Run tests with coverage using `dotnet test DupScan.sln --collect:"XPlat Code Coverage"`.
9. Review coverage results in the generated `TestResults` directory.
10. Limit coverage calculation to core projects via `--settings coverlet.runsettings`.
11. Format source files with `dotnet format` before committing.
12. Set `DOTNET_CLI_UI_LANGUAGE=en` to suppress localization noise during builds.
13. Build the Docker image with `docker build -t dupscan .` for containerized runs.
14. The `CliLinking` scenario demonstrates Graph shortcut creation using a mock server.
15. Try `dotnet run --project DupScan.Cli` to see duplicate detection in action.
16. Customize provider roots and enable linking with `--link` and `--parallel` flags.
17. Verify your environment with `dotnet test --no-build --no-restore` before making changes.
18. Combine providers with the orchestrator service to analyze multiple drives.
19. Run `dotnet restore` before building to ensure all NuGet packages are available.
20. `WorkerQueue` enables parallel linking by processing duplicate groups concurrently.
21. Test coverage reports are stored under `DupScan.Tests/TestResults` for review.
22. Set `GRAPH_BASEURL` to override the default Graph service URL; the CLI reads this variable automatically.
23. The `GOOGLE_BASEURL` variable lets you target a mock Google server.
24. The CI workflow automatically runs `dotnet format` and the full test suite.

## Duplicate Detection
The core library exposes `FileItem` and `DuplicateGroup` models. The
`DuplicateDetector` service groups files by hash and calculates the potential
space savings. Groups are ordered by the `RecoverableBytes` value so the most
impactful duplicates appear first. Reqnroll scenarios demonstrate how identical
hashes are detected and ranked by recoverable bytes. Additional unit tests mock
Graph responses with Moq to validate scanning logic.
The new `CsvExport` feature file demonstrates how to save summaries.
Unit tests now also verify CSV export formatting to keep regressions from
slipping in.

## Microsoft Graph Scanning
`GraphClientFactory` builds a `GraphServiceClient` using `DeviceCodeCredential`.
`GraphScanner` retrieves drive items and converts them to `FileItem` records for
detection.
`GraphDriveService` exposes methods that call the Graph API directly and reads the `quickXorHash` value for each file.
If the base URL is not configured the service falls back to an empty string so tests can run without real credentials.
Delete the cached authentication files in `~/.azure` if you need to reauthenticate with different credentials.
You can pass a custom callback to `GraphClientFactory.Create` if you need to modify the device-code sign-in message or log additional details.

## Graph Linking
`GraphLinkService` replaces smaller copies with Graph shortcuts. It calls a
drive service to create the shortcut and delete the redundant file.
BDD scenarios in `DupScan.Tests` validate both scanning and linking workflows using the Reqnroll test runner.

## Google Drive Scanning
`GoogleScanner` uses `GoogleDriveService` to list files via OAuth desktop
credentials. Drive files are converted to `FileItem` objects for detection.
The integration server returns stubbed JSON allowing tests to run offline.
`GoogleDriveService` now creates shortcut files referencing the largest copy and
removes the duplicates using the Drive API. Unit tests assert that the expected
HTTP endpoints are invoked for each operation.

When running locally you can swap in the `HttpGoogleDriveService` from the test
project to point at a mock WireMock server. This makes it easy to develop
against predictable Drive responses without network access.

## Orchestration
`Orchestrator` aggregates results from any number of scanners and can invoke provider-specific link services.
This allows automated duplicate cleanup across Graph and Google drives in one run.

## Extending Scenarios
Edit the `.feature` files under `DupScan.Tests/Features` to define new cases.
WireMock servers automatically respond using the provided tables making it easy
to model different drive contents.
- New orchestration scenarios demonstrate how multiple providers work together.

## Codex Tasks
Run `codex tasks` to list available tasks. Key ones include:
- `install-dotnet-9` to install the required SDK
- `restore` and `build` for setup
- `test` to run the suite with coverage
- `register-azure-app` and `register-google-app` to configure credentials
- `run-cli` executes the command line tool using your settings
- `e2e` restores, builds, tests and then runs the CLI in one go

## CLI Hints
- Use `--out` to export CSV results via CsvHelper.
- The `CsvExporter` service formats duplicate groups for easy reporting.
- `--parallel` controls the worker channel degree of parallelism.
- Use the orchestrator to combine Graph and Google scans in one command.
- Inspect the generated CSV file to determine which groups reclaim the most
  space.
- Specify provider roots to limit scanning to certain directories.
- Provide one or more roots using `--root <path>` to scan specific folders.
- Use `--link` to automatically replace redundant files with symbolic links.
- Increase throughput with `--parallel 4` when linking many groups.
- After filling `appsettings.json` you can run `codex tasks run-cli`.
- Inspect the generated `results.csv` for a summary of duplicate files.
- For a full run including build and tests use `codex tasks e2e`.
- Run `dotnet run --project DupScan.Cli --help` to see all available options.
- Set `DOTNET_CLI_TELEMETRY_OPTOUT=1` to suppress CLI telemetry prompts.
- Services are resolved via dependency injection, making customization easy.
- Pass `--verbose` to the CLI for detailed logging of scanning operations.
- You can inspect generated feature bindings in the `Features` folder to learn how tests are organized.
- Multi-provider scenarios demonstrate parallel scanning across Google and Graph.
- New `CliLinking` tests run the command line with `--link` against a WireMock server.
- The repo ships lightweight HTTP services for use with integration tests.

## Project Structure
```
DupScan.Core/           # domain models and CSV export helpers
DupScan.Graph/          # Microsoft Graph scanner and link service
DupScan.Google/         # Google Drive scanner and link service
DupScan.Orchestration/  # orchestrator for multi-provider scans
DupScan.Cli/            # command-line front end
DupScan.Tests/          # Reqnroll scenarios and xUnit tests
```

## App Settings Example
Add your credentials to an `appsettings.json` file at the repository root:
```json
{
  "Graph": {
    "TenantId": "00000000-0000-0000-0000-000000000000",
    "ClientId": "00000000-0000-0000-0000-000000000000"
  },
  "Google": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-secret"
  }
}
```

## Continuous Integration
The included GitHub Actions workflow builds and tests the solution on every push.
Coverage results are uploaded so you can track changes over time.
Formatting errors fail the build, so run `dotnet format` locally before committing.

## Contributing
1. Create a feature branch from `main`.
2. Run `codex tasks restore`, `codex tasks build` and `codex tasks test`.
3. Add or update feature files under `DupScan.Tests/Features`.
4. Submit a pull request describing your changes and any new Codex tasks.
5. Ensure CI checks pass before merging.

