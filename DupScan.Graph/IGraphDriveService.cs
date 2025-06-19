using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Microsoft.Graph;

namespace DupScan.Graph;

public interface IGraphDriveService
{
    Task<DriveItemCollectionResponse> GetRootChildrenAsync();

    Task CreateShortcutAsync(string itemId, string targetId);

    Task DeleteItemAsync(string itemId);
}
