using System.Linq;
using System.Threading.Tasks;
using DupScan.Core.Models;

namespace DupScan.Google;

public class GoogleLinkService
{
    private readonly IGoogleDriveService _drive;

    public GoogleLinkService(IGoogleDriveService drive)
    {
        _drive = drive;
    }

    public async Task LinkAsync(DuplicateGroup group)
    {
        var keep = group.Files.OrderByDescending(f => f.Size).First();
        foreach (var dup in group.Files.Where(f => f != keep))
        {
            await _drive.CreateShortcutAsync(dup.Id, keep.Id);
            await _drive.DeleteFileAsync(dup.Id);
        }
    }
}
