using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace DupScan.Google;

public static class GoogleClientFactory
{
    public static DriveService Create(string clientId, string clientSecret, string[] scopes)
    {
        var credential = GoogleWebAuthorizationBroker
            .AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
            scopes,
            "user",
            CancellationToken.None).Result;

        return new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "DupScan"
        });
    }
}
