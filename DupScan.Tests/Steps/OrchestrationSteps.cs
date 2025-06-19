using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DupScan.Core.Models;
using DupScan.Core.Services;
using DupScan.Graph;
using DupScan.Google;
using DupScan.Orchestration;
using Xunit;
using Microsoft.Graph.Models;
using Moq;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using Reqnroll;

namespace DupScan.Tests.Steps;

[Binding]
public class OrchestrationSteps
{
    private readonly List<DriveItem> _graphItems = new();
    private readonly List<GoogleFile> _googleItems = new();
    private IReadOnlyList<DuplicateGroup> _result = new List<DuplicateGroup>();

    [Given("Graph items for orchestration")]
    public void GivenGraphItems(Table table)
    {
        foreach (var row in table.Rows)
        {
            _graphItems.Add(new DriveItem
            {
                Id = row["Id"],
                Name = row["Name"],
                Size = long.Parse(row["Size"]),
                File = new FileObject { Hashes = new Hashes { QuickXorHash = row["Hash"] } }
            });
        }
    }

    [Given("Google files for orchestration")]
    public void GivenGoogleFiles(Table table)
    {
        foreach (var row in table.Rows)
        {
            _googleItems.Add(new GoogleFile
            {
                Id = row["Id"],
                Name = row["Name"],
                Md5Checksum = row["Hash"],
                Size = long.Parse(row["Size"])
            });
        }
    }

    [When("I orchestrate scanning")]
    public async Task WhenIOrchestrateScanning()
    {
        var graphResponse = new DriveItemCollectionResponse { Value = _graphItems };
        var graphMock = new Mock<IGraphDriveService>();
        graphMock.Setup(m => m.GetRootChildrenAsync()).ReturnsAsync(graphResponse);
        var googleMock = new Mock<IGoogleDriveService>();
        googleMock.Setup(m => m.ListFilesAsync()).ReturnsAsync(_googleItems);

        var orchestrator = new Orchestrator(new[]
        {
            new ScanProvider(new GraphScanner(graphMock.Object), null),
            new ScanProvider(new GoogleScanner(googleMock.Object), null)
        }, new DuplicateDetector());

        _result = await orchestrator.RunAsync(false);
    }

    [Then("the orchestrator result should contain {int} files with hash h{int}")]
    public void ThenResultShouldContain(int count, int suffix)
    {
        var hash = $"h{suffix}";
        var group = Assert.Single(_result, g => g.Hash == hash);
        Assert.Equal(count, group.Files.Count);
    }
}
