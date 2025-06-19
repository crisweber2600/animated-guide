using System.Text.Json;
using Google.Apis.Drive.v3.Data;
using DupScan.Google;

namespace DupScan.Tests.Integration;

public class HttpGoogleDriveService : IGoogleDriveService
{
    private readonly HttpClient _client;

    public HttpGoogleDriveService(string baseUrl)
    {
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<IList<File>> ListFilesAsync()
    {
        var json = await _client.GetStringAsync("/files");
        var wrapper = JsonSerializer.Deserialize<FilesWrapper>(json);
        return wrapper?.files ?? new List<File>();
    }

    private record FilesWrapper(List<File> files);
}
