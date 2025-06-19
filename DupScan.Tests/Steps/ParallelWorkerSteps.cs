using DupScan.Core.Infrastructure.Workers;
using Reqnroll;
using System.Diagnostics;
using Xunit;

namespace DupScan.Tests.Steps;

[Binding]
public class ParallelWorkerSteps
{
    private WorkerQueue _worker = null!;
    private readonly Stopwatch _sw = new();

    [Given(@"a worker with degree (.*)")]
    public void GivenWorker(int degree)
    {
        _worker = new WorkerQueue(degree);
    }

    [When(@"I enqueue (\d+) tasks lasting (\d+)ms")]
    public async Task WhenIEnqueueTasks(int count, int ms)
    {
        _sw.Start();
        var tasks = Enumerable.Range(0, count)
            .Select(_ => _worker.EnqueueAsync(async () => await Task.Delay(ms)))
            .ToArray();
        await Task.WhenAll(tasks);
        await _worker.DisposeAsync();
        _sw.Stop();
    }

    [Then(@"execution time should be under (\d+)ms")]
    public void ThenExecutionTimeShouldBeUnder(int limit)
    {
        Assert.InRange(_sw.ElapsedMilliseconds, 0, limit);
    }
}
