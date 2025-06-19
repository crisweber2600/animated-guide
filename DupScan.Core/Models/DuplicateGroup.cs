using System.Collections.Generic;
using System.Linq;

namespace DupScan.Core.Models;

public class DuplicateGroup
{
    public DuplicateGroup(string hash, IEnumerable<FileItem> files)
    {
        Hash = hash;
        Files = files.ToList().AsReadOnly();
        RecoverableBytes = Files.Sum(f => f.Size) - Files.Max(f => f.Size);
    }

    public string Hash { get; }
    public IReadOnlyList<FileItem> Files { get; }
    public long RecoverableBytes { get; }
}
