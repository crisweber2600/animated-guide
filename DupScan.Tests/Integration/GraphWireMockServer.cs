using System.Text.Json;
using DriveItem = Microsoft.Graph.Models.DriveItem;
using WireMock.Server;
using WireMockRequest = WireMock.RequestBuilders.Request;
using WireMockResponse = WireMock.ResponseBuilders.Response;
using WireMock.RequestBuilders;
using WMRequest = WireMock.RequestBuilders.Request;
using WireMock.ResponseBuilders;

namespace DupScan.Tests.Integration;

public class GraphWireMockServer : IDisposable
{
    public WireMockServer Server { get; }

    public string Url => Server.Urls[0];

    public GraphWireMockServer()
    {
        Server = WireMockServer.Start();
    }

    public void SetupChildren(IEnumerable<DriveItem> items)
    {
        var body = JsonSerializer.Serialize(new { value = items });
        Server.Given(WMRequest.Create().WithPath("/drive/root/children").UsingGet())
              .RespondWith(Response.Create().WithBody(body).WithHeader("Content-Type", "application/json"));
    }

    public void ExpectShortcut(string sourceId, string targetId)
    {
        Server.Given(
                WMRequest.Create()
                    .WithPath($"/drive/items/{sourceId}/shortcut")
                    .UsingPost()
                    .WithBody($"{{\"targetId\":\"{targetId}\"}}"))
              .RespondWith(Response.Create().WithStatusCode(200));
        Server.Given(WMRequest.Create().WithPath($"/drive/items/{sourceId}").UsingDelete())
              .RespondWith(Response.Create().WithStatusCode(200));
    }

    public void Dispose()
    {
        Server.Stop();
        Server.Dispose();
    }
}
