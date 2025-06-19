using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Drive.v3;
using Google.Apis.Http;
using Google.Apis.Services;
using DupScan.Google;
using Xunit;

namespace DupScan.Tests.Unit;

public class GoogleDriveServiceTests
{
    [Fact]
    public async Task CreateShortcutAsync_makes_expected_requests()
    {
        var handler = new RecordingHandler();
        var service = CreateService(handler);
        var gds = new GoogleDriveService(service);

        await gds.CreateShortcutAsync("1", "2");

        Assert.Equal(3, handler.Requests.Count);
        Assert.Equal(HttpMethod.Get, handler.Requests[0].Method);
        Assert.Contains("/drive/v3/files/1", handler.Requests[0].RequestUri!.ToString());
        Assert.Equal(HttpMethod.Post, handler.Requests[1].Method);
        Assert.Contains("/drive/v3/files", handler.Requests[1].RequestUri!.ToString());
        Assert.Equal(HttpMethod.Delete, handler.Requests[2].Method);
        Assert.Contains("/drive/v3/files/1", handler.Requests[2].RequestUri!.ToString());
    }

    [Fact]
    public async Task DeleteFileAsync_makes_delete_request()
    {
        var handler = new RecordingHandler();
        var service = CreateService(handler);
        var gds = new GoogleDriveService(service);

        await gds.DeleteFileAsync("1");

        var req = Assert.Single(handler.Requests);
        Assert.Equal(HttpMethod.Delete, req.Method);
        Assert.Contains("/drive/v3/files/1", req.RequestUri!.ToString());
    }

    private static DriveService CreateService(RecordingHandler handler)
    {
        var factory = new HandlerFactory(handler);
        return new DriveService(new BaseClientService.Initializer
        {
            HttpClientFactory = factory,
            ApplicationName = "test"
        });
    }

    private class HandlerFactory : global::Google.Apis.Http.IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;
        public HandlerFactory(HttpMessageHandler handler) => _handler = handler;
        public ConfigurableHttpClient CreateHttpClient(CreateHttpClientArgs args)
        {
            return new ConfigurableHttpClient(new ConfigurableMessageHandler(_handler));
        }
    }

    private class RecordingHandler : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = new();
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            });
        }
    }
}
