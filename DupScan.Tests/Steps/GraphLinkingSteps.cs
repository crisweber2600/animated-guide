using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DupScan.Core.Models;
using DupScan.Graph;
using DupScan.Tests.Integration;
using Reqnroll;
using Xunit;
using System.Threading.Tasks;

namespace DupScan.Tests.Steps;

[Binding]
public class GraphLinkingSteps : IDisposable
{
    private readonly List<FileItem> _files = new();
    private GraphWireMockServer? _server;

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
        if (_server == null)
        {
            _server = new GraphWireMockServer();
            _server.ExpectShortcut(_files[0].Id, _files[1].Id);
        }

        var group = new DuplicateGroup("h1", _files);
        var service = new HttpGraphDriveService(_server.Url);
        var linker = new GraphLinkService(service);
        await linker.LinkAsync(group);
    }

    [Then("the drive service should link (.*) to (.*)")]
    public void ThenTheDriveServiceShouldLink(string sourceId, string targetId)
    {
        Assert.Equal(1, _server?.Server.LogEntries.Count(l => l.RequestMessage.Path == $"/drive/items/{sourceId}/shortcut"));
        Assert.Equal(1, _server?.Server.LogEntries.Count(l => l.RequestMessage.Path == $"/drive/items/{sourceId}" && l.RequestMessage.Method == "DELETE"));
    }

    public void Dispose()
    {
        _server?.Dispose();
    }
}
