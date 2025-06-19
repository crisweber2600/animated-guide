# DupScan

DupScan is an example solution demonstrating a multi-project layout using .NET 9.
It now includes a core library with duplicate detection logic and BDD tests.

## Projects
- **DupScan.Core** – domain models and hash-based detection.
- **DupScan.Adapters** – infrastructure and external integrations.
- **DupScan.Orchestration** – coordination services.
- **DupScan.Graph** – OneDrive/SharePoint integrations.
- **DupScan.Google** – Google Drive integrations.
- **DupScan.Cli** – command-line entry point built with System.CommandLine.
- **DupScan.Tests** – xUnit and Reqnroll test suite with code coverage.

## Getting Started
1. Run `dotnet restore` to download dependencies.
2. Build the solution with `dotnet build DupScan.sln`.
3. Execute the CLI project using `dotnet run --project DupScan.Cli`.
4. Run tests with coverage using `dotnet test DupScan.sln --collect:"XPlat Code Coverage"`.
5. Review coverage results in the generated `TestResults` directory.
6. Try `dotnet run --project DupScan.Cli` to see duplicate detection in action.

## Duplicate Detection
The core library exposes `FileItem` and `DuplicateGroup` models. The
`DuplicateDetector` service groups files by hash and calculates the potential
space savings. Reqnroll scenarios demonstrate how identical hashes are detected
and ranked by recoverable bytes.
