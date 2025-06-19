using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;

namespace DupScan.Graph;

public static class GraphClientFactory
{
    public static GraphServiceClient Create(string tenantId, string clientId, string[] scopes)
    {
        var options = new DeviceCodeCredentialOptions
        {
            TenantId = tenantId,
            ClientId = clientId,
            DeviceCodeCallback = (code, ct) =>
            {
                Console.WriteLine(code.Message);
                return Task.CompletedTask;
            }
        };

        var credential = new DeviceCodeCredential(options);
        return new GraphServiceClient(credential, scopes);
    }
}
