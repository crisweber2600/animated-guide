using System.CommandLine;
using System.IO;
using DupScan.Core.Models;
using DupScan.Core.Services;
using DupScan.Graph;

var defaultGraphUrl = Environment.GetEnvironmentVariable("GRAPH_BASEURL") ?? "http://localhost:5000";

var outOption = new Option<FileInfo?>("--out", "CSV output file path");
var linkOption = new Option<bool>("--link", "Replace duplicates with shortcuts");
var graphUrlOption = new Option<string>("--graph-url", () => defaultGraphUrl, "Graph service base URL");
var root = new RootCommand("Duplicate scanner") { outOption, linkOption, graphUrlOption };

root.SetHandler(async (FileInfo? outFile, bool link, string graphUrl) =>
{
    var detector = new DuplicateDetector();
    var files = new[]
    {
        new FileItem("1", "foo.txt", "hash1", 100),
        new FileItem("2", "bar.txt", "hash1", 120),
        new FileItem("3", "baz.txt", "hash2", 50)
    };
    var groups = detector.FindDuplicates(files);
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
