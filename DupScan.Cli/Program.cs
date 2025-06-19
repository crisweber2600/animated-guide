using System.CommandLine;
using DupScan.Core.Models;
using DupScan.Core.Services;
using DupScan.Graph;
using DupScan.Google;
using Microsoft.Extensions.DependencyInjection;

var rootOption = new Option<string[]>("--root", "Provider roots")
{
    AllowMultipleArgumentsPerToken = true
};
var linkOption = new Option<bool>("--link", "Link duplicates");
var parallelOption = new Option<int>("--parallel", () => 1, "Degree of parallelism");
var outOption = new Option<string?>("--out", "CSV output path");

var cmd = new RootCommand("Duplicate scanning CLI");
cmd.AddOption(rootOption);
cmd.AddOption(linkOption);
cmd.AddOption(parallelOption);
cmd.AddOption(outOption);

cmd.SetHandler(async (string[] roots, bool link, int parallel, string? outFile) =>
{
    var services = new ServiceCollection();
    services.AddSingleton<DuplicateDetector>();
    services.AddSingleton<GoogleScanner>();
    services.AddSingleton<IGoogleDriveService, GoogleDriveService>();
    services.AddSingleton<GraphScanner>();
    services.AddSingleton<IGraphDriveService, GraphDriveService>();
    services.AddSingleton<GraphLinkService>();

    using var provider = services.BuildServiceProvider();

    var google = provider.GetRequiredService<GoogleScanner>();
    var graph = provider.GetRequiredService<GraphScanner>();
    var detector = provider.GetRequiredService<DuplicateDetector>();
    var files = new List<FileItem>();
    files.AddRange(await google.ScanAsync());
    files.AddRange(await graph.ScanAsync());
    var groups = detector.FindDuplicates(files);
    Console.WriteLine($"Found {groups.Count} duplicate group(s).");

    if (link)
    {
        var linker = provider.GetRequiredService<GraphLinkService>();
        foreach (var g in groups)
        {
            await linker.LinkAsync(g);
        }
    }

    if (outFile is not null)
    {
        Console.WriteLine($"Would export CSV to {outFile}");
    }
}, rootOption, linkOption, parallelOption, outOption);

return await cmd.InvokeAsync(args);
