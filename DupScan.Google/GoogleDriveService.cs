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
}
