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
        var files = new List<GoogleFile>();
        string? pageToken = null;

        do
        {
            var request = _service.Files.List();
            request.Fields = "nextPageToken,files(id,name,md5Checksum,size)";
            request.PageToken = pageToken;
            request.Q = "md5Checksum != null";

            var result = await request.ExecuteAsync().ConfigureAwait(false);
            if (result.Files != null)
            {
                files.AddRange(result.Files);
            }

            pageToken = result.NextPageToken;
        }
        while (!string.IsNullOrEmpty(pageToken));

        return files;
    }
}
