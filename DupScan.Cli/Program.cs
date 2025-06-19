using DupScan.Core.Models;
using DupScan.Core.Services;

var detector = new DuplicateDetector();
var files = new[]
{
    new FileItem("1", "foo.txt", "hash1", 100),
    new FileItem("2", "bar.txt", "hash1", 120),
    new FileItem("3", "baz.txt", "hash2", 50)
};
var groups = detector.FindDuplicates(files);
Console.WriteLine($"Found {groups.Count} duplicate group(s).");
