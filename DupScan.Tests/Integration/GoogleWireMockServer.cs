using System.Text.Json;
using GoogleFile = Google.Apis.Drive.v3.Data.File;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace DupScan.Tests.Integration;

public class GoogleWireMockServer : IDisposable
{
    public WireMockServer Server { get; }

    public string Url => Server.Urls[0];

    public GoogleWireMockServer()
    {
        Server = WireMockServer.Start();
    }

    public void SetupFiles(IEnumerable<GoogleFile> files)
    {
        var body = JsonSerializer.Serialize(new { files });
        Server.Given(Request.Create().WithPath("/files").UsingGet())
              .RespondWith(Response.Create().WithBody(body).WithHeader("Content-Type", "application/json"));
    }

    public void Dispose()
    {
        Server.Stop();
        Server.Dispose();
    }
}
