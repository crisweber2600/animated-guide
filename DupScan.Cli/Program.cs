using System.CommandLine;
using DupScan.Core.Models;
using DupScan.Core.Services;

var outOption = new Option<FileInfo?>("--out", "CSV output file path");
var root = new RootCommand("Duplicate scanner") { outOption };

root.SetHandler((FileInfo? outFile) =>
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

    if (outFile is not null)
    {
        using var writer = outFile.CreateText();
        CsvExporter.WriteSummary(groups, writer);
        Console.WriteLine($"Wrote summary to {outFile.FullName}");
    }
}, outOption);

return root.Invoke(args);

