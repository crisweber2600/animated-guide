# DupScan

DupScan is an example solution demonstrating a multi-project layout using .NET 9.
It now includes a core library with duplicate detection logic and BDD tests.
Duplicate groups are ranked by how many bytes you can reclaim by linking files.
The `CsvHelper` package is used to export results for further analysis and the
CLI can write summaries with the `--out` option.
The projects target **.NET 9.0** so ensure you have the latest SDK installed.

## Projects
- **DupScan.Core** – domain models and hash-based detection.
- **DupScan.Adapters** – infrastructure and external integrations.
- **DupScan.Orchestration** – coordination services.
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
3. Execute the CLI project using `dotnet run --project DupScan.Cli`.
4. Try exporting results by running `dotnet run --project DupScan.Cli -- --out results.csv`.
5. Install additional packages like `CsvHelper` with `dotnet add <proj> package <name>`.
6. Run tests with coverage using `dotnet test DupScan.sln --collect:"XPlat Code Coverage"`.
7. Review coverage results in the generated `TestResults` directory.
8. Try `dotnet run --project DupScan.Cli` to see duplicate detection in action.
9. Customize provider roots and enable linking with `--link` and `--parallel` flags.
10. Verify package versions with `dotnet list package --outdated` to stay current.
11. Upgrade references when restore warnings like NU1603 or NU1902 appear.
12. Keep an eye on advisory notices for security patches in Azure and Google SDKs.

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

## Graph Linking
`GraphLinkService` replaces smaller copies with Graph shortcuts. It calls a
drive service to create the shortcut and delete the redundant file.
BDD scenarios in `DupScan.Tests` validate both scanning and linking workflows using the Reqnroll test runner.

## Google Drive Scanning
`GoogleScanner` uses `GoogleDriveService` to list files via OAuth desktop
credentials. Drive files are converted to `FileItem` objects for detection.
The integration server returns stubbed JSON allowing tests to run offline.

## Extending Scenarios
Edit the `.feature` files under `DupScan.Tests/Features` to define new cases.
WireMock servers automatically respond using the provided tables making it easy
to model different drive contents.


## CLI Hints
- Use `--out` to export CSV results via CsvHelper.
- The `CsvExporter` service formats duplicate groups for easy reporting.
- `--parallel` controls the worker channel degree of parallelism.
- Inspect the generated CSV file to determine which groups reclaim the most
  space.
- Specify provider roots to limit scanning to certain directories.
- Provide one or more roots using `--root <path>` to scan specific folders.
- Run `dotnet run --project DupScan.Cli --help` to see all available options.
- Set `DOTNET_CLI_TELEMETRY_OPTOUT=1` to suppress CLI telemetry prompts.
- Pass `--verbose` to the CLI for detailed logging of scanning operations.
- You can inspect generated feature bindings in the `Features` folder to learn how tests are organized.

