using System.IO;
using DupScan.Core.Models;
using DupScan.Core.Services;
using Xunit;

namespace DupScan.Tests.Unit;

public class CsvExporterTests
{
    [Fact]
    public void WriteSummary_outputs_expected_csv()
    {
        var groups = new[]
        {
            new DuplicateGroup("h1", new[]
            {
                new FileItem("1", "a", "h1", 10),
                new FileItem("2", "b", "h1", 20),
                new FileItem("3", "c", "h1", 30)
            }),
            new DuplicateGroup("h2", new[]
            {
                new FileItem("4", "d", "h2", 5),
                new FileItem("5", "e", "h2", 15)
            })
        };

        using var writer = new StringWriter();
        CsvExporter.WriteSummary(groups, writer);
        var csv = writer.ToString().Trim();

        var lines = csv.Split('\n');
        Assert.Equal("Hash,Count,RecoverableBytes", lines[0].Trim());
        Assert.Contains("h1,3,30", lines[1]);
        Assert.Contains("h2,2,5", lines[2]);
    }
}
