using System.Text.Json;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using DupScan.Google;

namespace DupScan.Tests.Integration;

public class HttpGoogleDriveService : IGoogleDriveService
{
    private readonly HttpClient _client;

    public HttpGoogleDriveService(string baseUrl)
    {
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<IList<GoogleFile>> ListFilesAsync()
    {
        var json = await _client.GetStringAsync("/files");
        var wrapper = JsonSerializer.Deserialize<FilesWrapper>(json);
        return wrapper?.files ?? new List<GoogleFile>();
    }

    public async Task CreateShortcutAsync(string fileId, string targetId)
    {
        var json = JsonSerializer.Serialize(new { targetId });
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        await _client.PostAsync($"/files/{fileId}/shortcut", content);
    }

    public async Task DeleteFileAsync(string fileId)
    {
        await _client.DeleteAsync($"/files/{fileId}");
    }

    private record FilesWrapper(List<GoogleFile> files);
}
