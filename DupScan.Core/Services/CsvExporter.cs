using System.Globalization;
using CsvHelper;
using DupScan.Core.Models;

namespace DupScan.Core.Services;

public static class CsvExporter
{
    public static void WriteSummary(IEnumerable<DuplicateGroup> groups, TextWriter writer)
    {
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteHeader<DuplicateSummary>();
        csv.NextRecord();
        foreach (var group in groups)
        {
            var summary = new DuplicateSummary
            {
                Hash = group.Hash,
                Count = group.Files.Count,
                RecoverableBytes = group.RecoverableBytes
            };
            csv.WriteRecord(summary);
            csv.NextRecord();
        }
    }

    private record DuplicateSummary
    {
        public string Hash { get; init; } = string.Empty;
        public int Count { get; init; }
        public long RecoverableBytes { get; init; }
    }
}
