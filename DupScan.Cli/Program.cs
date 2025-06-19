using System.CommandLine;
using System.IO;
using DupScan.Core.Models;
using DupScan.Core.Services;
using DupScan.Graph;

var rootOption = new Option<DirectoryInfo[]>("--root", "Folders to scan") { AllowMultipleArgumentsPerToken = true };
var outOption = new Option<FileInfo?>("--out", "CSV output file path");
var linkOption = new Option<bool>("--link", "Replace duplicates with shortcuts");
var graphUrlOption = new Option<string>("--graph-url", () => "http://localhost:5000", "Graph service base URL");
var root = new RootCommand("Duplicate scanner") { outOption, linkOption, graphUrlOption };

root.SetHandler(async (FileInfo? outFile, bool link, string graphUrl) =>
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

    if (link)
    {
        var drive = new HttpGraphDriveService(graphUrl);
        var linker = new GraphLinkService(drive);
        foreach (var g in groups)
        {
            await linker.LinkAsync(g);
        }
    }

    if (outFile is not null)
    {
        using var writer = outFile.CreateText();
        CsvExporter.WriteSummary(groups, writer);
        Console.WriteLine($"Wrote summary to {outFile.FullName}");
    }
}, outOption, linkOption, graphUrlOption);

return await root.InvokeAsync(args);


return await rootCommand.InvokeAsync(args);
