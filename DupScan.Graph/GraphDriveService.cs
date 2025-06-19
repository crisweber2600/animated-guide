using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Microsoft.Graph;

namespace DupScan.Graph;

public class GraphDriveService : IGraphDriveService
{
    private readonly GraphServiceClient _client;

    public GraphDriveService(GraphServiceClient client)
    {
        _client = client;
    }

    public Task<DriveItemCollectionResponse> GetRootChildrenAsync()
    {
        // TODO: query Microsoft Graph for drive items
        return Task.FromResult(new DriveItemCollectionResponse());
    }

    public Task CreateShortcutAsync(string itemId, string targetId)
    {
        // TODO: call Graph API to replace item with shortcut
        return Task.CompletedTask;
    }

    public Task DeleteItemAsync(string itemId)
    {
        // TODO: delete the specified item
        return Task.CompletedTask;
    }
}
