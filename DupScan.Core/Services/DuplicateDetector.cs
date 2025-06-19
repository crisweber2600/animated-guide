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
            .Where(g => g.Count() > 1)
            .Select(g => new DuplicateGroup(g.Key, g))
            .OrderByDescending(g => g.RecoverableBytes)
            .ToList();
    }
}
