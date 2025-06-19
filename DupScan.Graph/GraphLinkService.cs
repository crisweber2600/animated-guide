using System.Linq;
using System.Threading.Tasks;
using DupScan.Core.Models;
using DupScan.Orchestration;

namespace DupScan.Graph;

public class GraphLinkService : ILinkService
{
    private readonly IGraphDriveService _drive;

    public GraphLinkService(IGraphDriveService drive)
    {
        _drive = drive;
    }

    public async Task LinkAsync(DuplicateGroup group)
    {
        var keep = group.Files.OrderByDescending(f => f.Size).First();
        foreach (var dup in group.Files.Where(f => f != keep))
        {
            await _drive.CreateShortcutAsync(dup.Id, keep.Id);
            await _drive.DeleteItemAsync(dup.Id);
        }
    }
}
