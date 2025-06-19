using DupScan.Core.Models;

namespace DupScan.Orchestration;

public class FileLinkService
{
    public Task LinkAsync(DuplicateGroup group)
    {
        var keep = group.Files.OrderByDescending(f => f.Size).First();
        foreach (var dup in group.Files.Where(f => f != keep))
        {
            if (File.Exists(dup.Path))
            {
                File.Delete(dup.Path);
                File.CreateSymbolicLink(dup.Path, keep.Path);
            }
        }
        return Task.CompletedTask;
    }
}
