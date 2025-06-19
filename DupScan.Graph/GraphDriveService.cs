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
}
