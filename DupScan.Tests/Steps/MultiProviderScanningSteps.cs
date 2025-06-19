using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DupScan.Core.Models;
using DupScan.Tests.Integration;
using DupScan.Google;
using DupScan.Graph;
using GoogleService = DupScan.Tests.Integration.HttpGoogleDriveService;
using GraphService = DupScan.Tests.Integration.HttpGraphDriveService;
using Reqnroll;
using Xunit;
using DriveFile = Google.Apis.Drive.v3.Data.File;
using GraphItem = Microsoft.Graph.Models.DriveItem;

namespace DupScan.Tests.Steps;

[Binding]
public class MultiProviderScanningSteps : IDisposable
{
    private readonly List<DriveFile> _googleFiles = new();
    private readonly List<GraphItem> _graphItems = new();
    private GoogleWireMockServer? _googleServer;
    private GraphWireMockServer? _graphServer;
    private IReadOnlyList<FileItem> _results = new List<FileItem>();

    [Given("Google files for multi scan")]
    public void GivenGoogleFiles(Table table)
    {
        foreach (var row in table.Rows)
        {
            _googleFiles.Add(new DriveFile
            {
                Id = row["Id"],
                Name = row["Name"],
                Md5Checksum = row["Md5"],
                Size = long.Parse(row["Size"])
            });
        }
        _googleServer = new GoogleWireMockServer();
        _googleServer.SetupFiles(_googleFiles);
    }

    [Given("Graph items for multi scan")]
    public void GivenGraphItems(Table table)
    {
        foreach (var row in table.Rows)
        {
            _graphItems.Add(new GraphItem
            {
                Id = row["Id"],
                Name = row["Name"],
                Size = long.Parse(row["Size"]),
                File = new Microsoft.Graph.Models.FileObject
                {
                    Hashes = new Microsoft.Graph.Models.Hashes
                    {
                        QuickXorHash = row["Hash"]
                    }
                }
            });
        }
        _graphServer = new GraphWireMockServer();
        _graphServer.SetupChildren(_graphItems);
    }

    [When("I scan providers in parallel")]
    public async Task WhenIScanProvidersInParallel()
    {
        if (_googleServer == null || _graphServer == null) throw new InvalidOperationException();
        var googleService = new GoogleService(_googleServer.Url);
        var graphService = new GraphService(_graphServer.Url);
        var googleScanner = new GoogleScanner(googleService);
        var graphScanner = new GraphScanner(graphService);

        var results = await Task.WhenAll(googleScanner.ScanAsync(), graphScanner.ScanAsync());
        _results = results.SelectMany(r => r).ToList();
    }

    [Then("both providers should have been queried")]
    public void ThenBothProvidersQueried()
    {
        Assert.Equal(1, _googleServer!.Server.LogEntries.Count(l => l.RequestMessage.Path == "/files"));
        Assert.Equal(1, _graphServer!.Server.LogEntries.Count(l => l.RequestMessage.Path == "/drive/root/children"));
    }

    public void Dispose()
    {
        _googleServer?.Dispose();
        _graphServer?.Dispose();
    }
}
