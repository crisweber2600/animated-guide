using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DupScan.Tests.Integration;
using Reqnroll;
using Xunit;

namespace DupScan.Tests.Steps;

[Binding]
public class CliLinkingSteps : IDisposable
{
    private GraphWireMockServer? _server;
    private string? _output;

    [Given("a Graph server expecting a shortcut from (.*) to (.*)")]
    public void GivenGraphServer(string sourceId, string targetId)
    {
        _server = new GraphWireMockServer();
        _server.ExpectShortcut(sourceId, targetId);
    }

    [When("I run the CLI with --link")]
    public void WhenIRunTheCliWithLink()
    {
        if (_server == null) throw new InvalidOperationException();
        var projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../DupScan.Cli"));
        var psi = new ProcessStartInfo("dotnet", $"run --project \"{projectDir}\" -- --link --graph-url {_server.Url}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        var process = Process.Start(psi)!;
        _output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
    }

    [Then("the Graph server should receive (.*) requests")]
    public void ThenServerShouldReceive(int count)
    {
        Assert.Equal(count, _server!.Server.LogEntries.Count());
    }

    public void Dispose()
    {
        _server?.Dispose();
    }
}
