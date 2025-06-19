using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using GoogleFile = Google.Apis.Drive.v3.Data.File;

namespace DupScan.Google;

public class GoogleDriveService : IGoogleDriveService
{
    private readonly DriveService _service;

    public GoogleDriveService(DriveService service)
    {
        _service = service;
    }

    public async Task<IList<GoogleFile>> ListFilesAsync()
    {
        var request = _service.Files.List();
        request.Fields = "files(id,name,md5Checksum,size)";
        var result = await request.ExecuteAsync();
        return result.Files ?? new List<GoogleFile>();
    }

    public Task CreateShortcutAsync(string fileId, string targetId)
    {
        // TODO: call Google Drive API to replace the file with a shortcut
        return Task.CompletedTask;
    }

    public Task DeleteFileAsync(string fileId)
    {
        // TODO: delete the specified file
        return Task.CompletedTask;
    }
}
