using System.Collections.Generic;
using System.Threading.Tasks;
using DupScan.Core.Models;
using DupScan.Google;
using DriveFile = Google.Apis.Drive.v3.Data.File;
using Moq;
using Xunit;

namespace DupScan.Tests.Unit;

public class GoogleScannerTests
{
    [Fact]
    public async Task ScanAsync_converts_drive_files_to_file_items()
    {
        var driveFiles = new List<DriveFile>
        {
            new DriveFile { Id = "1", Name = "a.txt", Md5Checksum = "h1", Size = 10 },
            new DriveFile { Id = "2", Name = "b.txt", Md5Checksum = "h2", Size = 20 }
        };

        var mock = new Mock<IGoogleDriveService>();
        mock.Setup(m => m.ListFilesAsync()).ReturnsAsync(driveFiles);

        var scanner = new GoogleScanner(mock.Object);
        var files = await scanner.ScanAsync();

        Assert.Collection(files,
            f => Assert.Equal("h1", f.Hash),
            f => Assert.Equal("h2", f.Hash));
    }
}
