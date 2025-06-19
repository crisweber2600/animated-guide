using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using DupScan.Core.Models;

namespace DupScan.Graph;

public class GraphScanner
{
    private readonly IGraphDriveService _drive;

    public GraphScanner(IGraphDriveService drive)
    {
        _drive = drive;
    }

    public async Task<IReadOnlyList<FileItem>> ScanAsync()
    {
        var items = await ExecuteWithBackoffAsync(() => _drive.GetRootChildrenAsync());
        return items.Value?
            .Where(i => i.File != null)
            .Select(i => new FileItem(
                i.Id ?? string.Empty,
                i.Name ?? string.Empty,
                i.File?.Hashes?.QuickXorHash ?? string.Empty,
                i.Size ?? 0))
            .ToList() ?? new List<FileItem>();
    }

    private static async Task<T> ExecuteWithBackoffAsync<T>(Func<Task<T>> op, int maxAttempts = 5)
    {
        for (int attempt = 1; ; attempt++)
        {
            try
            {
                return await op();
            }
            catch (ServiceException ex)
                when ((int)ex.ResponseStatusCode == 429 || (int)ex.ResponseStatusCode >= 500)
            {
                if (attempt >= maxAttempts) throw;
                await Task.Delay(TimeSpan.FromSeconds(attempt * attempt));
            }
        }
    }
}
