using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using GoogleApiException = Google.GoogleApiException;

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

    public async Task CreateShortcutAsync(string fileId, string targetId)
    {
        try
        {
            var get = _service.Files.Get(fileId);
            get.Fields = "name,parents";
            var original = await get.ExecuteAsync().ConfigureAwait(false);

            var shortcut = new GoogleFile
            {
                Name = original.Name,
                MimeType = "application/vnd.google-apps.shortcut",
                ShortcutDetails = new GoogleFile.ShortcutDetailsData
                {
                    TargetId = targetId
                },
                Parents = original.Parents
            };

            var create = _service.Files.Create(shortcut);
            create.Fields = "id";
            await create.ExecuteAsync().ConfigureAwait(false);

            await DeleteFileAsync(fileId).ConfigureAwait(false);
        }
        catch (GoogleApiException ex)
        {
            throw new InvalidOperationException($"Failed to create shortcut for {fileId}", ex);
        }
    }

    public async Task DeleteFileAsync(string fileId)
    {
        try
        {
            var request = _service.Files.Delete(fileId);
            await request.ExecuteAsync().ConfigureAwait(false);
        }
        catch (GoogleApiException ex)
        {
            throw new InvalidOperationException($"Failed to delete file {fileId}", ex);
        }
    }
}
