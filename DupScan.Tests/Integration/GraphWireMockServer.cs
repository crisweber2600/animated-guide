using System.Text.Json;
using Microsoft.Graph.Models;
using WireMock.Server;
using WireMockRequest = WireMock.RequestBuilders.Request;
using WireMockResponse = WireMock.ResponseBuilders.Response;
using WireMock.RequestBuilders;

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
        Server.Given(WireMockRequest.Create().WithPath("/drive/root/children").UsingGet())
              .RespondWith(WireMockResponse.Create().WithBodyAsJson(body));
    }

    public void ExpectShortcut(string sourceId, string targetId)
    {
        Server.Given(WireMockRequest.Create().WithPath($"/drive/items/{sourceId}/shortcut").UsingPost())
              .RespondWith(WireMockResponse.Create().WithStatusCode(200));
        Server.Given(WireMockRequest.Create().WithPath($"/drive/items/{sourceId}").UsingDelete())
              .RespondWith(WireMockResponse.Create().WithStatusCode(200));
    }

    public void Dispose()
    {
        Server.Stop();
        Server.Dispose();
    }
}
