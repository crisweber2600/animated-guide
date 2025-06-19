using DupScan.Core.Infrastructure.Workers;
using DupScan.Core.Models;
using DupScan.Core.Services;

namespace DupScan.Orchestration;

public class ScanOrchestrator
{
    private readonly LocalScanner _scanner;
    private readonly DuplicateDetector _detector;
    private readonly FileLinkService _linker;

    public ScanOrchestrator(LocalScanner scanner, DuplicateDetector detector, FileLinkService linker)
    {
        _scanner = scanner;
        _detector = detector;
        _linker = linker;
    }

    public async Task<IReadOnlyList<DuplicateGroup>> ExecuteAsync(IEnumerable<string> roots, bool link, int parallel)
    {
        var files = await _scanner.ScanAsync(roots);
        var groups = _detector.FindDuplicates(files);

        if (link)
        {
            using var queue = new WorkerQueue(parallel);
            foreach (var g in groups)
            {
                await queue.EnqueueAsync(() => _linker.LinkAsync(g));
            }
            await queue.DisposeAsync();
        }

        return groups;
    }
}
