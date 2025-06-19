using System.Collections.Generic;
using DupScan.Core.Models;
using DupScan.Core.Services;
using Reqnroll;
using Xunit;

namespace DupScan.Tests.Steps;

[Binding]
public class DuplicateDetectionSteps
{
    private readonly List<FileItem> _files = new();
    private IReadOnlyList<DuplicateGroup> _result = new List<DuplicateGroup>();

    [Given("the following files")]
    public void GivenTheFollowingFiles(Table table)
    {
        foreach (var row in table.Rows)
        {
            _files.Add(new FileItem(
                row["Id"],
                row["Path"],
                row["Hash"],
                long.Parse(row["Size"])));
        }
    }

    [When("I detect duplicates")]
    public void WhenIDetectDuplicates()
    {
        var detector = new DuplicateDetector();
        _result = detector.FindDuplicates(_files);
    }

    [Then("one group should contain {int} files with hash h{int}")]
    public void ThenOneGroupShouldContainFilesWithHash(int count, int suffix)
    {
        var hash = $"h{suffix}";
        var group = Assert.Single(_result, g => g.Hash == hash);
        Assert.Equal(count, group.Files.Count);
    }

    [Then("the recoverable bytes should be {long}")]
    public void ThenRecoverableBytesShouldBe(long bytes)
    {
        var group = Assert.Single(_result);
        Assert.Equal(bytes, group.RecoverableBytes);
    }

    [Then("groups should be ordered by recoverable bytes")]
    public void ThenGroupsShouldBeOrderedByRecoverableBytes(Table table)
    {
        Assert.Equal(table.RowCount, _result.Count);
        for (int i = 0; i < table.RowCount; i++)
        {
            var expectedHash = table.Rows[i]["Hash"];
            var expectedRec = long.Parse(table.Rows[i]["Recoverable"]);
            Assert.Equal(expectedHash, _result[i].Hash);
            Assert.Equal(expectedRec, _result[i].RecoverableBytes);
        }
    }
}
