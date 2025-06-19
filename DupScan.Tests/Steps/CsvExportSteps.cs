using System.Collections.Generic;
using System.IO;
using DupScan.Core.Models;
using DupScan.Core.Services;
using Reqnroll;
using Xunit;

namespace DupScan.Tests.Steps;

[Binding]
public class CsvExportSteps
{
    private readonly List<DuplicateGroup> _groups = new();
    private string _csv = string.Empty;

    [Given("duplicate summary groups")]
    public void GivenDuplicateSummaryGroups(Table table)
    {
        foreach (var row in table.Rows)
        {
            var count = int.Parse(row["Count"]);
            var recoverable = long.Parse(row["RecoverableBytes"]);
            var size = count > 1 ? recoverable / (count - 1) : 0;

            var files = new List<FileItem>();
            for (int i = 0; i < count; i++)
            {
                files.Add(new FileItem(i.ToString(), $"f{i}", row["Hash"], size));

            }
            _groups.Add(new DuplicateGroup(row["Hash"], files));
        }
    }

    [When("I export the summary")]
    public void WhenIExportTheSummary()
    {
        using var writer = new StringWriter();
        CsvExporter.WriteSummary(_groups, writer);
        _csv = writer.ToString().Trim();
    }

    [Then("the csv should contain")] 
    public void ThenTheCsvShouldContain(Table table)
    {
        var lines = _csv.Split('\n');
        for (int i = 0; i < table.RowCount; i++)
        {
            var expected = string.Join(',', table.Rows[i]["Hash"], table.Rows[i]["Count"], table.Rows[i]["RecoverableBytes"]);
            Assert.Contains(expected, lines[i + 1]);
        }
    }
}
