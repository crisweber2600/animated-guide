using System.Collections.Concurrent;
using System.Threading.Channels;

namespace DupScan.Core.Infrastructure.Workers;

public sealed class WorkerQueue : IAsyncDisposable
{
    private readonly Channel<Func<Task>> _channel;
    private readonly List<Task> _workers;

    public WorkerQueue(int degreeOfParallelism)
    {
        _channel = Channel.CreateUnbounded<Func<Task>>();
        _workers = Enumerable.Range(0, degreeOfParallelism)
            .Select(_ => Task.Run(WorkerLoopAsync))
            .ToList();
    }

    public async Task EnqueueAsync(Func<Task> work) => await _channel.Writer.WriteAsync(work);

    private async Task WorkerLoopAsync()
    {
        while (await _channel.Reader.WaitToReadAsync())
        {
            while (_channel.Reader.TryRead(out var work))
            {
                try
                {
                    await work();
                }
                catch
                {
                    // swallow exceptions to keep worker alive
                }
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.Complete();
        await Task.WhenAll(_workers);
    }
}
