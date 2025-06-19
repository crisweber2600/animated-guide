using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DupScan.Core.Models;

namespace DupScan.Google;

public class GoogleScanner
{
    private readonly IGoogleDriveService _drive;

    public GoogleScanner(IGoogleDriveService drive)
    {
        _drive = drive;
    }

    public async Task<IReadOnlyList<FileItem>> ScanAsync()
    {
        var files = await _drive.ListFilesAsync();
        return files
            .Where(f => !string.IsNullOrEmpty(f.Md5Checksum))
            .Select(f => new FileItem(
                f.Id ?? string.Empty,
                f.Name ?? string.Empty,
                f.Md5Checksum,
                (long?)f.Size ?? 0))
            .ToList();
    }
}
