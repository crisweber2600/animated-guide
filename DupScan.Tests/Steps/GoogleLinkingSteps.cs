using System.Collections.Generic;
using DupScan.Core.Models;
using DupScan.Google;
using Moq;
using Reqnroll;
using Xunit;
using System.Threading.Tasks;

namespace DupScan.Tests.Steps;

[Binding]
public class GoogleLinkingSteps
{
    private readonly List<FileItem> _files = new();
    private readonly Mock<IGoogleDriveService> _mock = new();

    [Given("Google duplicate files")]
    public void GivenGoogleDuplicateFiles(Table table)
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

    [When("I link duplicates on Google")]
    public async Task WhenILinkDuplicatesOnGoogle()
    {
        var group = new DuplicateGroup("h1", _files);
        var linker = new GoogleLinkService(_mock.Object);
        await linker.LinkAsync(group);
    }

    [Then("the Google drive service should link (.*) to (.*)")]
    public void ThenTheDriveServiceShouldLink(string sourceId, string targetId)
    {
        _mock.Verify(m => m.CreateShortcutAsync(sourceId, targetId), Times.Once);
        _mock.Verify(m => m.DeleteFileAsync(sourceId), Times.Once);
    }
}
