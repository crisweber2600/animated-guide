using System.Collections.Generic;
using DupScan.Core.Models;
using DupScan.Google;
using DriveFile = Google.Apis.Drive.v3.Data.File;
using Moq;
using Reqnroll;
using Xunit;

namespace DupScan.Tests.Steps;

[Binding]
public class GoogleScanningSteps
{
    private readonly List<DriveFile> _files = new();
    private readonly Mock<IGoogleDriveService> _mock = new();
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
    }

    [When("I scan Google Drive")]
    public async Task WhenIScanGoogleDrive()
    {
        _mock.Setup(m => m.ListFilesAsync()).ReturnsAsync(_files);
        var scanner = new GoogleScanner(_mock.Object);
        _result = await scanner.ScanAsync();
    }

    [Then("two FileItem objects should be returned")]
    public void ThenTwoFileItemsShouldBeReturned()
    {
        Assert.Equal(2, _result.Count);
    }
}
