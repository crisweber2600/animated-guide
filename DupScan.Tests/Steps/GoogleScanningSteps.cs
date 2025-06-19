using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DupScan.Core.Models;
using DupScan.Google;
using DupScan.Tests.Integration;
using DriveFile = Google.Apis.Drive.v3.Data.File;
using Reqnroll;
using Xunit;

namespace DupScan.Tests.Steps;

[Binding]
public class GoogleScanningSteps : IDisposable
{
    private readonly List<DriveFile> _files = new();
    private GoogleWireMockServer? _server;
    private IReadOnlyList<FileItem> _result = new List<FileItem>();

    [Given("Google Drive files")]
    public void GivenGoogleDriveFiles(Table table)
    {
        foreach (var row in table.Rows)
        {
            _files.Add(new DriveFile
            {
                Id = row["Id"],
                Name = row["Name"],
                Md5Checksum = row["Md5"],
                Size = long.Parse(row["Size"])
            });
        }
        _server = new GoogleWireMockServer();
        _server.SetupFiles(_files);
    }

    [When("I scan Google Drive")]
    public async Task WhenIScanGoogleDrive()
    {
        if (_server == null) throw new InvalidOperationException("Server not started");
        var service = new HttpGoogleDriveService(_server.Url);
        var scanner = new GoogleScanner(service);
        _result = await scanner.ScanAsync();
    }

    [Then("two FileItem objects should be returned")]
    public void ThenTwoFileItemsShouldBeReturned()
    {
        Assert.Equal(2, _result.Count);
    }

    public void Dispose()
    {
        _server?.Dispose();
    }
}
