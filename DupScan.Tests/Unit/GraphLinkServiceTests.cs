using System.Collections.Generic;
using System.Threading.Tasks;
using DupScan.Core.Models;
using DupScan.Graph;
using Moq;
using Xunit;

namespace DupScan.Tests.Unit;

public class GraphLinkServiceTests
{
    [Fact]
    public async Task LinkAsync_creates_shortcuts_and_deletes_items()
    {
        var group = new DuplicateGroup("h1", new[]
        {
            new FileItem("1", "/a", "h1", 10),
            new FileItem("2", "/b", "h1", 20)
        });

        var mock = new Mock<IGraphDriveService>();
        var linker = new GraphLinkService(mock.Object);

        await linker.LinkAsync(group);

        mock.Verify(m => m.CreateShortcutAsync("1", "2"), Times.Once);
        mock.Verify(m => m.DeleteItemAsync("1"), Times.Once);
    }
}
