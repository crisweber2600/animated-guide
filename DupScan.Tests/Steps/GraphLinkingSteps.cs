using System.Collections.Generic;
using DupScan.Core.Models;
using DupScan.Graph;
using Moq;
using Reqnroll;
using Xunit;

namespace DupScan.Tests.Steps;

[Binding]
public class GraphLinkingSteps
{
    private readonly List<FileItem> _files = new();
    private readonly Mock<IGraphDriveService> _mock = new();

    [Given("duplicate files")]
    public void GivenDuplicateFiles(Table table)
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

    [When("I link duplicates on Graph")]
    public async Task WhenILinkDuplicatesOnGraph()
    {
        var group = new DuplicateGroup("h1", _files);
        var linker = new GraphLinkService(_mock.Object);
        await linker.LinkAsync(group);
    }

    [Then("the drive service should link (.*) to (.*)")]
    public void ThenTheDriveServiceShouldLink(string sourceId, string targetId)
    {
        _mock.Verify(m => m.CreateShortcutAsync(sourceId, targetId), Times.Once);
        _mock.Verify(m => m.DeleteItemAsync(sourceId), Times.Once);
    }
}
