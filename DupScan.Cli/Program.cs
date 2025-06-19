using System.CommandLine;
using DupScan.Core.Services;
using DupScan.Orchestration;
using Microsoft.Extensions.DependencyInjection;

var rootOption = new Option<DirectoryInfo[]>("--root", "Folders to scan") { AllowMultipleArgumentsPerToken = true };
var outOption = new Option<FileInfo?>("--out", "CSV output file path");
var linkOption = new Option<bool>("--link", "Replace duplicates with links");
var parallelOption = new Option<int>("--parallel", () => 1, "Degree of parallelism for linking");

var rootCommand = new RootCommand("Duplicate scanner")
{
    rootOption,
    outOption,
    linkOption,
    parallelOption
};

var services = new ServiceCollection();
services.AddSingleton<LocalScanner>();
services.AddSingleton<FileLinkService>();
services.AddSingleton<DuplicateDetector>();
services.AddSingleton<ScanOrchestrator>();
var provider = services.BuildServiceProvider();

rootCommand.SetHandler(async (DirectoryInfo[] roots, FileInfo? outFile, bool link, int parallel) =>
{
    var orchestrator = provider.GetRequiredService<ScanOrchestrator>();
    var groups = await orchestrator.ExecuteAsync(roots.Select(r => r.FullName), link, parallel);
    Console.WriteLine($"Found {groups.Count} duplicate group(s).");

    if (outFile is not null)
    {
        using var writer = outFile.CreateText();
        CsvExporter.WriteSummary(groups, writer);
        Console.WriteLine($"Wrote summary to {outFile.FullName}");
    }
}, rootOption, outOption, linkOption, parallelOption);

return await rootCommand.InvokeAsync(args);
