using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;

namespace DupScan.Graph;

public static class GraphClientFactory
{
    /// <summary>
    /// Creates a <see cref="GraphServiceClient"/> using the device-code flow.
    /// The user is prompted to sign in on first use and the access token is
    /// cached for subsequent requests.
    /// </summary>
    /// <param name="tenantId">Azure AD tenant identifier.</param>
    /// <param name="clientId">Registered client application ID.</param>
    /// <param name="scopes">Requested Graph permission scopes.</param>
    public static GraphServiceClient Create(string tenantId, string clientId, string[] scopes)
    {
        var options = new DeviceCodeCredentialOptions
        {
            TenantId = tenantId,
            ClientId = clientId,
            DeviceCodeCallback = (info, token) =>
            {
                Console.WriteLine(info.Message);
                return Task.CompletedTask;
            }
        };

        var credential = new DeviceCodeCredential(options);
        return new GraphServiceClient(credential, scopes);
    }
}
