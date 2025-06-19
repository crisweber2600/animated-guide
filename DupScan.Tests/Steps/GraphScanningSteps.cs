using System.Collections.Generic;
using DupScan.Core.Models;
using DupScan.Graph;
using Microsoft.Graph.Models;
using Moq;
using Reqnroll;
using Xunit;

namespace DupScan.Tests.Steps;

[Binding]
public class GraphScanningSteps
{
    private readonly List<DriveItem> _items = new();
    private readonly Mock<IGraphDriveService> _mock = new();
    private IReadOnlyList<FileItem> _result = new List<FileItem>();

    [Given("Graph drive items")]
    public void GivenGraphDriveItems(Table table)
    {
        foreach (var row in table.Rows)
        {
            _items.Add(new DriveItem
            {
                Id = row["Id"],
                Name = row["Name"],
                Size = long.Parse(row["Size"]),
                File = new FileObject
                {
                    Hashes = new Hashes
                    {
                        QuickXorHash = row["Hash"]
                    }
                }
            });
        }
    }

    [When("I scan Graph")]
    public async Task WhenIScanGraph()
    {
        var response = new DriveItemCollectionResponse
        {
            Value = _items
        };

        _mock.Setup(m => m.GetRootChildrenAsync()).ReturnsAsync(response);
        var scanner = new GraphScanner(_mock.Object);
        _result = await scanner.ScanAsync();
    }

    [Then("two FileItem objects should be returned from Graph")]
    public void ThenTwoFileItemsShouldBeReturned()
    {
        Assert.Equal(2, _result.Count);
    }
}
