# DupScan

DupScan is an example solution demonstrating a multi-project layout using .NET 9.
It now includes a core library with duplicate detection logic and BDD tests.
Duplicate groups are ranked by how many bytes you can reclaim by linking files.
The `CsvHelper` package is used to export results for further analysis.
The repo demonstrates both Google Drive and Microsoft Graph integrations and the
tests use Reqnroll for behavior specifications.

## Projects
- **DupScan.Core** – domain models and hash-based detection.
- **DupScan.Adapters** – infrastructure and external integrations.
- **DupScan.Orchestration** – coordination services.
- **DupScan.Graph** – OneDrive/SharePoint integrations.
  Uses device-code authentication via Azure Identity to connect to Microsoft Graph.
- **DupScan.Google** – Google Drive integrations.
- **DupScan.Cli** – command-line entry point built with System.CommandLine.
- **DupScan.Tests** – xUnit and Reqnroll test suite with code coverage.

## Getting Started
1. Run `dotnet restore` to download dependencies.
2. Build the solution with `dotnet build DupScan.sln`.
3. Execute the CLI project using `dotnet run --project DupScan.Cli`.
4. Install additional packages like `CsvHelper` with `dotnet add <proj> package <name>`.
5. Run tests with coverage using `dotnet test DupScan.sln --collect:"XPlat Code Coverage"`.
6. Review coverage results in the generated `TestResults` directory.
7. Try `dotnet run --project DupScan.Cli` to see duplicate detection in action.
8. Customize provider roots and enable linking with `--link` and `--parallel` flags.
9. Restore global tools with `dotnet tool restore` if required for decompilation or other utilities.

## Duplicate Detection
The core library exposes `FileItem` and `DuplicateGroup` models. The
`DuplicateDetector` service groups files by hash and calculates the potential
space savings. Groups are ordered by the `RecoverableBytes` value so the most
impactful duplicates appear first. Reqnroll scenarios demonstrate how identical
hashes are detected and ranked by recoverable bytes. Additional unit tests mock
Graph responses with Moq to validate scanning logic.

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

## CLI Hints
- Use `--out` to export CSV results via CsvHelper.
- `--parallel` controls the worker channel degree of parallelism.
- Specify provider roots to limit scanning to certain directories.
- Set `DOTNET_CLI_TELEMETRY_OPTOUT=1` to suppress CLI telemetry prompts.
- Pass `--verbose` to the CLI for detailed logging of scanning operations.
- You can inspect generated feature bindings in the `Features` folder to learn how tests are organized.
