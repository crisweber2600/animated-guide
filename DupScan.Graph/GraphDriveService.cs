using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;

namespace DupScan.Graph;

public class GraphDriveService : IGraphDriveService
{
    private readonly GraphServiceClient _client;

    public GraphDriveService(GraphServiceClient client)
    {
        _client = client;
    }

    public async Task<DriveItemCollectionResponse> GetRootChildrenAsync()
    {
        var info = new RequestInformation
        {
            HttpMethod = Method.GET,
            UrlTemplate = "{+baseurl}/me/drive/root/children{?%24select}",
        };
        var baseUrl = _client.RequestAdapter.BaseUrl ?? string.Empty;
        info.PathParameters.Add("baseurl", baseUrl);
        info.QueryParameters.Add("%24select", "id,name,size,file");
        var response = await _client.RequestAdapter.SendAsync(info, DriveItemCollectionResponse.CreateFromDiscriminatorValue, default);
        return response ?? new DriveItemCollectionResponse();
    }

    public async Task CreateShortcutAsync(string itemId, string targetId)
    {
        var update = new DriveItem
        {
            AdditionalData = new Dictionary<string, object>
            {
                ["shortcut"] = new Dictionary<string, object>
                {
                    ["targetId"] = targetId
                },
                ["@microsoft.graph.conflictBehavior"] = "replace"
            }
        };

        var info = new RequestInformation
        {
            HttpMethod = Method.PATCH,
            UrlTemplate = "{+baseurl}/me/drive/items/{itemId}",
        };
        var baseUrl = _client.RequestAdapter.BaseUrl ?? string.Empty;
        info.PathParameters.Add("baseurl", baseUrl);
        info.PathParameters.Add("itemId", itemId);
        info.SetContentFromParsable(_client.RequestAdapter, "application/json", update);
        await _client.RequestAdapter.SendNoContentAsync(info);
    }

    public async Task DeleteItemAsync(string itemId)
    {
        var info = new RequestInformation
        {
            HttpMethod = Method.DELETE,
            UrlTemplate = "{+baseurl}/me/drive/items/{itemId}",
        };
        var baseUrl = _client.RequestAdapter.BaseUrl ?? string.Empty;
        info.PathParameters.Add("baseurl", baseUrl);
        info.PathParameters.Add("itemId", itemId);
        await _client.RequestAdapter.SendNoContentAsync(info);
    }
}
