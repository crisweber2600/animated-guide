# DupScan

DupScan is an example solution demonstrating a multi-project layout using .NET 9.
It currently contains placeholder projects that will be expanded in future guides.

## Projects
- **DupScan.Core** – foundational domain logic.
- **DupScan.Adapters** – infrastructure and external integrations.
- **DupScan.Orchestration** – coordination services.
- **DupScan.Cli** – command-line entry point built with System.CommandLine.
- **DupScan.Tests** – xUnit test suite with code coverage enabled.

## Getting Started
1. Run `dotnet restore` to download dependencies.
2. Build the solution with `dotnet build DupScan.sln`.
3. Execute the CLI project using `dotnet run --project DupScan.Cli`.
4. Run tests with coverage using `dotnet test DupScan.sln --collect:"XPlat Code Coverage"`.
5. Review coverage results in the generated `TestResults` directory.
