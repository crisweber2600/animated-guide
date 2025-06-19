using System.Collections.Generic;
using System.Threading.Tasks;
using DupScan.Core.Models;
using DupScan.Graph;
using Microsoft.Graph;
using Moq;
using Microsoft.Graph.Models;
using Xunit;

namespace DupScan.Tests.Unit;

public class GraphScannerTests
{
    [Fact]
    public async Task ScanAsync_converts_drive_items_to_file_items()
    {
        var driveItems = new List<DriveItem>
        {
            new DriveItem
            {
                Id = "1",
                Name = "file1.txt",
                Size = 10,
                File = new FileObject { Hashes = new Hashes { QuickXorHash = "h1" } }
            },
            new DriveItem
            {
                Id = "2",
                Name = "file2.txt",
                Size = 20,
                File = new FileObject { Hashes = new Hashes { QuickXorHash = "h2" } }
            }
        };

        var response = new DriveItemCollectionResponse { Value = driveItems };
        var mock = new Mock<IGraphDriveService>();
        mock.Setup(m => m.GetRootChildrenAsync()).ReturnsAsync(response);

        var scanner = new GraphScanner(mock.Object);
        var files = await scanner.ScanAsync();

        Assert.Collection(files,
            f => Assert.Equal("h1", f.Hash),
            f => Assert.Equal("h2", f.Hash));
    }
}
