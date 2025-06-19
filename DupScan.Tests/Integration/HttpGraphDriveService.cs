using System.Text.Json;
using DupScan.Graph;
using Microsoft.Graph.Models;

namespace DupScan.Tests.Integration;

public class HttpGraphDriveService : IGraphDriveService
{
    private readonly HttpClient _client;

    public HttpGraphDriveService(string baseUrl)
    {
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<DriveItemCollectionResponse> GetRootChildrenAsync()
    {
        var json = await _client.GetStringAsync("/drive/root/children");
        return JsonSerializer.Deserialize<DriveItemCollectionResponse>(json) ?? new DriveItemCollectionResponse();
    }

    public async Task CreateShortcutAsync(string itemId, string targetId)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { targetId }), System.Text.Encoding.UTF8, "application/json");
        await _client.PostAsync($"/drive/items/{itemId}/shortcut", content);
    }

    public async Task DeleteItemAsync(string itemId)
    {
        await _client.DeleteAsync($"/drive/items/{itemId}");
    }
}
