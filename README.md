# DupScan

DupScan is an example solution demonstrating a multi-project layout using .NET 9.
It now includes a core library with duplicate detection logic and BDD tests.
Duplicate groups are ranked by how many bytes you can reclaim by linking files.
The `CsvHelper` package is used to export results for further analysis.
The scanning services illustrate how to integrate with both Microsoft Graph and Google Drive.

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

## Getting Started
1. Run `dotnet restore` to download dependencies.
2. Build the solution with `dotnet build DupScan.sln`.
3. Execute the CLI project using `dotnet run --project DupScan.Cli`.
4. Install additional packages like `CsvHelper` with `dotnet add <proj> package <name>`.
5. Run tests with coverage using `dotnet test DupScan.sln --collect:"XPlat Code Coverage"`.
6. Review coverage results in the generated `TestResults` directory.
7. Try `dotnet run --project DupScan.Cli` to see duplicate detection in action.
8. Customize provider roots and enable linking with `--link` and `--parallel` flags.
9. Explore the new linking scenarios under `DupScan.Tests/Features` to see provider specific behavior.
10. Maintain coverage above 80% to meet the repository guidelines.

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

## Graph Linking
`GraphLinkService` replaces smaller copies with Graph shortcuts. It calls a
drive service to create the shortcut and delete the redundant file. The new
`GoogleLinkService` follows the same pattern for Google Drive to keep APIs
consistent across providers. BDD features demonstrate how duplicate sets are
reduced to a single master file while the extras are replaced with native
shortcuts.

## Google Drive Scanning
`GoogleScanner` uses `GoogleDriveService` to list files via OAuth desktop
credentials. Drive files are converted to `FileItem` objects for detection and
can be linked using the new linking service.


## CLI Hints
- Use `--out` to export CSV results via CsvHelper.
- `--parallel` controls the worker channel degree of parallelism.
- The core library now provides a `WorkerQueue` based on `System.Threading.Channels`.
- Google and Graph scanners automatically retry with quadratic back-off when 429 or 5xx errors occur.
- A new BDD scenario verifies channel workers execute tasks in parallel.
- Run `dotnet test` anytime you modify the code to ensure behavior remains correct.
- Explore the CLI with `--verbose` to see back-off and parallelism in action.
- Specify provider roots to limit scanning to certain directories.
- Set `DOTNET_CLI_TELEMETRY_OPTOUT=1` to suppress CLI telemetry prompts.
- Pass `--verbose` to the CLI for detailed logging of scanning operations.
- Use the test steps in `GraphLinkingSteps.cs` and `GoogleLinkingSteps.cs` as a
  reference when adding new providers.
