using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace DupScan.Google;

public static class GoogleClientFactory
{
    /// <summary>
    /// Creates a <see cref="DriveService"/> using the OAuth desktop flow. A local
    /// HTTP listener captures the authorization code and the token is cached on
    /// disk for reuse.
    /// </summary>
    /// <param name="clientId">Google Cloud OAuth client ID.</param>
    /// <param name="clientSecret">Google Cloud OAuth client secret.</param>
    /// <param name="scopes">Requested Drive scopes.</param>
    public static DriveService Create(string clientId, string clientSecret, string[] scopes)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
            Scopes = scopes,
            DataStore = new FileDataStore("dup-scan-google")
        });

        var codeReceiver = new LocalServerCodeReceiver();
        var app = new AuthorizationCodeInstalledApp(flow, codeReceiver);
        var credential = app.AuthorizeAsync("user", CancellationToken.None).Result;

        return new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "DupScan"
        });
    }
}
