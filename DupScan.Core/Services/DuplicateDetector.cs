using System.Collections.Generic;
using System.Linq;
using DupScan.Core.Models;

namespace DupScan.Core.Services;

public class DuplicateDetector
{
    public IReadOnlyList<DuplicateGroup> FindDuplicates(IEnumerable<FileItem> files)
    {
        return files
            .GroupBy(f => f.Hash)
            .Select(g => new
            {
                Hash = g.Key,
                Items = g.ToList(),
                Recoverable = g.Sum(f => f.Size) - g.Max(f => f.Size)
            })
            .Where(g => g.Items.Count > 1)
            .OrderByDescending(g => g.Recoverable)
            .Select(g => new DuplicateGroup(g.Hash, g.Items))
            .ToList();
    }
}
