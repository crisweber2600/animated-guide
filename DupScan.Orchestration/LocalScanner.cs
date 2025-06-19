using DupScan.Core.Models;
using System.Security.Cryptography;

namespace DupScan.Orchestration;

public class LocalScanner
{
    public async Task<IReadOnlyList<FileItem>> ScanAsync(IEnumerable<string> roots)
    {
        var items = new List<FileItem>();
        foreach (var root in roots)
        {
            if (!Directory.Exists(root)) continue;
            foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
            {
                var hash = await GetHashAsync(path);
                var info = new FileInfo(path);
                items.Add(new FileItem(path, path, hash, info.Length));
            }
        }
        return items;
    }

    private static async Task<string> GetHashAsync(string path)
    {
        using var stream = File.OpenRead(path);
        using var md5 = MD5.Create();
        var hash = await md5.ComputeHashAsync(stream);
        return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
    }
}
