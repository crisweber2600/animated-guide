using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        var files = await ExecuteWithBackoffAsync(() => _drive.ListFilesAsync());
        return files
            .Where(f => !string.IsNullOrEmpty(f.Md5Checksum))
            .Select(f => new FileItem(
                f.Id ?? string.Empty,
                f.Name ?? string.Empty,
                f.Md5Checksum,
                (long?)f.Size ?? 0))
            .ToList();
    }

    private static async Task<T> ExecuteWithBackoffAsync<T>(Func<Task<T>> op, int maxAttempts = 5)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                return await op();
            }
            catch (global::Google.GoogleApiException ex)
                when (ex.HttpStatusCode == HttpStatusCode.TooManyRequests || (int)ex.HttpStatusCode >= 500)
            {
                if (attempt >= maxAttempts) throw;
                await Task.Delay(TimeSpan.FromSeconds(attempt * attempt));
            }
        }
    }
}
