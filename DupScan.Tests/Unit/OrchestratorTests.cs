using System.Collections.Generic;
using System.Threading.Tasks;
using DupScan.Core.Models;
using DupScan.Core.Services;
using DupScan.Orchestration;
using Moq;
using Xunit;

namespace DupScan.Tests.Unit;

public class OrchestratorTests
{
    [Fact]
    public async Task RunAsync_aggregates_scanner_results()
    {
        var s1 = new Mock<IScanner>();
        s1.Setup(s => s.ScanAsync()).ReturnsAsync(new[]
        {
            new FileItem("1","a","h1",1)
        });

        var s2 = new Mock<IScanner>();
        s2.Setup(s => s.ScanAsync()).ReturnsAsync(new[]
        {
            new FileItem("2","b","h1",2)
        });

        var orchestrator = new Orchestrator(new[]
        {
            new ScanProvider(s1.Object, null),
            new ScanProvider(s2.Object, null)
        }, new DuplicateDetector());

        var groups = await orchestrator.RunAsync(false);

        var group = Assert.Single(groups);
        Assert.Equal(2, group.Files.Count);
    }

    [Fact]
    public async Task RunAsync_links_duplicates_when_enabled()
    {
        var file1 = new FileItem("1", "a", "h1", 1);
        var file2 = new FileItem("2", "b", "h1", 2);
        var scanner = new Mock<IScanner>();
        scanner.Setup(s => s.ScanAsync()).ReturnsAsync(new[] { file1, file2 });
        var linker = new Mock<ILinkService>();
        var orchestrator = new Orchestrator(new[] { new ScanProvider(scanner.Object, linker.Object) }, new DuplicateDetector());

        await orchestrator.RunAsync(true);

        linker.Verify(l => l.LinkAsync(It.Is<DuplicateGroup>(g => g.Files.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task RunAsync_does_not_link_across_providers()
    {
        var file1 = new FileItem("1", "a", "h1", 1);
        var file2 = new FileItem("2", "b", "h1", 2);
        var s1 = new Mock<IScanner>();
        s1.Setup(s => s.ScanAsync()).ReturnsAsync(new[] { file1 });
        var l1 = new Mock<ILinkService>();
        var s2 = new Mock<IScanner>();
        s2.Setup(s => s.ScanAsync()).ReturnsAsync(new[] { file2 });
        var l2 = new Mock<ILinkService>();

        var orchestrator = new Orchestrator(new[]
        {
            new ScanProvider(s1.Object, l1.Object),
            new ScanProvider(s2.Object, l2.Object)
        }, new DuplicateDetector());

        await orchestrator.RunAsync(true);

        l1.Verify(l => l.LinkAsync(It.IsAny<DuplicateGroup>()), Times.Never);
        l2.Verify(l => l.LinkAsync(It.IsAny<DuplicateGroup>()), Times.Never);
    }
}
